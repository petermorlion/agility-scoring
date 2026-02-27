using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace AgilityScoring.Maui.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        {
            Title = "Settings";
            AvailableLanguages = new List<string> { "English", "Français", "Deutsch", "Nederlands" };
            SelectedLanguage = "English";
        }

        public List<string> AvailableLanguages { get; }

        private string _selectedLanguage;
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        [RelayCommand]
        private async Task ChangeLanguage()
        {
            // TODO: Implement language change logic
            await Shell.Current.DisplayAlert("Language", $"Language changed to {SelectedLanguage}", "OK");
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}