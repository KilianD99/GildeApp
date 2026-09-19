using System;
using System.Collections.Generic;
using System.Text;

namespace GildeApp.Mobile.Core.Models
{
    public class MatchModel
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

        public string? ClaimedBy { get; set; }
        public bool IsClaimed { get; set; }

        public bool IsFinished => Status == "Finished";

        public string Title => $"{FirstPosition}. {FirstName}  vs  {SecondPosition}. {SecondName}";

        public string ScoreLine => $"{FirstScore} – {SecondScore}";

        public string StateLine => IsFinished
            ? "Finished"
            : IsClaimed
                ? $"{ClaimedBy} is scoring this"
                : Status == "InProgress" ? "In progress" : "Not started";

        public Color StateColor => IsFinished
            ? Color.FromArgb("#5A636E")
            : IsClaimed
                ? Color.FromArgb("#9A6408")
                : Color.FromArgb("#0E7A4F");
    }
}
