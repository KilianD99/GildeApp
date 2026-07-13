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
    public class TourneyService : ITourneyService
    {
        private readonly ApplicationDbContext _dbContext;

        public TourneyService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultModel<Tourney>> AddAsync(Tourney entity)
        {
            var resultModel = new ResultModel<Tourney>();
            _dbContext.Tourneys.Add(entity);
            await _dbContext.SaveChangesAsync();

            resultModel = new ResultModel<Tourney> { Data = entity };

            return resultModel;
        }

        public async Task<ResultModel<Tourney>> DeleteAsync(Tourney entity)
        {
            var resultModel = new ResultModel<Tourney>();

            _dbContext.Tourneys.Remove(entity);
            await _dbContext.SaveChangesAsync();

            resultModel.Data = entity;

            return resultModel;
        }

        public async Task<bool> DoesTourneyIdExistsAsync(Guid id)
        {
            bool doesTourneyExists = await _dbContext.Tourneys
                 .AnyAsync(b => b.Id.Equals(id));

            return doesTourneyExists;
        }

        public IQueryable<Tourney> GetAllTourneys()
        {
            return _dbContext.Tourneys
                .Include(s => s.Matches)
                .Include(s => s.RuleSet)
                .Include(s => s.Players)
                .AsQueryable();
        }

        public async Task<ResultModel<Tourney>> GetByIdAsync(Guid id)
        {
            var resultModel = new ResultModel<Tourney>();
            var tourney = await _dbContext.Tourneys
                .Include(s => s.Matches)
                .Include(s => s.RuleSet)
                .ThenInclude(d => d.Weapon)
                .Include(s => s.Players)
                .FirstOrDefaultAsync(a => a.Id.Equals(id));


            if (tourney is null)
            {
                resultModel = new ResultModel<Tourney>();
                resultModel.Errors.Add($"Tourney does not exists");

                return resultModel;
            }

            resultModel = new ResultModel<Tourney> { Data = tourney };

            return resultModel;
        }

        public async Task<ResultModel<IEnumerable<Tourney>>> ListAllAsync()
        {
            var tourneys = await _dbContext.Tourneys
                .Include(s => s.Matches)
                .Include(s => s.RuleSet)
                .Include(s => s.Players)
                .ToListAsync();
            var resultModel = new ResultModel<IEnumerable<Tourney>>
            {
                Data = tourneys
            };

            return resultModel;
        }

        public async Task<ResultModel<Tourney>> UpdateAsync(Tourney entity)
        {
            var resultModel = new ResultModel<Tourney>();

            if (await DoesTourneyIdExistsAsync(entity.Id) == false)
            {
                resultModel.Errors.Add($"There is no Tourney with the ID {entity.Id}");

                return resultModel;
            }


            _dbContext.Tourneys.Update(entity);
            await _dbContext.SaveChangesAsync();

            resultModel = new ResultModel<Tourney> { Data = entity };
            return resultModel;
        }
    }
}
