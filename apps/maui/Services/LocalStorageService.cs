using System.Text.Json;

namespace AgilityScoring.Maui.Services
{
    public class ObstacleScore
    {
        public int Refusals { get; set; }
        public int Faults { get; set; }
    }

    public class TournamentDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public Dictionary<int, string> ContestantNames { get; set; } = new();
        public Dictionary<int, Dictionary<string, ObstacleScore>> ContestantScores { get; set; } = new();
    }

    public class LocalStorageService
    {
        private readonly string _filePath;

        public LocalStorageService()
        {
            _filePath = Path.Combine(FileSystem.AppDataDirectory, "tournaments.json");
        }

        public async Task<List<TournamentDto>> GetTournamentsAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new List<TournamentDto>();

                var json = await File.ReadAllTextAsync(_filePath);
                return JsonSerializer.Deserialize<List<TournamentDto>>(json) ?? new List<TournamentDto>();
            }
            catch
            {
                return new List<TournamentDto>();
            }
        }

        public async Task<TournamentDto> GetTournamentAsync(string id)
        {
            var tournaments = await GetTournamentsAsync();
            return tournaments.FirstOrDefault(t => t.Id == id);
        }

        public async Task SaveTournamentAsync(TournamentDto tournament)
        {
            var tournaments = await GetTournamentsAsync();

            if (string.IsNullOrEmpty(tournament.Id))
                tournament.Id = Guid.NewGuid().ToString();

            var existing = tournaments.FindIndex(t => t.Id == tournament.Id);
            if (existing >= 0)
                tournaments[existing] = tournament;
            else
                tournaments.Add(tournament);

            var json = JsonSerializer.Serialize(tournaments);
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}
