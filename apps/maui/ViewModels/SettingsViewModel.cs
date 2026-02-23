using AgilityScoring.Maui.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace AgilityScoring.Maui.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        public SettingsViewModel() : this(null)
        {
        }

        public SettingsViewModel(AuthService authService)
        {
            _authService = authService;
            Title = "Settings";
            
            // Initialize available languages
            AvailableLanguages = new List<string> { "English", "Français", "Deutsch", "Nederlands" };
            SelectedLanguage = "English";
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

        public List<string> AvailableLanguages { get; }

        private string _selectedLanguage;
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
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
        private async Task ChangeLanguage()
        {
            // TODO: Implement language change logic
            // This would integrate with the i18n system
            await Shell.Current.DisplayAlert("Language", $"Language changed to {SelectedLanguage}", "OK");
        }

        [RelayCommand]
        private async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync("//login");
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}