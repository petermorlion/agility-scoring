using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace AgilityScoring.Maui.ViewModels
{
    public partial class IndexViewModel : BaseViewModel
    {
        public IndexViewModel()
        {
            Title = "Agility Scoring";
        }

        [RelayCommand]
        private async Task NavigateToSettings()
        {
            await Shell.Current.GoToAsync("//settings");
        }

        [RelayCommand]
        private async Task NavigateToTournaments()
        {
            await Shell.Current.GoToAsync("//tournament-list");
        }
    }
}