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

        /// <summary>
        /// What the judge's mobile app calls. Rejects a score for a match that is already
        /// finished, so two judges on the same bout cannot silently overwrite each other.
        /// </summary>
        Task<ResultModel<Match>> SubmitScoreAsync(Guid matchId, int firstScore, int secondScore, bool finish);

        /// <summary>Puts a finished match back in progress so a mistake can be corrected.</summary>
        Task<ResultModel<Match>> ReopenAsync(Guid matchId);

        Task<ResultModel<Match>> DeleteAsync(Match entity);
    }
}
