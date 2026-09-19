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

            _viewModel.StartRenewing(Dispatcher);
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            await _viewModel.ReleaseAsync();
        }
    }
}