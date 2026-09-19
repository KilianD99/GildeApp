using System;
using System.Collections.Generic;
using System.Text;
using GildeApp.Mobile.Core.Models;

namespace GildeApp.Mobile.Core.Services.Interfaces
{
    public interface IGildeApi
    {
        Task<ApiCall<List<TourneyModel>>> GetRunningTourneysAsync(CancellationToken ct = default);
        Task<ApiCall<List<MatchModel>>> GetMatchesAsync(Guid tourneyId, CancellationToken ct = default);
        Task<ApiCall<MatchDetailModel>> GetMatchAsync(Guid matchId, CancellationToken ct = default);
        Task<ApiCall<MatchDetailModel>> ClaimAsync(Guid matchId, CancellationToken ct = default);
        Task<ApiCall<MatchDetailModel>> ReleaseAsync(Guid matchId, CancellationToken ct = default);
        Task<ApiCall<MatchDetailModel>> SubmitScoreAsync(
            Guid matchId, int firstScore, int secondScore, bool finish, CancellationToken ct = default);
    }
}
