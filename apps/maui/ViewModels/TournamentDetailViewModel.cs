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
                    NextContestantCommand.NotifyCanExecuteChanged();
                    PreviousContestantCommand.NotifyCanExecuteChanged();
                    _ = LoadContestantScoresAsync();
                }
            }
        }

        public string ContestantName => _contestantNames.TryGetValue(ContestantNumber, out var name)
            ? name
            : $"Contestant {ContestantNumber}";

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
            OnPropertyChanged(nameof(ContestantName));
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
