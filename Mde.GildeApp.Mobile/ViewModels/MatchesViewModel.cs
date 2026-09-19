using System.Collections.ObjectModel;
using System.Windows.Input;
using GildeApp.Mobile.Core.Models;
using GildeApp.Mobile.Core.Services.Interfaces;
using GildeApp.Mobile.Services;

namespace GildeApp.Mobile.ViewModels
{
    [QueryProperty(nameof(TourneyIdText), "tourneyId")]
    [QueryProperty(nameof(TourneyName), "tourneyName")]
    public class MatchesViewModel : BaseViewModel
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);

        private readonly IGildeApi _api;

        private Guid _tourneyId;
        private string _tourneyName = string.Empty;
        private IDispatcherTimer? _timer;
        private bool _loading;

        public MatchesViewModel(IGildeApi api)
        {
            _api = api;

            RefreshCommand = new Command(async () => await LoadAsync());
        }

        public Guid TourneyId => _tourneyId;
        public string TourneyIdText
        {
            get => _tourneyId.ToString();
            set
            {
                if (Guid.TryParse(value, out var parsed))
                    _tourneyId = parsed;
            }
        }
        public string TourneyName
        {
            get => _tourneyName;
            set => Set(ref _tourneyName, value ?? string.Empty);
        }

        public ObservableCollection<MatchModel> Matches { get; } = new();

        public ICommand RefreshCommand { get; }

        public bool IsEmpty => !IsBusy && Matches.Count == 0 && !HasError;

        public void StartPolling(IDispatcher dispatcher)
        {
            if (_timer is not null)
                return;

            _timer = dispatcher.CreateTimer();
            _timer.Interval = PollInterval;
            _timer.Tick += async (_, _) => await LoadAsync(quiet: true);
            _timer.Start();
        }

        public void StopPolling()
        {
            _timer?.Stop();
            _timer = null;
        }
        public async Task LoadAsync(bool quiet = false)
        {
            if (_loading || TourneyId == Guid.Empty)
                return;

            _loading = true;

            if (!quiet)
            {
                IsBusy = true;
                ClearError();
            }

            try
            {
                var result = await _api.GetMatchesAsync(TourneyId);

                if (!result.IsSuccess)
                {
                    if (!quiet)
                        ErrorMessage = result.Error;
                    return;
                }

                var incoming = result.Data ?? new List<MatchModel>();

                var ordered = incoming
                    .OrderBy(m => m.IsFinished)
                    .ThenBy(m => m.Order)
                    .ToList();

                Matches.Clear();

                foreach (var match in ordered)
                    Matches.Add(match);
            }
            finally
            {
                _loading = false;

                if (!quiet)
                    IsBusy = false;

                OnPropertyChanged(nameof(IsEmpty));
            }
        }

        public async Task OpenAsync(MatchModel? match)
        {
            if (match is null || _loading)
                return;

            if (match.IsFinished)
            {
                ErrorMessage = "That match is already finished.";
                return;
            }

            IsBusy = true;
            ClearError();

            try
            {
                var claim = await _api.ClaimAsync(match.MatchId);

                if (!claim.IsSuccess)
                {
                    ErrorMessage = claim.Error;
                    await LoadAsync(quiet: true);
                    return;
                }

                await Shell.Current.GoToAsync($"{Routes.Score}?matchId={match.MatchId}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}