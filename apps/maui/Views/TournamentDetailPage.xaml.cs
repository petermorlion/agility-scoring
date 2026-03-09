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

        private async void OnRenameContestantClicked(object sender, EventArgs e)
        {
            if (BindingContext is not TournamentDetailViewModel vm) return;

            var result = await DisplayPromptAsync(
                "Rename Contestant",
                $"Enter a name for contestant {vm.ContestantNumber}:",
                initialValue: vm.ContestantName.StartsWith("Contestant ") ? string.Empty : vm.ContestantName,
                maxLength: 50,
                keyboard: Keyboard.Text);

            if (result != null)
                await vm.SetContestantNameAsync(vm.ContestantNumber, result.Trim());
        }
    }
}
