using AgilityScoring.Maui.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace AgilityScoring.Maui.ViewModels
{
    public partial class TournamentListViewModel : BaseViewModel
    {
        private readonly LocalStorageService _localStorageService;
        private readonly PdfExportService _pdfExportService;

        public TournamentListViewModel() : this(null, null)
        {
        }

        public TournamentListViewModel(LocalStorageService localStorageService, PdfExportService pdfExportService)
        {
            _localStorageService = localStorageService;
            _pdfExportService = pdfExportService;
            Title = "Tournaments";
            Tournaments = new ObservableCollection<TournamentDto>();
        }

        public ObservableCollection<TournamentDto> Tournaments { get; }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public async Task InitializeAsync()
        {
            await LoadTournamentsAsync();
        }

        [RelayCommand]
        private async Task LoadTournamentsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                IsRefreshing = true;

                var tournaments = await _localStorageService.GetTournamentsAsync();

                var sorted = tournaments.OrderByDescending(t =>
                    DateTime.TryParse(t.Date, out var d) ? d : DateTime.MinValue);

                Application.Current.Dispatcher.Dispatch(() =>
                {
                    Tournaments.Clear();
                    foreach (var tournament in sorted)
                    {
                        Tournaments.Add(tournament);
                    }
                });
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToAddTournament()
        {
            await Shell.Current.GoToAsync("//add-tournament");
        }

        [RelayCommand]
        private async Task SelectTournament(TournamentDto tournament)
        {
            if (tournament == null) return;
            await Shell.Current.GoToAsync($"tournament-detail?tournamentId={tournament.Id}&tournamentName={Uri.EscapeDataString(tournament.Name)}");
        }

        [RelayCommand]
        private async Task ExportToPdf(TournamentDto tournament)
        {
            if (tournament == null || IsBusy)
                return;

            try
            {
                IsBusy = true;

                // Build contestant list from tournament data
                var contestants = new List<ContestantResult>();

                foreach (var kvp in tournament.ContestantNames)
                {
                    int contestantNumber = kvp.Key;
                    string name = kvp.Value;

                    // Calculate totals
                    int totalRefusals = 0;
                    int totalFaults = 0;

                    if (tournament.ContestantScores.TryGetValue(contestantNumber, out var scores))
                    {
                        foreach (var obstacleScore in scores.Values)
                        {
                            totalRefusals += obstacleScore.Refusals;
                            totalFaults += obstacleScore.Faults;
                        }
                    }

                    bool isDisqualified = tournament.ContestantDisqualified.TryGetValue(contestantNumber, out var dq) && dq;

                    contestants.Add(new ContestantResult
                    {
                        ContestantNumber = contestantNumber,
                        Name = name,
                        TotalRefusals = totalRefusals,
                        TotalFaults = totalFaults,
                        IsDisqualified = isDisqualified
                    });
                }

                // Export to PDF
                var filePath = await _pdfExportService.ExportTournamentToPdfAsync(tournament, contestants);

                // Open the PDF
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath),
                    Title = "Tournament Results"
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to export PDF: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}