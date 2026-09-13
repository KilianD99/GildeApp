namespace Mvc.GildeApp.mvc.Models
{
    public class TourneyDetailModel
    {
        public Guid RuleSetId { get; set; }
        public RuleSetModel? RuleSet { get; set; }
        public List<TourneyEntryModel> Entries { get; set; } = new();
    }
}
