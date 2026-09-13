using System;
using System.Collections.Generic;
using System.Text;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Models;

namespace GildeApp.Api.Core.Services.Interfaces
{
    public interface ITourneyService
    {
        IQueryable<Tourney> GetAllTourneys();
        Task<ResultModel<IEnumerable<Tourney>>> ListAllAsync();
        Task<ResultModel<Tourney>> GetByIdAsync(Guid id);
        Task<ResultModel<Tourney>> GetFullAsync(Guid id);
        Task<bool> DoesTourneyIdExistsAsync(Guid id);
        Task<ResultModel<Tourney>> UpdateAsync(Tourney entity);
        Task<ResultModel<Tourney>> AddAsync(Tourney entity);
        Task<ResultModel<Tourney>> DeleteAsync(Tourney entity);
        Task<ResultModel<TourneyEntry>> AddEntryAsync(Guid tourneyId, Guid playerId);
        Task<ResultModel<TourneyEntry>> RemoveEntryAsync(Guid tourneyId, Guid entryId);
        Task<ResultModel<Tourney>> GenerateMatchesAsync(Guid tourneyId);
        Task<ResultModel<Tourney>> FinishAsync(Guid tourneyId);
    }
}
