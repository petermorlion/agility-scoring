using AgilityScoring.Maui.ViewModels;
using MauiIcons.Core;

namespace AgilityScoring.Maui.Views
{
    public partial class TournamentDetailPage : ContentPage
    {
        public TournamentDetailPage(TournamentDetailViewModel viewModel)
        {
            // Workaround for MauiIcons url-style namespace in XAML (dotnet/maui#7503)
            _ = new MauiIcon();
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
