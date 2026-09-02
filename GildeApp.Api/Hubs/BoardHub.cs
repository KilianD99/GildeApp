using System;
using Microsoft.AspNetCore.SignalR;

namespace GildeApp.Api.Hubs
{
    /// <summary>
    /// Browsers watching a board join that tourney's group. When a judge submits a
    /// score the API pushes the recomputed board to that group only, so a busy club
    /// running three tourneys at once does not spray every update at every screen.
    /// </summary>
    public class BoardHub : Hub
    {
        public const string BoardUpdated = "BoardUpdated";

        public static string GroupFor(Guid tourneyId) => $"tourney-{tourneyId}";

        public Task JoinTourney(Guid tourneyId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, GroupFor(tourneyId));
        }

        public Task LeaveTourney(Guid tourneyId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupFor(tourneyId));
        }
    }
}
