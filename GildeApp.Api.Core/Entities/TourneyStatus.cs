namespace GildeApp.Api.Core.Entities
{
    public enum TourneyStatus
    {
        // Being put together: players can still be added and removed, no matches exist yet.
        Setup = 0,

        // Matches have been generated and are being fenced.
        Running = 1,

        // Every match is finished and the result is final.
        Finished = 2
    }
}
