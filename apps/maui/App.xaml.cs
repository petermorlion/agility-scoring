using AgilityScoring.Maui.Services;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace AgilityScoring.Maui
{
    public partial class App : Application
    {
        public App(LocalizationService localizationService)
        {
            InitializeComponent();
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            Resources.Add("Loc", localizationService);
            MainPage = new AppShell();
        }

        private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            Debug.WriteLine($"***** Handling Unhandled Exception *****: {e.Exception.Message}");
            // YourLogger.LogError($"***** Handling Unhandled Exception *****: {e.Exception.Message}");
        }
    }
}