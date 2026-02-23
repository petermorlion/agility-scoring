using AgilityScoring.Maui.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System.ComponentModel.DataAnnotations;

namespace AgilityScoring.Maui.ViewModels
{
    public partial class AddTournamentViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly ApiService _apiService;
        private readonly NavigationService _navigationService;

        public AddTournamentViewModel() : this(null, null, null)
        {
        }

        public AddTournamentViewModel(AuthService authService, ApiService apiService, NavigationService navigationService)
        {
            _authService = authService;
            _apiService = apiService;
            _navigationService = navigationService;
            Title = "Add Tournament";
            Date = DateTime.Today;
        }

        private string _name;
        [Required(ErrorMessage = "Tournament name is required")]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
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
                var isAuthenticated = await _authService.CheckAuthAsync();
                
                if (!isAuthenticated)
                {
                    await _navigationService.NavigateToLoginAsync("//add-tournament");
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SaveTournament()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = "Tournament name is required";
                return;
            }

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var tournament = new TournamentDto
                {
                    Name = Name.Trim(),
                    Date = Date.ToString("yyyy-MM-dd")
                };

                var result = await _apiService.UpsertTournamentAsync(tournament);
                
                if (result.Success)
                {
                    await Shell.Current.GoToAsync("//tournament-list");
                }
                else
                {
                    ErrorMessage = "Failed to save tournament";
                }
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
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}