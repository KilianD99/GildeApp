namespace Mvc.GildeApp.mvc.Models
{
    public class BoardRowModel
    {
        public Guid EntryId { get; set; }
        public Guid PlayerId { get; set; }
        public int Position { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalScore { get; set; }
        public int TotalReceived { get; set; }
        public int MatchesPlayed { get; set; }
        public int Placement { get; set; }
        public List<BoardCellModel> Cells { get; set; } = new();
    }
}
