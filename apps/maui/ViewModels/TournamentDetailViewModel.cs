using AgilityScoring.Maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace AgilityScoring.Maui.ViewModels
{
    [QueryProperty(nameof(TournamentId), "tournamentId")]
    [QueryProperty(nameof(TournamentName), "tournamentName")]
    public partial class TournamentDetailViewModel : BaseViewModel, IQueryAttributable
    {
        private static readonly (string Key, string LocKey)[] ObstacleDefinitions =
        {
            ("jump",    "ObstacleJump"),
            ("aframe",  "ObstacleAFrame"),
            ("tunnel",  "ObstacleTunnel"),
            ("chute",   "ObstacleChute"),
            ("weave",   "ObstacleWeave"),
            ("tire",    "ObstacleTire"),
            ("teeter",  "ObstacleTeeter"),
            ("table",   "ObstacleTable"),
            ("dogwalk", "ObstacleDogWalk"),
        };

        private readonly LocalStorageService _localStorageService;
        private readonly LocalizationService _localizationService;
        private Dictionary<int, string> _contestantNames = new();
        private Dictionary<int, bool> _contestantDisqualified = new();
        private Dictionary<int, ContestantTime> _contestantTimes = new();

        public ObservableCollection<ObstacleRowViewModel> Obstacles { get; } = new();

        private string _tournamentId;
        public string TournamentId
        {
            get => _tournamentId;
            set => SetProperty(ref _tournamentId, value);
        }

        private string _tournamentName;
        public string TournamentName
        {
            get => _tournamentName;
            set => SetProperty(ref _tournamentName, value);
        }

        private int _contestantNumber = 1;
        public int ContestantNumber
        {
            get => _contestantNumber;
            set
            {
                if (SetProperty(ref _contestantNumber, value))
                {
                    OnPropertyChanged(nameof(ContestantName));
                    OnPropertyChanged(nameof(CanGoPrevious));
                    IsDisqualified = _contestantDisqualified.GetValueOrDefault(value, false);
                    var time = _contestantTimes.GetValueOrDefault(value);
                    ContestantMinutes = time?.Minutes ?? 0;
                    ContestantSeconds = time?.Seconds ?? 0;
                    NextContestantCommand.NotifyCanExecuteChanged();
                    PreviousContestantCommand.NotifyCanExecuteChanged();
                    _ = LoadContestantScoresAsync();
                }
            }
        }

        public string ContestantName => _contestantNames.TryGetValue(ContestantNumber, out var name)
            ? name
            : $"Contestant {ContestantNumber}";

        private bool _isDisqualified;
        public bool IsDisqualified
        {
            get => _isDisqualified;
            set
            {
                if (SetProperty(ref _isDisqualified, value))
                {
                    OnPropertyChanged(nameof(DisqualifiedButtonText));
                }
            }
        }

        public string DisqualifiedButtonText => IsDisqualified ? "Disqualified ✓" : "Mark as Disqualified";

        private int _contestantMinutes;
        public int ContestantMinutes
        {
            get => _contestantMinutes;
            set
            {
                if (SetProperty(ref _contestantMinutes, value))
                {
                    _ = SaveContestantTimeAsync();
                }
            }
        }

        private int _contestantSeconds;
        public int ContestantSeconds
        {
            get => _contestantSeconds;
            set
            {
                if (SetProperty(ref _contestantSeconds, value))
                {
                    _ = SaveContestantTimeAsync();
                }
            }
        }

        public bool CanGoPrevious => ContestantNumber > 1;

        public TournamentDetailViewModel(LocalStorageService localStorageService, LocalizationService localizationService)
        {
            _localStorageService = localStorageService;
            _localizationService = localizationService;
            Title = "Tournament Detail";

            foreach (var (key, locKey) in ObstacleDefinitions)
                Obstacles.Add(new ObstacleRowViewModel(key, locKey, localizationService, SaveCurrentScoresAsync));
        }

        public async Task LoadContestantNamesAsync()
        {
            if (string.IsNullOrEmpty(TournamentId)) return;
            var tournament = await _localStorageService.GetTournamentAsync(TournamentId);
            _contestantNames = tournament?.ContestantNames != null
                ? new Dictionary<int, string>(tournament.ContestantNames)
                : new Dictionary<int, string>();
            _contestantDisqualified = tournament?.ContestantDisqualified != null
                ? new Dictionary<int, bool>(tournament.ContestantDisqualified)
                : new Dictionary<int, bool>();
            _contestantTimes = tournament?.ContestantTimes != null
                ? new Dictionary<int, ContestantTime>(tournament.ContestantTimes)
                : new Dictionary<int, ContestantTime>();
            OnPropertyChanged(nameof(ContestantName));
            IsDisqualified = _contestantDisqualified.GetValueOrDefault(ContestantNumber, false);
            var time = _contestantTimes.GetValueOrDefault(ContestantNumber);
            ContestantMinutes = time?.Minutes ?? 0;
            ContestantSeconds = time?.Seconds ?? 0;
        }

        public async Task SetContestantNameAsync(int contestantNumber, string name)
        {
            if (string.IsNullOrEmpty(TournamentId)) return;
            _contestantNames[contestantNumber] = name;
            var tournament = await _localStorageService.GetTournamentAsync(TournamentId);
            if (tournament != null)
            {
                tournament.ContestantNames = _contestantNames;
                await _localStorageService.SaveTournamentAsync(tournament);
            }
            OnPropertyChanged(nameof(ContestantName));
        }

        public async Task LoadContestantScoresAsync()
        {
            if (string.IsNullOrEmpty(TournamentId)) return;
            var tournament = await _localStorageService.GetTournamentAsync(TournamentId);
            var scores = tournament?.ContestantScores?.GetValueOrDefault(ContestantNumber)
                         ?? new Dictionary<string, ObstacleScore>();
            foreach (var obstacle in Obstacles)
            {
                var score = scores.GetValueOrDefault(obstacle.Key) ?? new ObstacleScore();
                obstacle.SetScores(score.Refusals, score.Faults);
            }
        }

        private async Task SaveCurrentScoresAsync()
        {
            if (string.IsNullOrEmpty(TournamentId)) return;
            var tournament = await _localStorageService.GetTournamentAsync(TournamentId);
            if (tournament == null) return;
            tournament.ContestantScores ??= new();
            tournament.ContestantScores[ContestantNumber] = Obstacles.ToDictionary(
                o => o.Key,
                o => new ObstacleScore { Refusals = o.Refusals, Faults = o.Faults });
            await _localStorageService.SaveTournamentAsync(tournament);
        }

        [RelayCommand]
        private void NextContestant()
        {
            ContestantNumber++;
        }

        [RelayCommand(CanExecute = nameof(CanGoPrevious))]
        private void PreviousContestant()
        {
            if (ContestantNumber > 1)
                ContestantNumber--;
        }

        [RelayCommand]
        private async Task ToggleDisqualifiedAsync()
        {
            try
            {
                IsBusy = true;
                IsDisqualified = !IsDisqualified;
                _contestantDisqualified[ContestantNumber] = IsDisqualified;

                var tournament = await _localStorageService.GetTournamentAsync(TournamentId);
                if (tournament != null)
                {
                    tournament.ContestantDisqualified = _contestantDisqualified;
                    await _localStorageService.SaveTournamentAsync(tournament);
                }
            }
            catch (Exception ex)
            {
                IsDisqualified = !IsDisqualified;
                _contestantDisqualified[ContestantNumber] = IsDisqualified;
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to update disqualified status: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveContestantTimeAsync()
        {
            if (string.IsNullOrEmpty(TournamentId)) return;

            try
            {
                _contestantTimes[ContestantNumber] = new ContestantTime
                {
                    Minutes = ContestantMinutes,
                    Seconds = ContestantSeconds
                };

                var tournament = await _localStorageService.GetTournamentAsync(TournamentId);
                if (tournament != null)
                {
                    tournament.ContestantTimes = _contestantTimes;
                    await _localStorageService.SaveTournamentAsync(tournament);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save time: {ex.Message}", "OK");
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("tournamentId"))
                TournamentId = Uri.UnescapeDataString(query["tournamentId"].ToString());

            if (query.ContainsKey("tournamentName"))
                TournamentName = Uri.UnescapeDataString(query["tournamentName"].ToString());

            _contestantNumber = 1;
            OnPropertyChanged(nameof(ContestantNumber));
            OnPropertyChanged(nameof(ContestantName));
            OnPropertyChanged(nameof(CanGoPrevious));

            _ = LoadContestantNamesAsync();
            _ = LoadContestantScoresAsync();
        }
    }
}
