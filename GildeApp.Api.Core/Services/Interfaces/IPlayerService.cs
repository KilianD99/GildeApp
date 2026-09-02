using System;
using System.Collections.Generic;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Models;

namespace GildeApp.Api.Core.Services.Interfaces
{
    public interface IPlayerService
    {
        IQueryable<Player> GetAllPlayers();
        Task<ResultModel<IEnumerable<Player>>> ListAllAsync();
        Task<ResultModel<IEnumerable<Player>>> SearchAsync(string? term);
        Task<ResultModel<Player>> GetByIdAsync(Guid id);
        Task<bool> DoesPlayerIdExistsAsync(Guid id);
        Task<ResultModel<Player>> UpdateAsync(Player entity);
        Task<ResultModel<Player>> AddAsync(Player entity);
        Task<ResultModel<Player>> DeleteAsync(Player entity);
    }
}
