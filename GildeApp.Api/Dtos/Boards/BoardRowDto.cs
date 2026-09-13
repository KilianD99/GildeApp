namespace GildeApp.Api.Dtos.Boards
{
    public class BoardRowDto
    {
        public Guid EntryId { get; set; }
        public Guid PlayerId { get; set; }
        public int Position { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalScore { get; set; }
        public int TotalReceived { get; set; }
        public int MatchesPlayed { get; set; }
        public int Placement { get; set; }
        public List<BoardCellDto> Cells { get; set; } = new();
    }
}
