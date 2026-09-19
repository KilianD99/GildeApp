using System.Collections.ObjectModel;
using System.Windows.Input;
using GildeApp.Mobile.Models;
using GildeApp.Mobile.Services;

namespace GildeApp.Mobile.ViewModels
{
    [QueryProperty(nameof(TourneyIdText), "tourneyId")]
    [QueryProperty(nameof(TourneyName), "tourneyName")]
    public class MatchesViewModel : BaseViewModel
    {
        /// <summary>
        /// How often the list re-checks who holds what. Claims change from other
        /// people's phones, so without this a judge would tap a match that was taken
        /// ten seconds ago.
        /// </summary>
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);

        private readonly IGildeApi _api;

        private Guid _tourneyId;
        private string _tourneyName = string.Empty;
        private IDispatcherTimer? _timer;

        // Separate from IsBusy on purpose. RefreshView.IsRefreshing is a two-way
        // binding, so a pull-to-refresh sets IsBusy = true before it fires the
        // command -- guarding on IsBusy would make LoadAsync return immediately, leave
        // the spinner turning, and kill the claim poll with it.
        private bool _loading;

        public MatchesViewModel(IGildeApi api)
        {
            _api = api;

            RefreshCommand = new Command(async () => await LoadAsync());
        }

        public Guid TourneyId => _tourneyId;

        /// <summary>
        /// Shell hands query values over as strings, so take a string and parse it
        /// rather than relying on Shell to convert straight to Guid.
        /// </summary>
        public string TourneyIdText
        {
            get => _tourneyId.ToString();
            set
            {
                if (Guid.TryParse(value, out var parsed))
                    _tourneyId = parsed;
            }
        }

        /// <summary>
        /// Already decoded: Shell unescapes query values on the way in, so calling
        /// UnescapeDataString again here would mangle a name containing a literal %.
        /// </summary>
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

        /// <summary>
        /// quiet: true is the background poll -- it must not flip the spinner on, or
        /// the list would flicker every five seconds.
        /// </summary>
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

                // Unfinished bouts first, in running order, so the next one to fence
                // is at the top.
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
                // Claim before navigating: if someone else is on it, the judge finds
                // out here rather than after typing half a score.
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
