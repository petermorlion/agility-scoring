using AgilityScoring.Maui.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace AgilityScoring.Maui.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly LocalizationService _localizationService;

        public SettingsViewModel(LocalizationService localizationService)
        {
            _localizationService = localizationService;
            Title = "Settings";
            AvailableLanguages = new List<string> { "English", "Français", "Deutsch", "Nederlands" };
            SelectedLanguage = localizationService.CurrentLanguage;
        }

        public List<string> AvailableLanguages { get; }

        private string _selectedLanguage;
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        [RelayCommand]
        private void ChangeLanguage()
        {
            _localizationService.SetLanguage(SelectedLanguage);
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("//tournament-list");
        }
    }
}