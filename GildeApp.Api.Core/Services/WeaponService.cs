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
    public class WeaponService : IWeaponService
    {
        private readonly ApplicationDbContext _dbContext;

        public WeaponService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultModel<Weapon>> AddAsync(Weapon entity)
        {
            var resultModel = new ResultModel<Weapon>();
            _dbContext.Weapons.Add(entity);
            await _dbContext.SaveChangesAsync();

            resultModel = new ResultModel<Weapon> { Data = entity };

            return resultModel;
        }

        public async Task<ResultModel<Weapon>> DeleteAsync(Weapon entity)
        {
            var resultModel = new ResultModel<Weapon>();

            _dbContext.Weapons.Remove(entity);
            await _dbContext.SaveChangesAsync();

            resultModel.Data = entity;

            return resultModel;
        }

        public async Task<bool> DoesWeaponIdExistsAsync(Guid id)
        {
            bool doesWeaponExists = await _dbContext.Weapons
                 .AnyAsync(b => b.Id.Equals(id));

            return doesWeaponExists;
        }

        public IQueryable<Weapon> GetAllWeapons()
        {
            return _dbContext.Weapons.AsQueryable();
        }

        public async Task<ResultModel<Weapon>> GetByIdAsync(Guid id)
        {
            var resultModel = new ResultModel<Weapon>();
            var weapon = await _dbContext.Weapons
                .Include(w => w.RuleSets)
                .FirstOrDefaultAsync(a => a.Id.Equals(id));


            if (weapon is null)
            {
                resultModel = new ResultModel<Weapon>();
                resultModel.Errors.Add($"Weapon does not exists");

                return resultModel;
            }

            resultModel = new ResultModel<Weapon> { Data = weapon };

            return resultModel;
        }

        public async Task<ResultModel<IEnumerable<Weapon>>> ListAllAsync()
        {
            var weapons = await _dbContext.Weapons.ToListAsync();
            var resultModel = new ResultModel<IEnumerable<Weapon>>
            {
                Data = weapons
            };

            return resultModel;
        }

        public async Task<ResultModel<Weapon>> UpdateAsync(Weapon entity)
        {
            var resultModel = new ResultModel<Weapon>();

            if (await DoesWeaponIdExistsAsync(entity.Id) == false)
            {
                resultModel.Errors.Add($"There is no Weapon with the ID {entity.Id}");

                return resultModel;
            }


            _dbContext.Weapons.Update(entity);
            await _dbContext.SaveChangesAsync();

            resultModel = new ResultModel<Weapon> { Data = entity };
            return resultModel;
        }
    }
}
