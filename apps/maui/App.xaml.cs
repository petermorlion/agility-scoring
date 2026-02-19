using AgilityScoring.Maui.Services;
using Microsoft.Maui.Controls;

namespace AgilityScoring.Maui
{
    public partial class App : Application
    {
        public App(AuthService authService)
        {
            InitializeComponent();
            
            // Set platform config for auth service
            PlatformConfig.Instance = this;
            
            // Initialize main page based on auth state
            MainPage = new AppShell();
        }
    }
}