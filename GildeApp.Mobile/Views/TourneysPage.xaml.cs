using GildeApp.Mobile.Models;
using GildeApp.Mobile.Services;
using GildeApp.Mobile.ViewModels;

namespace GildeApp.Mobile.Views
{
    public partial class TourneysPage : ContentPage
    {
        private readonly TourneysViewModel _viewModel;
        private readonly JudgeIdentity _judge;

        public TourneysPage(TourneysViewModel viewModel, JudgeIdentity judge)
        {
            InitializeComponent();

            _viewModel = viewModel;
            _judge = judge;
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // First run: there is no judge name yet, so ask for one before anything
            // else. Coming back from that page lands here again with a name set.
            if (!_judge.IsSet)
            {
                await Shell.Current.GoToAsync(Routes.JudgeSetup);
                return;
            }

            _viewModel.RefreshJudgeLine();
            await _viewModel.LoadAsync();
        }

        private async void OnTourneySelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is not TourneyModel tourney)
                return;

            // Clear it straight away, or coming back to this page leaves the row
            // highlighted and a second tap on it does nothing.
            ((CollectionView)sender).SelectedItem = null;

            await _viewModel.OpenAsync(tourney);
        }
    }
}
