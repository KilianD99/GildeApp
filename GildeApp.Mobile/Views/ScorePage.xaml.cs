using GildeApp.Mobile.ViewModels;

namespace GildeApp.Mobile.Views
{
    public partial class ScorePage : ContentPage
    {
        private readonly ScoreViewModel _viewModel;

        public ScorePage(ScoreViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.LoadAsync();

            // The claim was taken on the match list. Keep renewing it while this
            // screen is open so a bout with long pauses never loses its lock.
            _viewModel.StartRenewing(Dispatcher);
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            // Leaving the screen hands the match straight back, so the next judge
            // does not have to wait out the expiry.
            await _viewModel.ReleaseAsync();
        }
    }
}
