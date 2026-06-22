using System;
using System.Collections.Generic;
using System.Text;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Models;

namespace GildeApp.Api.Core.Services.Interfaces
{
    public interface IMatchService
    {
        IQueryable<Match> GetAllMatches();
        Task<ResultModel<IEnumerable<Match>>> ListAllAsync();
        Task<ResultModel<Match>> GetByIdAsync(int id);
        Task<bool> DoesMatchIdExistsAsync(int id);
        Task<ResultModel<Match>> UpdateAsync(Match entity);
        Task<ResultModel<Match>> AddAsync(Match entity);
        Task<ResultModel<Match>> DeleteAsync(Match entity);
    }
}
