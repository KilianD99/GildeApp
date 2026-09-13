using Mvc.GildeApp.mvc.Models;

namespace Mvc.GildeApp.mvc.Services.Interfaces
{
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
}

