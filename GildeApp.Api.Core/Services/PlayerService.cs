using System;
using System.Collections.Generic;
using System.Text;
using GildeApp.Api.Core.Data;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;

namespace GildeApp.Api.Core.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly ApplicationDbContext _dbContext;

        public PlayerService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ResultModel<Player>> AddAsync(Player entity)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<Player>> DeleteAsync(Player entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DoesPlayerIdExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Player> GetAllMatches()
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<Player>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<IEnumerable<Player>>> ListAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<Player>> UpdateAsync(Player entity)
        {
            throw new NotImplementedException();
        }
    }
}
