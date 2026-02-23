using System.Text;
using System.Text.Json;

namespace AgilityScoring.Maui.Services
{
    public class TournamentDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
    }

    public class TournamentResponse
    {
        public List<TournamentDto> Tournaments { get; set; }
    }

    public class TournamentResult
    {
        public bool Success { get; set; }
        public TournamentDto Tournament { get; set; }
        public string Action { get; set; }
    }

    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ConfigService _configService;

        public ApiService(ConfigService configService)
        {
            _configService = configService;
            _httpClient = new HttpClient();
        }

        public async Task<TournamentResponse> GetTournamentsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_configService.GetConfig().ApiUrl}/trpc/getTournaments");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    // Parse the tRPC response format
                    var jsonDoc = JsonDocument.Parse(content);
                    if (jsonDoc.RootElement.TryGetProperty("result", out var resultElement) && 
                        resultElement.TryGetProperty("data", out var dataElement))
                    {
                        return dataElement.Deserialize<TournamentResponse>();
                    }
                }
                return new TournamentResponse { Tournaments = new List<TournamentDto>() };
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to fetch tournaments: " + ex.Message);
            }
        }

        public async Task<TournamentResult> GetTournamentAsync(string id)
        {
            try
            {
                var request = new
                {
                    input = new { id }
                };
                
                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json");
                    
                var response = await _httpClient.PostAsync($"{_configService.GetConfig().ApiUrl}/trpc/getTournament", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(responseContent);
                    if (jsonDoc.RootElement.TryGetProperty("result", out var resultElement) && 
                        resultElement.TryGetProperty("data", out var dataElement))
                    {
                        return dataElement.Deserialize<TournamentResult>();
                    }
                }
                return new TournamentResult { Success = false };
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to fetch tournament: " + ex.Message);
            }
        }

        public async Task<TournamentResult> UpsertTournamentAsync(TournamentDto tournament)
        {
            try
            {
                var request = new
                {
                    input = tournament
                };
                
                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json");
                    
                var response = await _httpClient.PostAsync($"{_configService.GetConfig().ApiUrl}/trpc/upsertTournament", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(responseContent);
                    if (jsonDoc.RootElement.TryGetProperty("result", out var resultElement) && 
                        resultElement.TryGetProperty("data", out var dataElement))
                    {
                        return dataElement.Deserialize<TournamentResult>();
                    }
                }
                throw new Exception("Failed to save tournament");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save tournament: " + ex.Message);
            }
        }
    }
}