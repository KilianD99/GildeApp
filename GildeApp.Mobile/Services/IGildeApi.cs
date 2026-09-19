using GildeApp.Mobile.Models;

namespace GildeApp.Mobile.Services
{
    public interface IGildeApi
    {
        Task<ApiCall<List<TourneyModel>>> GetRunningTourneysAsync(CancellationToken ct = default);
        Task<ApiCall<List<MatchModel>>> GetMatchesAsync(Guid tourneyId, CancellationToken ct = default);
        Task<ApiCall<MatchDetailModel>> GetMatchAsync(Guid matchId, CancellationToken ct = default);

        /// <summary>Takes the match, or renews a claim we already hold.</summary>
        Task<ApiCall<MatchDetailModel>> ClaimAsync(Guid matchId, CancellationToken ct = default);

        /// <summary>Hands the match back without finishing it.</summary>
        Task<ApiCall<MatchDetailModel>> ReleaseAsync(Guid matchId, CancellationToken ct = default);

        /// <summary>Pushes a score. finish: false keeps it live, true locks it in.</summary>
        Task<ApiCall<MatchDetailModel>> SubmitScoreAsync(
            Guid matchId, int firstScore, int secondScore, bool finish, CancellationToken ct = default);
    }

    /// <summary>
    /// A call that either worked or came back with something worth showing the judge.
    /// <see cref="IsConflict"/> separates "someone else has this match" from a real
    /// failure, because the app reacts to the two differently.
    /// </summary>
    public class ApiCall<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public string Error { get; init; } = string.Empty;
        public bool IsConflict { get; init; }

        public static ApiCall<T> Ok(T? data) => new() { IsSuccess = true, Data = data };

        public static ApiCall<T> Fail(string error) =>
            new() { IsSuccess = false, Error = error };

        public static ApiCall<T> Conflict(string error) =>
            new() { IsSuccess = false, Error = error, IsConflict = true };
    }
}
