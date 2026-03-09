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
                vm.ResetForm();
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            if (BindingContext is AddTournamentViewModel vm)
                vm.IsDateSet = true;
        }
    }
}