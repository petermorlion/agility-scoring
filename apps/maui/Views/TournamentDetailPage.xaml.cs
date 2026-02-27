using AgilityScoring.Maui.ViewModels;

namespace AgilityScoring.Maui.Views
{
    public partial class TournamentDetailPage : ContentPage
    {
        public TournamentDetailPage(TournamentDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
