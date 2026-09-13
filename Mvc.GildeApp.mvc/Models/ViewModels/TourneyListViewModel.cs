namespace Mvc.GildeApp.mvc.Models.ViewModels
{
    public class TourneyListViewModel
    {
        public List<TourneyModel> Running { get; set; } = new();
        public List<TourneyModel> Setup { get; set; } = new();
        public List<TourneyModel> Finished { get; set; } = new();

        public bool IsEmpty => Running.Count == 0 && Setup.Count == 0 && Finished.Count == 0;
    }
}
