using System;
using System.Collections.Generic;
using System.Text;
using GildeApp.Api.Core.Data;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;

namespace GildeApp.Api.Core.Services
{
    public class WeaponService : IWeaponService
    {
        private readonly ApplicationDbContext _dbContext;

        public WeaponService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ResultModel<Weapon>> AddAsync(Weapon entity)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<Weapon>> DeleteAsync(Weapon entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DoesWeaponIdExistsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Weapon> GetAllMatches()
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<Weapon>> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<IEnumerable<Weapon>>> ListAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel<Weapon>> UpdateAsync(Weapon entity)
        {
            throw new NotImplementedException();
        }
    }
}
