using System;
using System.Collections.Generic;
using System.Text;
using GildeApp.Api.Core.Data;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace GildeApp.Api.Core.Services
{
    public class RuleSetService : IRuleSetService
    {
        private readonly ApplicationDbContext _dbContext;

        public RuleSetService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultModel<RuleSet>> AddAsync(RuleSet entity)
        {
            var resultModel = new ResultModel<RuleSet>();
            _dbContext.RuleSets.Add(entity);
            await _dbContext.SaveChangesAsync();

            resultModel = new ResultModel<RuleSet> { Data = entity };

            return resultModel;
        }

        public async Task<ResultModel<RuleSet>> DeleteAsync(RuleSet entity)
        {
            var resultModel = new ResultModel<RuleSet>();

            _dbContext.RuleSets.Remove(entity);
            await _dbContext.SaveChangesAsync();

            resultModel.Data = entity;

            return resultModel;
        }

        public async Task<bool> DoesRuleSetIdExistsAsync(Guid id)
        {
            bool doesRuleSetExist = await _dbContext.RuleSets
                 .AnyAsync(b => b.Id.Equals(id));

            return doesRuleSetExist;
        }

        public IQueryable<RuleSet> GetAllRuleSets()
        {
            return _dbContext.RuleSets.AsQueryable();
        }

        public async Task<ResultModel<RuleSet>> GetByIdAsync(Guid id)
        {
            var resultModel = new ResultModel<RuleSet>();
            var ruleSet = await _dbContext.RuleSets
                .FirstOrDefaultAsync(a => a.Id.Equals(id));


            if (ruleSet is null)
            {
                resultModel = new ResultModel<RuleSet>();
                resultModel.Errors.Add($"RuleSet does not exists");

                return resultModel;
            }

            resultModel = new ResultModel<RuleSet> { Data = ruleSet };

            return resultModel;
        }

        public async Task<ResultModel<IEnumerable<RuleSet>>> ListAllAsync()
        {
            var ruleSets = await _dbContext.RuleSets.ToListAsync();
            var resultModel = new ResultModel<IEnumerable<RuleSet>>
            {
                Data = ruleSets
            };

            return resultModel;
        }

        public async Task<ResultModel<RuleSet>> UpdateAsync(RuleSet entity)
        {
            var resultModel = new ResultModel<RuleSet>();

            if (await DoesRuleSetIdExistsAsync(entity.Id) == false)
            {
                resultModel.Errors.Add($"There is no RuleSet with the ID {entity.Id}");

                return resultModel;
            }


            _dbContext.RuleSets.Update(entity);
            await _dbContext.SaveChangesAsync();

            resultModel = new ResultModel<RuleSet> { Data = entity };
            return resultModel;
        }
    }
}
