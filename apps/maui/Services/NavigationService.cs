using Microsoft.Maui.Storage;
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

        public async Task NavigateToLoginAsync(string redirectRoute = "//tournament-list")
        {
            // Store the redirect route for after successful login
            Preferences.Set("redirect_route", redirectRoute);
            await Shell.Current.GoToAsync("//login");
        }

        public string GetRedirectRoute()
        {
            return Preferences.Get("redirect_route", "//tournament-list");
        }

        public void ClearRedirectRoute()
        {
            Preferences.Remove("redirect_route");
        }
    }
}