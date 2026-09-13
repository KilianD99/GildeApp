using Microsoft.AspNetCore.SignalR;

namespace GildeApp.Api.Hubs
{
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
