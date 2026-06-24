using System;
using System.Collections.Generic;
using System.Text;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Models;

namespace GildeApp.Api.Core.Services.Interfaces
{
    public interface IRuleSetService
    {
        IQueryable<RuleSet> GetAllMatches();
        Task<ResultModel<IEnumerable<RuleSet>>> ListAllAsync();
        Task<ResultModel<RuleSet>> GetByIdAsync(Guid id);
        Task<bool> DoesRuleSetIdExistsAsync(Guid id);
        Task<ResultModel<RuleSet>> UpdateAsync(RuleSet entity);
        Task<ResultModel<RuleSet>> AddAsync(RuleSet entity);
        Task<ResultModel<RuleSet>> DeleteAsync(RuleSet entity);
    }
}
