using AgilityScoring.Maui.Services;
using AgilityScoring.Maui.ViewModels;
using AgilityScoring.Maui.Views;
using CommunityToolkit.Maui;
using MauiIcons.Fluent;

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
                .UseFluentMauiIcons()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                });

            // Register services
            builder.Services.AddSingleton<LocalizationService>();
            builder.Services.AddSingleton<LocalStorageService>();
            builder.Services.AddSingleton<NavigationService>();

            // Register view models
            builder.Services.AddSingleton<IndexViewModel>();
            builder.Services.AddSingleton<TournamentListViewModel>();
            builder.Services.AddSingleton<AddTournamentViewModel>();
            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<TournamentDetailViewModel>();

            // Register pages
            builder.Services.AddSingleton<IndexPage>();
            builder.Services.AddSingleton<TournamentListPage>();
            builder.Services.AddSingleton<AddTournamentPage>();
            builder.Services.AddSingleton<SettingsPage>();
            builder.Services.AddSingleton<TournamentDetailPage>();

            return builder.Build();
        }
    }
}