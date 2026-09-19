using GildeApp.Mobile.Core.Models;
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

            ((CollectionView)sender).SelectedItem = null;

            await _viewModel.OpenAsync(tourney);
        }
    }
}