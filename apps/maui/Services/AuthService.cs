using Microsoft.Identity.Client;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        private readonly IPublicClientApplication _publicClientApplication;
        private readonly AuthConfig _config;
        private User _currentUser;

        public User CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;

        public AuthService(AuthConfig config)
        {
            _config = config;
            _publicClientApplication = PublicClientApplicationBuilder
                .Create(config.AndroidClientId)
                .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                .WithRedirectUri(config.RedirectUri)
                .WithParentActivityOrWindow(() => PlatformConfig.GetParentWindow())
                .Build();
        }

        public async Task LoginAsync()
        {
            try
            {
                var result = await _publicClientApplication
                    .AcquireTokenInteractive(_config.Scopes)
                    .ExecuteAsync();
                    
                // Parse user info from ID token
                var user = new User
                {
                    Id = result.UniqueId,
                    Name = result.Account.Username,
                    Email = result.Account.Username // In real app, you'd parse the email from the token
                };
                
                _currentUser = user;
                await SecureStorage.SetAsync("user", JsonSerializer.Serialize(user));
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
                var accounts = await _publicClientApplication.GetAccountsAsync();
                while (accounts.Any())
                {
                    await _publicClientApplication.RemoveAsync(accounts.First());
                    accounts = await _publicClientApplication.GetAccountsAsync();
                }
                
                _currentUser = null;
                SecureStorage.Remove("user");
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
                var userJson = await SecureStorage.GetAsync("user");
                if (!string.IsNullOrEmpty(userJson))
                {
                    _currentUser = JsonSerializer.Deserialize<User>(userJson);
                    return true;
                }
                
                var accounts = await _publicClientApplication.GetAccountsAsync();
                if (accounts.Any())
                {
                    var result = await _publicClientApplication
                        .AcquireTokenSilent(_config.Scopes, accounts.First())
                        .ExecuteAsync();
                    
                    _currentUser = new User
                    {
                        Id = result.UniqueId,
                        Name = result.Account.Username,
                        Email = result.Account.Username
                    };
                    
                    await SecureStorage.SetAsync("user", JsonSerializer.Serialize(_currentUser));
                    return true;
                }
                
                return false;
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
    }

    public static class PlatformConfig
    {
        public static object Instance { get; set; }

        public static object GetParentWindow()
        {
            #if ANDROID
            return Instance as Android.App.Activity ?? throw new InvalidOperationException("Parent window not set");
            #elif IOS
            return Instance as UIKit.UIWindow ?? throw new InvalidOperationException("Parent window not set");
            #else
            return Instance;
            #endif
        }
    }
}