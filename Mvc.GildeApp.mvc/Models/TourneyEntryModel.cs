namespace Mvc.GildeApp.mvc.Models
{
    public class TourneyEntryModel
    {
        public Guid EntryId { get; set; }
        public Guid TourneyId { get; set; }
        public Guid PlayerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Position { get; set; }
    }
}
