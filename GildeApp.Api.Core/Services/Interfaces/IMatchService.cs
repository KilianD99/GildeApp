using System;
using System.Collections.Generic;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Models;

namespace GildeApp.Api.Core.Services.Interfaces
{
    public interface IMatchService
    {
        IQueryable<Match> GetAllMatches();
        Task<ResultModel<IEnumerable<Match>>> ListAllAsync();
        Task<ResultModel<IEnumerable<Match>>> ListForTourneyAsync(Guid tourneyId);
        Task<ResultModel<Match>> GetByIdAsync(Guid id);
        Task<bool> DoesMatchIdExistsAsync(Guid id);

        Task<ResultModel<Match>> DeleteAsync(Match entity);

        /// <summary>
        /// Takes or renews the claim on a match for this judge. Fails if another
        /// judge holds a claim that has not yet lapsed.
        /// </summary>
        Task<ResultModel<Match>> ClaimAsync(Guid matchId, string judgeName);

        /// <summary>Gives the match back. Doing this twice is not an error.</summary>
        Task<ResultModel<Match>> ReleaseAsync(Guid matchId, string judgeName);

        /// <summary>
        /// Records a score. The judge must hold the claim (an unclaimed match is
        /// claimed automatically), and a successful write renews it. Finishing the
        /// match releases the claim.
        /// </summary>
        Task<ResultModel<Match>> SubmitScoreAsync(Guid matchId, int firstScore, int secondScore, bool finish, string judgeName);

        Task<ResultModel<Match>> ReopenAsync(Guid matchId);
    }
}
