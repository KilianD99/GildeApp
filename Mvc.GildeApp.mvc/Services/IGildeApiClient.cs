using System;
using System.Collections.Generic;
using Mvc.GildeApp.mvc.Models;

namespace Mvc.GildeApp.mvc.Services
{
    /// <summary>
    /// The only thing in the web app that knows the API exists. Everything the views
    /// render comes through here, which is what keeps the web app and the mobile app
    /// reading the same numbers.
    /// </summary>
    public interface IGildeApiClient
    {
        Task<List<TourneyModel>> GetTourneysAsync(CancellationToken ct = default);
        Task<TourneyDetailModel?> GetTourneyAsync(Guid id, CancellationToken ct = default);
        Task<BoardModel?> GetBoardAsync(Guid id, CancellationToken ct = default);

        Task<ApiCallResult<TourneyDetailModel>> CreateTourneyAsync(string name, Guid ruleSetId, CancellationToken ct = default);
        Task<ApiCallResult<BoardModel>> GenerateAsync(Guid tourneyId, CancellationToken ct = default);
        Task<ApiCallResult<BoardModel>> FinishAsync(Guid tourneyId, CancellationToken ct = default);
        Task<ApiCallResult<object>> DeleteTourneyAsync(Guid tourneyId, CancellationToken ct = default);

        Task<List<RuleSetModel>> GetRuleSetsAsync(CancellationToken ct = default);

        Task<List<PlayerModel>> GetPlayersAsync(string? search = null, CancellationToken ct = default);
        Task<ApiCallResult<PlayerModel>> CreatePlayerAsync(string firstName, string lastName, CancellationToken ct = default);

        Task<ApiCallResult<TourneyEntryModel>> AddEntryAsync(Guid tourneyId, Guid playerId, CancellationToken ct = default);
        Task<ApiCallResult<object>> RemoveEntryAsync(Guid tourneyId, Guid entryId, CancellationToken ct = default);
    }

    /// <summary>
    /// A call that either worked or came back with messages worth showing the user.
    /// </summary>
    public class ApiCallResult<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public List<string> Errors { get; init; } = new();

        public static ApiCallResult<T> Ok(T? data) => new() { IsSuccess = true, Data = data };

        public static ApiCallResult<T> Fail(params string[] errors) =>
            new() { IsSuccess = false, Errors = errors.ToList() };

        public static ApiCallResult<T> Fail(IEnumerable<string> errors) =>
            new() { IsSuccess = false, Errors = errors.ToList() };
    }
}
