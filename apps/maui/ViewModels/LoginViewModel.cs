using AgilityScoring.Maui.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace AgilityScoring.Maui.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly ConfigService _configService;
        private readonly NavigationService _navigationService;

        public LoginViewModel(AuthService authService, ConfigService configService, NavigationService navigationService)
        {
            _authService = authService;
            _configService = configService;
            _navigationService = navigationService;
            Title = "Login";
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        [RelayCommand]
        private async Task LoginWithGoogle()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                
                await _authService.LoginAsync();
                
                // After successful login, navigate based on redirect parameter
                var redirect = _navigationService.GetRedirectRoute();
                _navigationService.ClearRedirectRoute();
                await Shell.Current.GoToAsync(redirect);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}