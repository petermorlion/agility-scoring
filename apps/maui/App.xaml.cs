using AgilityScoring.Maui.Services;

namespace AgilityScoring.Maui
{
    public partial class App : Application
    {
        public App(LocalizationService localizationService)
        {
            InitializeComponent();
            Resources.Add("Loc", localizationService);
            MainPage = new AppShell();
        }
    }
}