namespace GildeApp.Mobile.Models
{
    /// <summary>The API wraps every payload in { data, errors, isSuccess }.</summary>
    public class ApiResult<T>
    {
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public bool IsSuccess { get; set; }
    }

    public class TourneyModel
    {
        public Guid TourneyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string WeaponName { get; set; } = string.Empty;
        public int PlayerCount { get; set; }
        public int MatchesTotal { get; set; }
        public int MatchesFinished { get; set; }

        public string Subtitle =>
            string.IsNullOrEmpty(WeaponName)
                ? $"{MatchesFinished} of {MatchesTotal} matches"
                : $"{WeaponName} · {MatchesFinished} of {MatchesTotal} matches";
    }

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

        // ---- display helpers, used straight from the XAML ----

        public bool IsFinished => Status == "Finished";

        public string Title => $"{FirstPosition}. {FirstName}  vs  {SecondPosition}. {SecondName}";

        public string ScoreLine => $"{FirstScore} – {SecondScore}";

        public string StateLine => IsFinished
            ? "Finished"
            : IsClaimed
                ? $"{ClaimedBy} is scoring this"
                : Status == "InProgress" ? "In progress" : "Not started";

        /// <summary>Green when free, amber when someone holds it, grey when done.</summary>
        public Color StateColor => IsFinished
            ? Color.FromArgb("#5A636E")
            : IsClaimed
                ? Color.FromArgb("#9A6408")
                : Color.FromArgb("#0E7A4F");
    }

    public class MatchDetailModel : MatchModel
    {
        public string TourneyName { get; set; } = string.Empty;
        public int MaxScore { get; set; }
    }
}
