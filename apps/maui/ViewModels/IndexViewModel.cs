using AgilityScoring.Maui.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace AgilityScoring.Maui.ViewModels
{
    public partial class IndexViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly ApiService _apiService;
        private readonly NavigationService _navigationService;

        public IndexViewModel() : this(null, null, null)
        {
        }

        public IndexViewModel(AuthService authService, ApiService apiService, NavigationService navigationService)
        {
            _authService = authService;
            _apiService = apiService;
            _navigationService = navigationService;
            Title = "Agility Scoring";
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
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToSettings()
        {
            await Shell.Current.GoToAsync("//settings");
        }

        [RelayCommand]
        private async Task NavigateToTournaments()
        {
            if (IsAuthenticated)
            {
                await Shell.Current.GoToAsync("//tournament-list");
            }
            else
            {
                await _navigationService.NavigateToLoginAsync("//tournament-list");
            }
        }

        [RelayCommand]
        private async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}