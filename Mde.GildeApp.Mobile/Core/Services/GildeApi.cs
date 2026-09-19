using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using GildeApp.Mobile.Core.Models;
using GildeApp.Mobile.Core.Services;
using GildeApp.Mobile.Core.Services.Interfaces;

namespace GildeApp.Mobile.Services
{
    public class GildeApi : IGildeApi
    {
        private const string JudgeHeader = "X-Judge-Name";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _http;
        private readonly JudgeIdentity _judge;
        private readonly ServerSettings _server;

        public GildeApi(HttpClient http, JudgeIdentity judge, ServerSettings server)
        {
            _http = http;
            _judge = judge;
            _server = server;
        }

        public async Task<ApiCall<List<TourneyModel>>> GetRunningTourneysAsync(CancellationToken ct = default)
        {
            var all = await GetAsync<List<TourneyModel>>("api/tourney", ct);

            if (!all.IsSuccess || all.Data is null)
                return all;

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
        private HttpRequestMessage BuildRequest(HttpMethod method, string url, object? body)
        {
            // Absolute URI built per request rather than relying on
            // HttpClient.BaseAddress: BaseAddress is fixed when the client is created
            // at startup, so a judge changing the server address in settings would
            // otherwise keep hitting the old one until the app restarted.
            var request = new HttpRequestMessage(method, new Uri(_server.BaseUri, url));

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
                    $"Cannot reach the scoring server at {_server.BaseUrl}. " +
                    "Check the wifi, the address in settings, and that the API is running.");
            }
            catch (UriFormatException)
            {
                return ApiCall<T>.Fail(
                    $"'{_server.BaseUrl}' is not a usable server address. Fix it in settings.");
            }
            catch (JsonException)
            {
                return ApiCall<T>.Fail("The server sent back something the app could not read.");
            }
        }
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
                
            }

            return fallback;
        }
    }
}