using AgilityScoring.Maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace AgilityScoring.Maui.ViewModels
{
    [QueryProperty(nameof(TournamentId), "tournamentId")]
    [QueryProperty(nameof(TournamentName), "tournamentName")]
    public partial class TournamentDetailViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly LocalStorageService _localStorageService;
        private Dictionary<int, string> _contestantNames = new();

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
                }
            }
        }

        public string ContestantName => _contestantNames.TryGetValue(ContestantNumber, out var name)
            ? name
            : $"Contestant {ContestantNumber}";

        public bool CanGoPrevious => ContestantNumber > 1;

        public TournamentDetailViewModel(LocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
            Title = "Tournament Detail";
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

        [RelayCommand]
        private void NextContestant()
        {
            ContestantNumber++;
        }

        [RelayCommand(CanExecute = nameof(CanGoPrevious))]
        private void PreviousContestant()
        {
            if (ContestantNumber > 1)
            {
                ContestantNumber--;
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("tournamentId"))
            {
                TournamentId = Uri.UnescapeDataString(query["tournamentId"].ToString());
            }

            if (query.ContainsKey("tournamentName"))
            {
                TournamentName = Uri.UnescapeDataString(query["tournamentName"].ToString());
            }

            _ = LoadContestantNamesAsync();
        }
    }
}
