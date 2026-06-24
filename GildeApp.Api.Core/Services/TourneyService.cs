using System;
using System.Collections.Generic;
using System.Text;
using GildeApp.Api.Core.Data;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;

namespace GildeApp.Api.Core.Services
{
    public class TourneyService : ITourneyService
    {
        private readonly ApplicationDbContext _dbContext;

        public TourneyService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ResultModel<Tourney>> AddAsync(Tourney entity)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<Tourney>> DeleteAsync(Tourney entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DoesTourneyIdExistsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Tourney> GetAllMatches()
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<Tourney>> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<IEnumerable<Tourney>>> ListAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<Tourney>> UpdateAsync(Tourney entity)
        {
            throw new NotImplementedException();
        }
    }
}
