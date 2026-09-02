namespace GildeApp.Api.Core.Entities
{
    public enum MatchStatus
    {
        // Generated, but nobody has started it. Scores are still 0-0.
        Scheduled = 0,

        // A judge has opened it on the mobile app and is entering scores.
        InProgress = 1,

        // Submitted. Scores no longer change unless it is explicitly reopened.
        Finished = 2
    }
}
