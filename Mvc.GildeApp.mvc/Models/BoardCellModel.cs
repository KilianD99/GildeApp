namespace Mvc.GildeApp.mvc.Models
{
    public class BoardCellModel
    {
        public int OpponentPosition { get; set; }
        public bool IsSelf { get; set; }
        public Guid? MatchId { get; set; }
        public int? Score { get; set; }
        public int? OpponentScore { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
