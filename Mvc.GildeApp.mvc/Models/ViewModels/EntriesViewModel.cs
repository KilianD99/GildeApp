using System.ComponentModel.DataAnnotations;

namespace Mvc.GildeApp.mvc.Models.ViewModels
{
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
