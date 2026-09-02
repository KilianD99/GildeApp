using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using Mvc.GildeApp.mvc.Models;

namespace Mvc.GildeApp.mvc.Services
{
    public class GildeApiClient : IGildeApiClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<GildeApiClient> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public GildeApiClient(HttpClient http, ILogger<GildeApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        // ---- tourneys ------------------------------------------------------

        public async Task<List<TourneyModel>> GetTourneysAsync(CancellationToken ct = default)
        {
            var result = await GetAsync<List<TourneyModel>>("api/tourney", ct);
            return result ?? new List<TourneyModel>();
        }

        public Task<TourneyDetailModel?> GetTourneyAsync(Guid id, CancellationToken ct = default)
        {
            return GetAsync<TourneyDetailModel>($"api/tourney/{id}", ct);
        }

        public Task<BoardModel?> GetBoardAsync(Guid id, CancellationToken ct = default)
        {
            return GetAsync<BoardModel>($"api/tourney/{id}/board", ct);
        }

        public Task<ApiCallResult<TourneyDetailModel>> CreateTourneyAsync(string name, Guid ruleSetId, CancellationToken ct = default)
        {
            return SendAsync<TourneyDetailModel>(HttpMethod.Post, "api/tourney",
                new { name, ruleSetId }, ct);
        }

        public Task<ApiCallResult<BoardModel>> GenerateAsync(Guid tourneyId, CancellationToken ct = default)
        {
            return SendAsync<BoardModel>(HttpMethod.Post, $"api/tourney/{tourneyId}/generate", null, ct);
        }

        public Task<ApiCallResult<BoardModel>> FinishAsync(Guid tourneyId, CancellationToken ct = default)
        {
            return SendAsync<BoardModel>(HttpMethod.Post, $"api/tourney/{tourneyId}/finish", null, ct);
        }

        public Task<ApiCallResult<object>> DeleteTourneyAsync(Guid tourneyId, CancellationToken ct = default)
        {
            return SendAsync<object>(HttpMethod.Delete, $"api/tourney/{tourneyId}", null, ct);
        }

        // ---- rule sets -----------------------------------------------------

        public async Task<List<RuleSetModel>> GetRuleSetsAsync(CancellationToken ct = default)
        {
            var result = await GetAsync<List<RuleSetModel>>("api/ruleset", ct);
            return result ?? new List<RuleSetModel>();
        }

        // ---- players -------------------------------------------------------

        public async Task<List<PlayerModel>> GetPlayersAsync(string? search = null, CancellationToken ct = default)
        {
            var url = string.IsNullOrWhiteSpace(search)
                ? "api/player"
                : $"api/player?search={Uri.EscapeDataString(search)}";

            var result = await GetAsync<List<PlayerModel>>(url, ct);
            return result ?? new List<PlayerModel>();
        }

        public Task<ApiCallResult<PlayerModel>> CreatePlayerAsync(string firstName, string lastName, CancellationToken ct = default)
        {
            return SendAsync<PlayerModel>(HttpMethod.Post, "api/player",
                new { firstName, lastName }, ct);
        }

        // ---- entries -------------------------------------------------------

        public Task<ApiCallResult<TourneyEntryModel>> AddEntryAsync(Guid tourneyId, Guid playerId, CancellationToken ct = default)
        {
            return SendAsync<TourneyEntryModel>(HttpMethod.Post, $"api/tourney/{tourneyId}/entries",
                new { playerId }, ct);
        }

        public Task<ApiCallResult<object>> RemoveEntryAsync(Guid tourneyId, Guid entryId, CancellationToken ct = default)
        {
            return SendAsync<object>(HttpMethod.Delete, $"api/tourney/{tourneyId}/entries/{entryId}", null, ct);
        }

        // ---- plumbing ------------------------------------------------------

        private async Task<T?> GetAsync<T>(string url, CancellationToken ct)
        {
            try
            {
                var response = await _http.GetAsync(url, ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("GET {Url} returned {Status}", url, response.StatusCode);
                    return default;
                }

                var wrapper = await response.Content.ReadFromJsonAsync<ApiResult<T>>(JsonOptions, ct);
                return wrapper is null ? default : wrapper.Data;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Could not reach the API for GET {Url}", url);
                return default;
            }
        }

        private async Task<ApiCallResult<T>> SendAsync<T>(HttpMethod method, string url, object? body, CancellationToken ct)
        {
            try
            {
                using var request = new HttpRequestMessage(method, url);

                if (body is not null)
                    request.Content = JsonContent.Create(body);

                var response = await _http.SendAsync(request, ct);
                var raw = await response.Content.ReadAsStringAsync(ct);

                if (!response.IsSuccessStatusCode)
                    return ApiCallResult<T>.Fail(ReadErrors(raw, response.StatusCode));

                if (string.IsNullOrWhiteSpace(raw))
                    return ApiCallResult<T>.Ok(default);

                var wrapper = JsonSerializer.Deserialize<ApiResult<T>>(raw, JsonOptions);
                return ApiCallResult<T>.Ok(wrapper is null ? default : wrapper.Data);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Could not reach the API for {Method} {Url}", method, url);
                return ApiCallResult<T>.Fail("The API is not responding. Is it running?");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "The API sent something unreadable for {Method} {Url}", method, url);
                return ApiCallResult<T>.Fail("The API sent back a response the app could not read.");
            }
        }

        /// <summary>
        /// The API returns errors three ways: a bare string array, a { message } object,
        /// or an MVC ModelState dictionary. Flatten whichever arrived into plain lines.
        /// </summary>
        private static List<string> ReadErrors(string raw, System.Net.HttpStatusCode status)
        {
            var fallback = new List<string> { $"The request failed ({(int)status})." };

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

                    return messages.Count > 0 ? messages : fallback;
                }

                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("message", out var message) &&
                        message.ValueKind == JsonValueKind.String)
                    {
                        return new List<string> { message.GetString()! };
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
                            return messages;
                    }
                }
            }
            catch (JsonException)
            {
                // Not JSON at all; fall through to the generic message.
            }

            return fallback;
        }
    }
}
