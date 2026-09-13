namespace GildeApp.Api.Dtos.Boards
{
    public class BoardDto
    {
        public Guid TourneyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string WeaponName { get; set; } = string.Empty;
        public int MaxScore { get; set; }

        public int PlayerCount { get; set; }
        public int MatchesTotal { get; set; }
        public int MatchesFinished { get; set; }
        public List<BoardRowDto> Rows { get; set; } = new();
        public List<BoardMatchDto> Matches { get; set; } = new();
    }
}
