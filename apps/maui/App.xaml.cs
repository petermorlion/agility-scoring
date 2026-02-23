using AgilityScoring.Maui.Services;

namespace AgilityScoring.Maui
{
    public partial class App : Application
    {
        public App(AuthService authService)
        {
            InitializeComponent();
                        
            // Initialize main page based on auth state
            MainPage = new AppShell();
        }
    }
}