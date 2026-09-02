using System;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace GildeApp.Api.Hubs
{
    /// <summary>
    /// Rebuilds a tourney's board and pushes it to everyone watching. Kept in one
    /// place so every path that changes a score broadcasts the same payload the
    /// board endpoint would have returned.
    /// </summary>
    public class BoardBroadcaster
    {
        private readonly IHubContext<BoardHub> _hub;
        private readonly ITourneyService _tourneyService;

        public BoardBroadcaster(IHubContext<BoardHub> hub, ITourneyService tourneyService)
        {
            _hub = hub;
            _tourneyService = tourneyService;
        }

        public async Task BroadcastAsync(Guid tourneyId)
        {
            var result = await _tourneyService.GetFullAsync(tourneyId);

            if (!result.IsSuccess)
                return;

            var board = result.Data.ToBoardDto();

            await _hub.Clients
                .Group(BoardHub.GroupFor(tourneyId))
                .SendAsync(BoardHub.BoardUpdated, board);
        }
    }
}
