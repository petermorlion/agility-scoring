using System.Diagnostics;

namespace AgilityScoring.Maui.Services
{
    public class AppConfig
    {
        public string ApiUrl { get; set; }
        public AuthConfig Auth { get; set; }
        public bool IsDevelopment { get; set; }
    }

    public class ConfigService
    {
        private readonly AppConfig _config;

        public ConfigService()
        {
            // Load configuration based on environment
            _config = GetConfiguration();
        }

        public AppConfig GetConfig() => _config;

        private AppConfig GetConfiguration()
        {
            // Check if running in development mode
            bool isDevelopment = Debugger.IsAttached;

            return new AppConfig
            {
                IsDevelopment = isDevelopment,
                ApiUrl = isDevelopment ? "http://localhost:3000" : "https://api.yourdomain.com",
                Auth = new AuthConfig
                {
                    AndroidClientId = isDevelopment 
                        ? "650791542042-6ub4916cfv65tt54566ecedu6qkaqgti.apps.googleusercontent.com" 
                        : "YOUR_PROD_ANDROID_CLIENT_ID.apps.googleusercontent.com",
                    IosClientId = isDevelopment 
                        ? "YOUR_DEV_IOS_CLIENT_ID.apps.googleusercontent.com" 
                        : "YOUR_PROD_IOS_CLIENT_ID.apps.googleusercontent.com",
                    WebClientId = isDevelopment 
                        ? "YOUR_DEV_WEB_CLIENT_ID.apps.googleusercontent.com" 
                        : "YOUR_PROD_WEB_CLIENT_ID.apps.googleusercontent.com",
                    // Reverse client ID scheme — supported by Google for native Android apps.
                    // The Google Cloud Console credential must be an Android type (or a Web type
                    // with this redirect URI explicitly allowed).
                    RedirectUri = "com.googleusercontent.apps.650791542042-6ub4916cfv65tt54566ecedu6qkaqgti:/oauth2redirect",
                    Scopes = new string[] { "openid", "email", "profile" }
                }
            };
        }
    }
}