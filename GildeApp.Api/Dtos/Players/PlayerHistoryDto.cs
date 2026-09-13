namespace GildeApp.Api.Dtos.Players
{
    public class PlayerHistoryDto
    {
        public Guid TourneyId { get; set; }
        public string TourneyName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Position { get; set; }
    }
}
