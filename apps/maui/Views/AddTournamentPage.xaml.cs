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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is AddTournamentViewModel vm)
            {
                vm.Name = string.Empty;
                vm.Date = DateTime.Today;
                vm.ErrorMessage = string.Empty;
            }
        }
    }
}