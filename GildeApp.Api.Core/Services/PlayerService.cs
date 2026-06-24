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
    public class PlayerService : IPlayerService
    {
        private readonly ApplicationDbContext _dbContext;

        public PlayerService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultModel<Player>> AddAsync(Player entity)
        {
            var resultModel = new ResultModel<Player>();
            _dbContext.Players.Add(entity);
            await _dbContext.SaveChangesAsync();

            resultModel = new ResultModel<Player> { Data = entity };

            return resultModel;
        }

        public async Task<ResultModel<Player>> DeleteAsync(Player entity)
        {
            var resultModel = new ResultModel<Player>();

            _dbContext.Players.Remove(entity);
            await _dbContext.SaveChangesAsync();

            resultModel.Data = entity;

            return resultModel;
        }

        public async Task<bool> DoesPlayerIdExistsAsync(Guid id)
        {
            bool doesPlayerExist = await _dbContext.Players
                 .AnyAsync(b => b.Id.Equals(id));

            return doesPlayerExist;
        }

        public IQueryable<Player> GetAllPlayers()
        {
            return _dbContext.Players.AsQueryable();
        }

        public async Task<ResultModel<Player>> GetByIdAsync(Guid id)
        {
            var resultModel = new ResultModel<Player>();
            var player = await _dbContext.Players
                .FirstOrDefaultAsync(a => a.Id.Equals(id));


            if (player is null)
            {
                resultModel = new ResultModel<Player>();
                resultModel.Errors.Add($"Player does not exists");

                return resultModel;
            }

            resultModel = new ResultModel<Player> { Data = player };

            return resultModel;
        }

        public async Task<ResultModel<IEnumerable<Player>>> ListAllAsync()
        {
            var players = await _dbContext.Players.ToListAsync();
            var resultModel = new ResultModel<IEnumerable<Player>>
            {
                Data = players
            };

            return resultModel;
        }

        public async Task<ResultModel<Player>> UpdateAsync(Player entity)
        {
            var resultModel = new ResultModel<Player>();

            if (await DoesPlayerIdExistsAsync(entity.Id) == false)
            {
                resultModel.Errors.Add($"There is no Player with the ID {entity.Id}");

                return resultModel;
            }


            _dbContext.Players.Update(entity);
            await _dbContext.SaveChangesAsync();

            resultModel = new ResultModel<Player> { Data = entity };
            return resultModel;
        }
    }
}
