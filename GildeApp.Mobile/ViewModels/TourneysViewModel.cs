using System.Collections.ObjectModel;
using System.Windows.Input;
using GildeApp.Mobile.Models;
using GildeApp.Mobile.Services;

namespace GildeApp.Mobile.ViewModels
{
    public class TourneysViewModel : BaseViewModel
    {
        private readonly IGildeApi _api;
        private readonly JudgeIdentity _judge;

        // Separate from IsBusy on purpose. RefreshView.IsRefreshing is a two-way
        // binding, so a pull-to-refresh sets IsBusy = true before it fires the
        // command -- guarding on IsBusy would make LoadAsync return immediately and
        // leave the spinner turning forever.
        private bool _loading;

        public TourneysViewModel(IGildeApi api, JudgeIdentity judge)
        {
            _api = api;
            _judge = judge;

            RefreshCommand = new Command(async () => await LoadAsync());
            ChangeJudgeCommand = new Command(async () => await Shell.Current.GoToAsync(Routes.JudgeSetup));
        }

        public ObservableCollection<TourneyModel> Tourneys { get; } = new();

        public ICommand RefreshCommand { get; }
        public ICommand ChangeJudgeCommand { get; }

        public string JudgeLine => _judge.Name is { } name ? $"Signed in as {name}" : "No judge name set";

        public bool IsEmpty => !IsBusy && Tourneys.Count == 0 && !HasError;

        public void RefreshJudgeLine() => OnPropertyChanged(nameof(JudgeLine));

        public async Task LoadAsync()
        {
            if (_loading)
                return;

            _loading = true;
            IsBusy = true;
            ClearError();

            try
            {
                var result = await _api.GetRunningTourneysAsync();

                if (!result.IsSuccess)
                {
                    ErrorMessage = result.Error;
                    return;
                }

                Tourneys.Clear();

                foreach (var tourney in result.Data ?? new List<TourneyModel>())
                    Tourneys.Add(tourney);
            }
            finally
            {
                _loading = false;
                IsBusy = false;
                OnPropertyChanged(nameof(IsEmpty));
            }
        }

        public async Task OpenAsync(TourneyModel? tourney)
        {
            if (tourney is null)
                return;

            await Shell.Current.GoToAsync(
                $"{Routes.Matches}?tourneyId={tourney.TourneyId}&tourneyName={Uri.EscapeDataString(tourney.Name)}");
        }
    }
}
