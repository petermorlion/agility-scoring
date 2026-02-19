using AgilityScoring.Maui.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace AgilityScoring.Maui.ViewModels
{
    public partial class TournamentListViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly ApiService _apiService;
        private readonly NavigationService _navigationService;

        public TournamentListViewModel(AuthService authService, ApiService apiService, NavigationService navigationService)
        {
            _authService = authService;
            _apiService = apiService;
            _navigationService = navigationService;
            Title = "Tournaments";
            Tournaments = new ObservableCollection<TournamentDto>();
        }

        private bool _isAuthenticated;
        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set => SetProperty(ref _isAuthenticated, value);
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
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
            try
            {
                IsLoading = true;
                IsAuthenticated = await _authService.CheckAuthAsync();
                
                if (IsAuthenticated)
                {
                    var user = await _authService.GetUserAsync();
                    UserName = user?.Name ?? "User";
                    await LoadTournamentsAsync();
                }
                else
                {
                    await _navigationService.NavigateToLoginAsync("//tournament-list");
                }
            }
            finally
            {
                IsLoading = false;
            }
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
                
                var response = await _apiService.GetTournamentsAsync();
                
                Application.Current.Dispatcher.Dispatch(() =>
                {
                    Tournaments.Clear();
                    foreach (var tournament in response.Tournaments)
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
        private async Task NavigateToSettings()
        {
            await Shell.Current.GoToAsync("//settings");
        }

        [RelayCommand]
        private async Task NavigateToAddTournament()
        {
            await Shell.Current.GoToAsync("//add-tournament");
        }

        [RelayCommand]
        private async Task Logout()
        {
            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync("//index");
        }

        [RelayCommand]
        private async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}