using AgilityScoring.Maui.ViewModels;

namespace AgilityScoring.Maui.Views
{
    public partial class TournamentListPage : ContentPage
    {
        public TournamentListPage(TournamentListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is TournamentListViewModel viewModel)
            {
                await viewModel.InitializeAsync();
            }
        }
    }
}