namespace Mvc.GildeApp.mvc.Models
{
    // Inherits TourneyModel because the API returns TourneyDetailDto : TourneyDto --
    // the JSON carries TourneyId, Name, Status, CreatedAt, WeaponName, PlayerCount,
    // MatchesTotal and MatchesFinished as well as the three properties below.
    public class TourneyDetailModel : TourneyModel
    {
        public Guid RuleSetId { get; set; }
        public RuleSetModel? RuleSet { get; set; }
        public List<TourneyEntryModel> Entries { get; set; } = new();
    }
}
