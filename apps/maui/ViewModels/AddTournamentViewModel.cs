using AgilityScoring.Maui.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System.ComponentModel.DataAnnotations;

namespace AgilityScoring.Maui.ViewModels
{
    public partial class AddTournamentViewModel : BaseViewModel
    {
        private readonly LocalStorageService _localStorageService;

        public AddTournamentViewModel() : this(null)
        {
        }

        public AddTournamentViewModel(LocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
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

                await _localStorageService.SaveTournamentAsync(tournament);
                await Shell.Current.GoToAsync("//tournament-list");
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
            await Shell.Current.GoToAsync("//tournament-list");
        }
    }
}