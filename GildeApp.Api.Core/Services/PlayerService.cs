using System;
using System.Collections.Generic;
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

        public IQueryable<Player> GetAllPlayers()
        {
            return _dbContext.Players.AsQueryable();
        }

        public async Task<ResultModel<IEnumerable<Player>>> ListAllAsync()
        {
            var players = await _dbContext.Players
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();

            return new ResultModel<IEnumerable<Player>> { Data = players };
        }

        public async Task<ResultModel<IEnumerable<Player>>> SearchAsync(string? term)
        {
            var query = _dbContext.Players.AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
            {
                var needle = term.Trim();
                query = query.Where(p =>
                    EF.Functions.Like(p.FirstName, $"%{needle}%") ||
                    EF.Functions.Like(p.LastName, $"%{needle}%"));
            }

            var players = await query
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Take(50)
                .ToListAsync();

            return new ResultModel<IEnumerable<Player>> { Data = players };
        }

        public async Task<ResultModel<Player>> GetByIdAsync(Guid id)
        {
            var resultModel = new ResultModel<Player>();

            var player = await _dbContext.Players
                .Include(p => p.Entries)
                    .ThenInclude(e => e.Tourney)
                .FirstOrDefaultAsync(p => p.Id.Equals(id));

            if (player is null)
            {
                resultModel.Errors.Add("Player does not exist");
                return resultModel;
            }

            resultModel.Data = player;
            return resultModel;
        }

        public async Task<bool> DoesPlayerIdExistsAsync(Guid id)
        {
            return await _dbContext.Players.AnyAsync(p => p.Id.Equals(id));
        }

        public async Task<ResultModel<Player>> AddAsync(Player entity)
        {
            _dbContext.Players.Add(entity);
            await _dbContext.SaveChangesAsync();

            return new ResultModel<Player> { Data = entity };
        }

        public async Task<ResultModel<Player>> UpdateAsync(Player entity)
        {
            var resultModel = new ResultModel<Player>();

            if (await DoesPlayerIdExistsAsync(entity.Id) == false)
            {
                resultModel.Errors.Add($"There is no player with the ID {entity.Id}");
                return resultModel;
            }

            _dbContext.Players.Update(entity);
            await _dbContext.SaveChangesAsync();

            resultModel.Data = entity;
            return resultModel;
        }

        public async Task<ResultModel<Player>> DeleteAsync(Player entity)
        {
            var resultModel = new ResultModel<Player>();

            var isEntered = await _dbContext.TourneyEntries
                .AnyAsync(e => e.PlayerId == entity.Id);

            if (isEntered)
            {
                resultModel.Errors.Add(
                    "This player is entered in a tourney and cannot be deleted. Remove them from the tourney first.");
                return resultModel;
            }

            _dbContext.Players.Remove(entity);
            await _dbContext.SaveChangesAsync();

            resultModel.Data = entity;
            return resultModel;
        }
    }
}
