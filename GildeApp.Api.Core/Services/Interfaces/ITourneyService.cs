using System;
using System.Collections.Generic;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Models;

namespace GildeApp.Api.Core.Services.Interfaces
{
    public interface ITourneyService
    {
        IQueryable<Tourney> GetAllTourneys();

        /// <summary>Tourneys with entry counts and weapon, for the list page.</summary>
        Task<ResultModel<IEnumerable<Tourney>>> ListAllAsync();

        Task<ResultModel<Tourney>> GetByIdAsync(Guid id);

        /// <summary>
        /// The whole tourney in one query: entries with players, and matches with both
        /// entries. This is what the board is built from.
        /// </summary>
        Task<ResultModel<Tourney>> GetFullAsync(Guid id);

        Task<bool> DoesTourneyIdExistsAsync(Guid id);

        Task<ResultModel<Tourney>> AddAsync(Tourney entity);
        Task<ResultModel<Tourney>> UpdateAsync(Tourney entity);
        Task<ResultModel<Tourney>> DeleteAsync(Tourney entity);

        /// <summary>Enters a player, giving them the next free position number.</summary>
        Task<ResultModel<TourneyEntry>> AddEntryAsync(Guid tourneyId, Guid playerId);

        /// <summary>Removes an entry and closes the gap in the position numbers.</summary>
        Task<ResultModel<TourneyEntry>> RemoveEntryAsync(Guid tourneyId, Guid entryId);

        /// <summary>
        /// Builds every match for the round robin and moves the tourney to Running.
        /// Fails if it is not in Setup or has fewer than two players.
        /// </summary>
        Task<ResultModel<Tourney>> GenerateMatchesAsync(Guid tourneyId);

        /// <summary>Marks the tourney Finished. Fails if any match is still unfinished.</summary>
        Task<ResultModel<Tourney>> FinishAsync(Guid tourneyId);
    }
}
