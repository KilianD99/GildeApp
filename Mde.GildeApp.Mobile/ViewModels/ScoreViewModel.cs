using System.Windows.Input;
using GildeApp.Mobile.Core.Models;
using GildeApp.Mobile.Core.Services.Interfaces;
using GildeApp.Mobile.Services;

namespace GildeApp.Mobile.ViewModels
{
    [QueryProperty(nameof(MatchIdText), "matchId")]
    public class ScoreViewModel : BaseViewModel
    {
        private static readonly TimeSpan RenewInterval = TimeSpan.FromSeconds(45);

        private readonly IGildeApi _api;

        private Guid _matchId;
        private IDispatcherTimer? _renewTimer;

        private string _firstName = string.Empty;
        private string _secondName = string.Empty;
        private string _tourneyName = string.Empty;
        private int _firstScore;
        private int _secondScore;
        private int _maxScore;
        private bool _isFinished;

        public ScoreViewModel(IGildeApi api)
        {
            _api = api;

            FirstPlusCommand = new Command(async () => await AdjustAsync(first: true, delta: +1));
            FirstMinusCommand = new Command(async () => await AdjustAsync(first: true, delta: -1));
            SecondPlusCommand = new Command(async () => await AdjustAsync(first: false, delta: +1));
            SecondMinusCommand = new Command(async () => await AdjustAsync(first: false, delta: -1));
            FinishCommand = new Command(async () => await FinishAsync());

            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(IsBusy))
                    OnPropertyChanged(nameof(CanTap));
            };
        }
        public string MatchIdText
        {
            get => _matchId.ToString();
            set
            {
                if (Guid.TryParse(value, out var parsed))
                    _matchId = parsed;
            }
        }

        public string FirstName
        {
            get => _firstName;
            private set => Set(ref _firstName, value);
        }

        public string SecondName
        {
            get => _secondName;
            private set => Set(ref _secondName, value);
        }

        public string TourneyName
        {
            get => _tourneyName;
            private set => Set(ref _tourneyName, value);
        }

        public int FirstScore
        {
            get => _firstScore;
            private set => Set(ref _firstScore, value);
        }

        public int SecondScore
        {
            get => _secondScore;
            private set => Set(ref _secondScore, value);
        }

        public int MaxScore
        {
            get => _maxScore;
            private set
            {
                if (Set(ref _maxScore, value))
                    OnPropertyChanged(nameof(MaxScoreLine));
            }
        }

        public string MaxScoreLine => MaxScore > 0 ? $"Fenced to {MaxScore}" : string.Empty;

        public bool IsFinished
        {
            get => _isFinished;
            private set
            {
                if (Set(ref _isFinished, value))
                {
                    OnPropertyChanged(nameof(CanScore));
                    OnPropertyChanged(nameof(CanTap));
                }
            }
        }

        public bool CanScore => !IsFinished;
        public bool CanTap => !IsFinished && !IsBusy;

        public ICommand FirstPlusCommand { get; }
        public ICommand FirstMinusCommand { get; }
        public ICommand SecondPlusCommand { get; }
        public ICommand SecondMinusCommand { get; }
        public ICommand FinishCommand { get; }

        public async Task LoadAsync()
        {
            if (_matchId == Guid.Empty)
                return;

            IsBusy = true;
            ClearError();

            try
            {
                var result = await _api.GetMatchAsync(_matchId);

                if (!result.IsSuccess || result.Data is null)
                {
                    ErrorMessage = result.Error;
                    return;
                }

                Apply(result.Data);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void Apply(MatchDetailModel match)
        {
            FirstName = $"{match.FirstPosition}. {match.FirstName}";
            SecondName = $"{match.SecondPosition}. {match.SecondName}";
            TourneyName = match.TourneyName;
            FirstScore = match.FirstScore;
            SecondScore = match.SecondScore;
            MaxScore = match.MaxScore;
            IsFinished = match.IsFinished;
        }
        public void StartRenewing(IDispatcher dispatcher)
        {
            if (_renewTimer is not null)
                return;

            _renewTimer = dispatcher.CreateTimer();
            _renewTimer.Interval = RenewInterval;
            _renewTimer.Tick += async (_, _) => await RenewAsync();
            _renewTimer.Start();
        }

        public void StopRenewing()
        {
            _renewTimer?.Stop();
            _renewTimer = null;
        }

        private async Task RenewAsync()
        {
            if (_matchId == Guid.Empty || IsFinished)
                return;

            var result = await _api.ClaimAsync(_matchId);
            if (!result.IsSuccess && result.IsConflict)
            {
                ErrorMessage = result.Error;
                StopRenewing();
            }
        }

        public async Task ReleaseAsync()
        {
            StopRenewing();

            if (_matchId == Guid.Empty || IsFinished)
                return;

            try
            {
                await _api.ReleaseAsync(_matchId);
            }
            catch (Exception)
            {
                
            }
        }

        private async Task AdjustAsync(bool first, int delta)
        {
            if (IsBusy || IsFinished)
                return;

            var newFirst = first ? FirstScore + delta : FirstScore;
            var newSecond = first ? SecondScore : SecondScore + delta;

            if (newFirst < 0 || newSecond < 0)
                return;

            if (MaxScore > 0 && (newFirst > MaxScore || newSecond > MaxScore))
            {
                ErrorMessage = $"This tourney is fenced to {MaxScore}.";
                return;
            }

            FirstScore = newFirst;
            SecondScore = newSecond;

            await PushAsync(newFirst, newSecond, finish: false);
        }

        private async Task FinishAsync()
        {
            if (IsBusy || IsFinished)
                return;

            var confirmed = await Shell.Current.DisplayAlert(
                "Submit final score",
                $"{FirstScore} – {SecondScore}. This locks the result in.",
                "Submit",
                "Cancel");

            if (!confirmed)
                return;

            if (await PushAsync(FirstScore, SecondScore, finish: true))
            {
                StopRenewing();
                await Shell.Current.GoToAsync("..");
            }
        }

        private async Task<bool> PushAsync(int firstScore, int secondScore, bool finish)
        {
            IsBusy = true;
            ClearError();

            try
            {
                var result = await _api.SubmitScoreAsync(_matchId, firstScore, secondScore, finish);

                if (!result.IsSuccess)
                {
                    ErrorMessage = result.Error;

                    await LoadAsync();
                    return false;
                }

                if (result.Data is not null)
                    Apply(result.Data);

                return true;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}