using System;
using System.Collections.Generic;
using System.Text;
using GildeApp.Api.Core.Data;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;

namespace GildeApp.Api.Core.Services
{
    public class RuleSetService : IRuleSetService
    {
        private readonly ApplicationDbContext _dbContext;

        public RuleSetService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ResultModel<RuleSet>> AddAsync(RuleSet entity)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<RuleSet>> DeleteAsync(RuleSet entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DoesRuleSetIdExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<RuleSet> GetAllMatches()
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<RuleSet>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<IEnumerable<RuleSet>>> ListAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<RuleSet>> UpdateAsync(RuleSet entity)
        {
            throw new NotImplementedException();
        }
    }
}
