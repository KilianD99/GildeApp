using System.ComponentModel.DataAnnotations;

namespace Mvc.GildeApp.mvc.Models.ViewModels
{
    public class CreateTourneyViewModel
    {
        [Required(ErrorMessage = "Give the tourney a name")]
        [MaxLength(200)]
        [Display(Name = "Tourney name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pick a rule set")]
        [Display(Name = "Rule set")]
        public Guid? RuleSetId { get; set; }

        public List<RuleSetModel> AvailableRuleSets { get; set; } = new();
    }
}
