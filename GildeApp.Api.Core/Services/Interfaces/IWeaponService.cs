using System;
using System.Collections.Generic;
using System.Text;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Models;

namespace GildeApp.Api.Core.Services.Interfaces
{
    public interface IWeaponService
    {
        IQueryable<Weapon> GetAllWeapons();
        Task<ResultModel<IEnumerable<Weapon>>> ListAllAsync();
        Task<ResultModel<Weapon>> GetByIdAsync(Guid id);
        Task<bool> DoesWeaponIdExistsAsync(Guid id);
        Task<ResultModel<Weapon>> UpdateAsync(Weapon entity);
        Task<ResultModel<Weapon>> AddAsync(Weapon entity);
        Task<ResultModel<Weapon>> DeleteAsync(Weapon entity);
    }
}
