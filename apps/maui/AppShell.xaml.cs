using AgilityScoring.Maui.ViewModels;
using AgilityScoring.Maui.Views;

namespace AgilityScoring.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            // Set up routing
            Routing.RegisterRoute("index", typeof(IndexPage));
            Routing.RegisterRoute("tournament-list", typeof(TournamentListPage));
            Routing.RegisterRoute("add-tournament", typeof(AddTournamentPage));
            Routing.RegisterRoute("settings", typeof(SettingsPage));
            Routing.RegisterRoute("tournament-detail", typeof(TournamentDetailPage));
        }
    }
}