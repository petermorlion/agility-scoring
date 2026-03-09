using AgilityScoring.Maui.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace AgilityScoring.Maui.ViewModels
{
    public partial class TournamentListViewModel : BaseViewModel
    {
        private readonly LocalStorageService _localStorageService;

        public TournamentListViewModel() : this(null)
        {
        }

        public TournamentListViewModel(LocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
            Title = "Tournaments";
            Tournaments = new ObservableCollection<TournamentDto>();
        }

        public ObservableCollection<TournamentDto> Tournaments { get; }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public async Task InitializeAsync()
        {
            await LoadTournamentsAsync();
        }

        [RelayCommand]
        private async Task LoadTournamentsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                IsRefreshing = true;

                var tournaments = await _localStorageService.GetTournamentsAsync();

                var sorted = tournaments.OrderByDescending(t =>
                    DateTime.TryParse(t.Date, out var d) ? d : DateTime.MinValue);

                Application.Current.Dispatcher.Dispatch(() =>
                {
                    Tournaments.Clear();
                    foreach (var tournament in sorted)
                    {
                        Tournaments.Add(tournament);
                    }
                });
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToAddTournament()
        {
            await Shell.Current.GoToAsync("//add-tournament");
        }

        [RelayCommand]
        private async Task SelectTournament(TournamentDto tournament)
        {
            if (tournament == null) return;
            await Shell.Current.GoToAsync($"tournament-detail?tournamentId={tournament.Id}&tournamentName={Uri.EscapeDataString(tournament.Name)}");
        }
    }
}