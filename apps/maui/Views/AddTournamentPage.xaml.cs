using AgilityScoring.Maui.ViewModels;

namespace AgilityScoring.Maui.Views
{
    public partial class AddTournamentPage : ContentPage
    {
        public AddTournamentPage(AddTournamentViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}