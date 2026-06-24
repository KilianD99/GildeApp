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
    public class MatchService : IMatchService
    {
        private readonly ApplicationDbContext _dbContext;

        public MatchService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultModel<Match>> AddAsync(Match entity)
        {
            var resultModel = new ResultModel<Match>();            
            _dbContext.Matches.Add(entity);
            await _dbContext.SaveChangesAsync();

            resultModel = new ResultModel<Match> { Data = entity };

            return resultModel;
        }

        public async Task<ResultModel<Match>> DeleteAsync(Match entity)
        {
            var resultModel = new ResultModel<Match>();

            _dbContext.Matches.Remove(entity);
            await _dbContext.SaveChangesAsync();

            resultModel.Data = entity;

            return resultModel;
        }

        public async Task<bool> DoesMatchIdExistsAsync(Guid id)
        {
            bool doesMatchExist = await _dbContext.Matches
                 .AnyAsync(b => b.Id.Equals(id));

            return doesMatchExist;
        }

        public IQueryable<Match> GetAllMatches()
        {
            return _dbContext.Matches.AsQueryable();
        }

        public async Task<ResultModel<Match>> GetByIdAsync(Guid id)
        {
            var resultModel = new ResultModel<Match>();
            var match = await _dbContext.Matches
                .FirstOrDefaultAsync(a => a.Id.Equals(id));


            if (match is null)
            {
                resultModel = new ResultModel<Match>();
                resultModel.Errors.Add($"Match does not exists");

                return resultModel;
            }

            resultModel = new ResultModel<Match> { Data = match };

            return resultModel;
        }

        public async Task<ResultModel<IEnumerable<Match>>> ListAllAsync()
        {
            var matches = await _dbContext.Matches.ToListAsync();
            var resultModel = new ResultModel<IEnumerable<Match>>
            {
                Data = matches
            };

            return resultModel;
        }

        public async Task<ResultModel<Match>> UpdateAsync(Match entity)
        {
            var resultModel = new ResultModel<Match>();

            if (await DoesMatchIdExistsAsync(entity.Id) == false)
            {
                resultModel.Errors.Add($"There is no Match with the ID {entity.Id}");

                return resultModel;
            }


            _dbContext.Matches.Update(entity);
            await _dbContext.SaveChangesAsync();

            resultModel = new ResultModel<Match> { Data = entity };
            return resultModel;
        }
    }
}
