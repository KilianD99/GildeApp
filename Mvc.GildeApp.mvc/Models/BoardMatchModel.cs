namespace Mvc.GildeApp.mvc.Models
{
    public class BoardMatchModel
    {
        public Guid MatchId { get; set; }
        public int Order { get; set; }
        public string Status { get; set; } = string.Empty;
        public int FirstPosition { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public int FirstScore { get; set; }
        public int SecondPosition { get; set; }
        public string SecondName { get; set; } = string.Empty;
        public int SecondScore { get; set; }
        public DateTime? UpdatedAt { get; set; }

        /// <summary>Judge currently scoring this match on the mobile app.</summary>
        public string? ClaimedBy { get; set; }
    }
}
