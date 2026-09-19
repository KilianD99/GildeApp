using GildeApp.Mobile.Models;
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

            // Claims come and go on other people's phones, so keep checking while
            // this list is on screen.
            _viewModel.StartPolling(Dispatcher);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Nothing to poll for once the page is gone; leaving the timer running
            // would keep hitting the API from a page nobody is looking at.
            _viewModel.StopPolling();
        }

        private async void OnMatchSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is not MatchModel match)
                return;

            // Clear it straight away, or coming back from scoring leaves the row
            // highlighted and a second tap on it does nothing.
            ((CollectionView)sender).SelectedItem = null;

            await _viewModel.OpenAsync(match);
        }
    }
}
