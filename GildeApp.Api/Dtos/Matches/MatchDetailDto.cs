namespace GildeApp.Api.Dtos.Matches
{
    public class MatchDetailDto : MatchDto
    {
        public string TourneyName { get; set; } = string.Empty;
        public int MaxScore { get; set; }
    }
}
