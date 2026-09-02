using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mvc.GildeApp.mvc.Models
{
    public class TourneyListViewModel
    {
        public List<TourneyModel> Running { get; set; } = new();
        public List<TourneyModel> Setup { get; set; } = new();
        public List<TourneyModel> Finished { get; set; } = new();

        public bool IsEmpty => Running.Count == 0 && Setup.Count == 0 && Finished.Count == 0;
    }

    public class CreateTourneyViewModel
    {
        [Required(ErrorMessage = "Give the tourney a name")]
        [MaxLength(200)]
        [Display(Name = "Tourney name")]
        public string Name { get; set; } = string.Empty;

        // Nullable on purpose: [Required] on a plain Guid never fails, because
        // Guid.Empty is not null, so the "pick one" message would never show.
        [Required(ErrorMessage = "Pick a rule set")]
        [Display(Name = "Rule set")]
        public Guid? RuleSetId { get; set; }

        public List<RuleSetModel> AvailableRuleSets { get; set; } = new();
    }

    public class EntriesViewModel
    {
        public TourneyDetailModel Tourney { get; set; } = new();
        public List<PlayerModel> AvailablePlayers { get; set; } = new();

        [Display(Name = "First name")]
        public string NewFirstName { get; set; } = string.Empty;

        [Display(Name = "Last name")]
        public string NewLastName { get; set; } = string.Empty;

        public bool CanStart => Tourney.Entries.Count >= 2 && Tourney.Status == "Setup";
    }
}
