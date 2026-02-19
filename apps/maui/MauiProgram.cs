using Microsoft.Extensions.Logging;
using AgilityScoring.Maui.Services;
using AgilityScoring.Maui.ViewModels;
using AgilityScoring.Maui.Views;
using CommunityToolkit.Maui;

namespace AgilityScoring.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                });

            // Register services
            builder.Services.AddSingleton<ConfigService>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<NavigationService>();

            // Register view models
            builder.Services.AddSingleton<IndexViewModel>();
            builder.Services.AddSingleton<TournamentListViewModel>();
            builder.Services.AddSingleton<AddTournamentViewModel>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<SettingsViewModel>();

            // Register pages
            builder.Services.AddSingleton<IndexPage>();
            builder.Services.AddSingleton<TournamentListPage>();
            builder.Services.AddSingleton<AddTournamentPage>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<SettingsPage>();

#if DEBUG
            //builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}