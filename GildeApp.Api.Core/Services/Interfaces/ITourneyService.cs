using System;
using System.Collections.Generic;
using System.Text;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Models;

namespace GildeApp.Api.Core.Services.Interfaces
{
    public interface ITourneyService
    {
        IQueryable<Tourney> GetAllMatches();
        Task<ResultModel<IEnumerable<Tourney>>> ListAllAsync();
        Task<ResultModel<Tourney>> GetByIdAsync(Guid id);
        Task<bool> DoesTourneyIdExistsAsync(Guid id);
        Task<ResultModel<Tourney>> UpdateAsync(Tourney entity);
        Task<ResultModel<Tourney>> AddAsync(Tourney entity);
        Task<ResultModel<Tourney>> DeleteAsync(Tourney entity);
    }
}
