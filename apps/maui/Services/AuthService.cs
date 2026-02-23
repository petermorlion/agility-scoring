using Microsoft.Maui.Storage;
using Microsoft.Maui.Authentication;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace AgilityScoring.Maui.Services
{
    public class User
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class AuthConfig
    {
        public string AndroidClientId { get; set; }
        public string IosClientId { get; set; }
        public string WebClientId { get; set; }
        public string RedirectUri { get; set; }
        public string[] Scopes { get; set; }
    }

    public class AuthService
    {
        private readonly ConfigService _configService;
        private User _currentUser;
        private const string UserStorageKey = "user";
        private const string RefreshTokenKey = "refresh_token";
        private const string AccessTokenKey = "access_token";
        private const string TokenExpiryKey = "token_expiry";

        public User CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;

        public AuthService(ConfigService configService)
        {
            _configService = configService;
        }

        public async Task LoginAsync()
        {
            try
            {
                var config = _configService.GetConfig().Auth;
                
                // Generate PKCE parameters
                var codeVerifier = GenerateCodeVerifier();
                var codeChallenge = GenerateCodeChallenge(codeVerifier);
                
                // Build Google OAuth URL
                var scopes = string.Join("%20", config.Scopes);
                var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth" +
                    $"?response_type=code" +
                    $"&client_id={config.AndroidClientId}" +
                    $"&redirect_uri={Uri.EscapeDataString(config.RedirectUri)}" +
                    $"&scope={scopes}" +
                    $"&code_challenge={codeChallenge}" +
                    $"&code_challenge_method=S256" +
                    $"&access_type=offline";

                // Authenticate with WebAuthenticator
                var authResult = await WebAuthenticator.AuthenticateAsync(
                    new Uri(authUrl),
                    new Uri(config.RedirectUri));

                if (!authResult.Properties.TryGetValue("code", out var authCode))
                    throw new Exception("Authorization code not received");

                // Exchange auth code for tokens
                var tokens = await ExchangeCodeForTokensAsync(authCode, codeVerifier, config);
                
                // Parse ID token to get user info
                var user = ParseIdToken(tokens.IdToken);
                
                _currentUser = user;
                
                // Store user and tokens
                await SecureStorage.SetAsync(UserStorageKey, JsonSerializer.Serialize(user));
                await SecureStorage.SetAsync(AccessTokenKey, tokens.AccessToken);
                await SecureStorage.SetAsync(TokenExpiryKey, tokens.ExpiresAt.ToString("o"));
                
                if (!string.IsNullOrEmpty(tokens.RefreshToken))
                    await SecureStorage.SetAsync(RefreshTokenKey, tokens.RefreshToken);
            }
            catch (TaskCanceledException)
            {
                throw new Exception("Login was cancelled");
            }
            catch (Exception ex)
            {
                throw new Exception("Login failed: " + ex.Message);
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                _currentUser = null;
                SecureStorage.Remove(UserStorageKey);
                SecureStorage.Remove(AccessTokenKey);
                SecureStorage.Remove(RefreshTokenKey);
                SecureStorage.Remove(TokenExpiryKey);
            }
            catch (Exception ex)
            {
                throw new Exception("Logout failed: " + ex.Message);
            }
        }

        public async Task<bool> CheckAuthAsync()
        {
            try
            {
                var userJson = await SecureStorage.GetAsync(UserStorageKey);
                if (string.IsNullOrEmpty(userJson))
                    return false;

                _currentUser = JsonSerializer.Deserialize<User>(userJson);
                
                // Check if access token is still valid
                var expiryStr = await SecureStorage.GetAsync(TokenExpiryKey);
                if (!string.IsNullOrEmpty(expiryStr) && DateTime.TryParse(expiryStr, out var expiry))
                {
                    if (DateTime.UtcNow < expiry.AddMinutes(-5)) // 5 min buffer
                        return true;
                    
                    // Token expired, try refresh
                    var refreshToken = await SecureStorage.GetAsync(RefreshTokenKey);
                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        try
                        {
                            var tokens = await RefreshAccessTokenAsync(refreshToken);
                            await SecureStorage.SetAsync(AccessTokenKey, tokens.AccessToken);
                            await SecureStorage.SetAsync(TokenExpiryKey, tokens.ExpiresAt.ToString("o"));
                            return true;
                        }
                        catch
                        {
                            // Refresh failed, user needs to re-login
                            return false;
                        }
                    }
                }
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<User> GetUserAsync()
        {
            if (_currentUser != null)
                return _currentUser;
                
            await CheckAuthAsync();
            return _currentUser;
        }

        private string GenerateCodeVerifier()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Base64UrlEncode(bytes);
        }

        private string GenerateCodeChallenge(string codeVerifier)
        {
            using var sha256 = SHA256.Create();
            var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            return Base64UrlEncode(challengeBytes);
        }

        private string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        private async Task<TokenResponse> ExchangeCodeForTokensAsync(string authCode, string codeVerifier, AuthConfig config)
        {
            using var httpClient = new HttpClient();
            
            var formData = new Dictionary<string, string>
            {
                { "code", authCode },
                { "client_id", config.AndroidClientId },
                { "redirect_uri", config.RedirectUri },
                { "grant_type", "authorization_code" },
                { "code_verifier", codeVerifier }
            };

            var response = await httpClient.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(formData));

            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<JsonElement>(json);
            
            return new TokenResponse
            {
                AccessToken = tokenData.GetProperty("access_token").GetString(),
                RefreshToken = tokenData.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : null,
                IdToken = tokenData.GetProperty("id_token").GetString(),
                ExpiresAt = DateTime.UtcNow.AddSeconds(tokenData.GetProperty("expires_in").GetInt32())
            };
        }

        private async Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken)
        {
            using var httpClient = new HttpClient();
            var config = _configService.GetConfig().Auth;
            
            var formData = new Dictionary<string, string>
            {
                { "refresh_token", refreshToken },
                { "client_id", config.AndroidClientId },
                { "grant_type", "refresh_token" }
            };

            var response = await httpClient.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(formData));

            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<JsonElement>(json);
            
            return new TokenResponse
            {
                AccessToken = tokenData.GetProperty("access_token").GetString(),
                ExpiresAt = DateTime.UtcNow.AddSeconds(tokenData.GetProperty("expires_in").GetInt32())
            };
        }

        private User ParseIdToken(string idToken)
        {
            // JWT format: header.payload.signature
            var parts = idToken.Split('.');
            if (parts.Length != 3)
                throw new Exception("Invalid ID token format");

            // Decode payload (base64url)
            var payload = parts[1];
            // Pad to multiple of 4
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            payload = payload.Replace('-', '+').Replace('_', '/');
            
            var payloadBytes = Convert.FromBase64String(payload);
            var payloadJson = Encoding.UTF8.GetString(payloadBytes);
            var claims = JsonSerializer.Deserialize<JsonElement>(payloadJson);

            return new User
            {
                Id = claims.GetProperty("sub").GetString(),
                Name = claims.TryGetProperty("name", out var name) ? name.GetString() : null,
                Email = claims.TryGetProperty("email", out var email) ? email.GetString() : null
            };
        }

        private class TokenResponse
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public string IdToken { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
    }
}