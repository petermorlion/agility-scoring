using Microsoft.Maui.Controls;

namespace AgilityScoring.Maui.Services
{
    public class NavigationService
    {
        public async Task NavigateToAsync(string route)
        {
            await Shell.Current.GoToAsync(route);
        }

        public async Task NavigateBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}