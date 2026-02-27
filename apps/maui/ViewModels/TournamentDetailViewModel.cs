using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace AgilityScoring.Maui.ViewModels
{
    [QueryProperty(nameof(TournamentId), "tournamentId")]
    [QueryProperty(nameof(TournamentName), "tournamentName")]
    public partial class TournamentDetailViewModel : BaseViewModel, IQueryAttributable
    {
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

        public string ContestantName => $"Contestant {ContestantNumber}";

        public bool CanGoPrevious => ContestantNumber > 1;

        public TournamentDetailViewModel()
        {
            Title = "Tournament Detail";
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
        }
    }
}
