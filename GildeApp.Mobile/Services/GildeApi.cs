using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using GildeApp.Mobile.Models;

namespace GildeApp.Mobile.Services
{
    public class GildeApi : IGildeApi
    {
        /// <summary>Must match MatchController.JudgeHeader on the API.</summary>
        private const string JudgeHeader = "X-Judge-Name";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _http;
        private readonly JudgeIdentity _judge;

        public GildeApi(HttpClient http, JudgeIdentity judge)
        {
            _http = http;
            _judge = judge;
        }

        public async Task<ApiCall<List<TourneyModel>>> GetRunningTourneysAsync(CancellationToken ct = default)
        {
            var all = await GetAsync<List<TourneyModel>>("api/tourney", ct);

            if (!all.IsSuccess || all.Data is null)
                return all;

            // A judge only ever scores a tourney that is under way.
            var running = all.Data
                .Where(t => t.Status == "Running")
                .OrderBy(t => t.Name)
                .ToList();

            return ApiCall<List<TourneyModel>>.Ok(running);
        }

        public Task<ApiCall<List<MatchModel>>> GetMatchesAsync(Guid tourneyId, CancellationToken ct = default)
        {
            return GetAsync<List<MatchModel>>($"api/match/tourney/{tourneyId}", ct);
        }

        public Task<ApiCall<MatchDetailModel>> GetMatchAsync(Guid matchId, CancellationToken ct = default)
        {
            return GetAsync<MatchDetailModel>($"api/match/{matchId}", ct);
        }

        public Task<ApiCall<MatchDetailModel>> ClaimAsync(Guid matchId, CancellationToken ct = default)
        {
            return SendAsync<MatchDetailModel>(HttpMethod.Post, $"api/match/{matchId}/claim", null, ct);
        }

        public Task<ApiCall<MatchDetailModel>> ReleaseAsync(Guid matchId, CancellationToken ct = default)
        {
            return SendAsync<MatchDetailModel>(HttpMethod.Post, $"api/match/{matchId}/release", null, ct);
        }

        public Task<ApiCall<MatchDetailModel>> SubmitScoreAsync(
            Guid matchId, int firstScore, int secondScore, bool finish, CancellationToken ct = default)
        {
            return SendAsync<MatchDetailModel>(
                HttpMethod.Put,
                $"api/match/{matchId}/score",
                new { firstScore, secondScore, finish },
                ct);
        }

        // ---- plumbing ------------------------------------------------------

        private HttpRequestMessage BuildRequest(HttpMethod method, string url, object? body)
        {
            var request = new HttpRequestMessage(method, url);

            // Every call carries the judge's name; the API needs it to decide who
            // owns a claim.
            if (_judge.Name is { } name)
                request.Headers.Add(JudgeHeader, name);

            if (body is not null)
                request.Content = JsonContent.Create(body);

            return request;
        }

        private async Task<ApiCall<T>> GetAsync<T>(string url, CancellationToken ct)
        {
            return await SendAsync<T>(HttpMethod.Get, url, null, ct);
        }

        private async Task<ApiCall<T>> SendAsync<T>(HttpMethod method, string url, object? body, CancellationToken ct)
        {
            try
            {
                using var request = BuildRequest(method, url, body);
                using var response = await _http.SendAsync(request, ct);

                var raw = await response.Content.ReadAsStringAsync(ct);

                if (!response.IsSuccessStatusCode)
                {
                    var message = ReadError(raw, response.StatusCode);

                    // 409 is the API saying another judge holds this match. That is
                    // an expected outcome, not a fault, so it gets its own flag.
                    return response.StatusCode == HttpStatusCode.Conflict
                        ? ApiCall<T>.Conflict(message)
                        : ApiCall<T>.Fail(message);
                }

                if (string.IsNullOrWhiteSpace(raw))
                    return ApiCall<T>.Ok(default);

                var wrapper = JsonSerializer.Deserialize<ApiResult<T>>(raw, JsonOptions);
                return ApiCall<T>.Ok(wrapper is null ? default : wrapper.Data);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (HttpRequestException)
            {
                return ApiCall<T>.Fail(
                    "Cannot reach the scoring server. Check the wifi and that the API is running.");
            }
            catch (JsonException)
            {
                return ApiCall<T>.Fail("The server sent back something the app could not read.");
            }
        }

        /// <summary>
        /// The API reports errors as a bare string array, a { message } object, or an
        /// MVC ModelState dictionary. Flatten whichever arrived into one line.
        /// </summary>
        private static string ReadError(string raw, HttpStatusCode status)
        {
            var fallback = $"The request failed ({(int)status}).";

            if (string.IsNullOrWhiteSpace(raw))
                return fallback;

            try
            {
                using var document = JsonDocument.Parse(raw);
                var root = document.RootElement;

                if (root.ValueKind == JsonValueKind.Array)
                {
                    var messages = root.EnumerateArray()
                        .Where(e => e.ValueKind == JsonValueKind.String)
                        .Select(e => e.GetString()!)
                        .ToList();

                    return messages.Count > 0 ? string.Join(" ", messages) : fallback;
                }

                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("message", out var message) &&
                        message.ValueKind == JsonValueKind.String)
                    {
                        return message.GetString()!;
                    }

                    if (root.TryGetProperty("errors", out var errors) &&
                        errors.ValueKind == JsonValueKind.Object)
                    {
                        var messages = new List<string>();

                        foreach (var field in errors.EnumerateObject())
                        {
                            if (field.Value.ValueKind != JsonValueKind.Array)
                                continue;

                            messages.AddRange(field.Value.EnumerateArray()
                                .Where(e => e.ValueKind == JsonValueKind.String)
                                .Select(e => e.GetString()!));
                        }

                        if (messages.Count > 0)
                            return string.Join(" ", messages);
                    }
                }
            }
            catch (JsonException)
            {
                // Not JSON; fall through.
            }

            return fallback;
        }
    }
}
