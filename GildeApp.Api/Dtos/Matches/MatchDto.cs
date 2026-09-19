namespace GildeApp.Api.Dtos.Matches
{
    public class MatchDto
    {
        public Guid MatchId { get; set; }
        public Guid TourneyId { get; set; }
        public int Order { get; set; }
        public string Status { get; set; } = string.Empty;

        public Guid FirstEntryId { get; set; }
        public int FirstPosition { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public int FirstScore { get; set; }

        public Guid SecondEntryId { get; set; }
        public int SecondPosition { get; set; }
        public string SecondName { get; set; } = string.Empty;
        public int SecondScore { get; set; }

        public DateTime? UpdatedAt { get; set; }

        /// <summary>Judge currently scoring this match, or null if it is free.</summary>
        public string? ClaimedBy { get; set; }

        /// <summary>
        /// Whether the claim was still live when this response was built. Sent as a
        /// plain flag so clients never have to compare their own clock to the
        /// server's — phones drift.
        /// </summary>
        public bool IsClaimed { get; set; }
    }
}
