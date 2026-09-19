using GildeApp.Mobile.Core.Models;
using GildeApp.Mobile.ViewModels;

namespace GildeApp.Mobile.Views
{
    public partial class MatchesPage : ContentPage
    {
        private readonly MatchesViewModel _viewModel;

        public MatchesPage(MatchesViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.LoadAsync();

            _viewModel.StartPolling(Dispatcher);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _viewModel.StopPolling();
        }

        private async void OnMatchSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is not MatchModel match)
                return;

            ((CollectionView)sender).SelectedItem = null;

            await _viewModel.OpenAsync(match);
        }
    }
}