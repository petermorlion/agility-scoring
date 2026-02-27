using System.ComponentModel;
using Microsoft.Maui.Storage;

namespace AgilityScoring.Maui.Services
{
    public class LocalizationService : INotifyPropertyChanged
    {
        private string _currentLanguage;

        private static readonly Dictionary<string, Dictionary<string, string>> _translations = new()
        {
            ["English"] = new()
            {
                ["AppTitle"] = "Agility Scoring App",
                ["AppSubtitle"] = "Agility Tournament Manager",
                ["WelcomeMessage"] = "Welcome to Agility Scoring!",
                ["WelcomeDescription"] = "Manage your agility tournaments and track performance",
                ["ViewTournaments"] = "View Tournaments",
                ["AppFeatures"] = "App Features",
                ["FeatureManage"] = "Manage tournaments",
                ["FeatureTrack"] = "Track performance",
                ["FeatureAnalyze"] = "Analyze results",
                ["PoweredBy"] = "Powered by .NET MAUI",
                ["Tournaments"] = "Tournaments",
                ["NoTournaments"] = "No tournaments yet. Create one to get started!",
                ["AddTournament"] = "Add Tournament",
                ["TournamentName"] = "Tournament Name",
                ["TournamentNamePlaceholder"] = "Enter tournament name",
                ["Date"] = "Date",
                ["CreateTournament"] = "Create Tournament",
                ["Cancel"] = "Cancel",
                ["Settings"] = "Settings",
                ["Language"] = "Language",
                ["SelectLanguage"] = "Select language",
                ["ChangeLanguage"] = "Change Language",
                ["Back"] = "Back",
                ["TournamentNameRequired"] = "Tournament name is required",
            },
            ["Français"] = new()
            {
                ["AppTitle"] = "Application Agility",
                ["AppSubtitle"] = "Gestionnaire de tournois d'agility",
                ["WelcomeMessage"] = "Bienvenue dans Agility Scoring !",
                ["WelcomeDescription"] = "Gérez vos tournois d'agility et suivez les performances",
                ["ViewTournaments"] = "Voir les tournois",
                ["AppFeatures"] = "Fonctionnalités",
                ["FeatureManage"] = "Gérer les tournois",
                ["FeatureTrack"] = "Suivre les performances",
                ["FeatureAnalyze"] = "Analyser les résultats",
                ["PoweredBy"] = "Propulsé par .NET MAUI",
                ["Tournaments"] = "Tournois",
                ["NoTournaments"] = "Pas encore de tournois. Créez-en un pour commencer !",
                ["AddTournament"] = "Ajouter un tournoi",
                ["TournamentName"] = "Nom du tournoi",
                ["TournamentNamePlaceholder"] = "Entrez le nom du tournoi",
                ["Date"] = "Date",
                ["CreateTournament"] = "Créer le tournoi",
                ["Cancel"] = "Annuler",
                ["Settings"] = "Paramètres",
                ["Language"] = "Langue",
                ["SelectLanguage"] = "Sélectionner la langue",
                ["ChangeLanguage"] = "Changer la langue",
                ["Back"] = "Retour",
                ["TournamentNameRequired"] = "Le nom du tournoi est obligatoire",
            },
            ["Deutsch"] = new()
            {
                ["AppTitle"] = "Agility Score App",
                ["AppSubtitle"] = "Agility Turnier-Manager",
                ["WelcomeMessage"] = "Willkommen bei Agility Scoring!",
                ["WelcomeDescription"] = "Verwalten Sie Ihre Agility-Turniere und verfolgen Sie Leistungen",
                ["ViewTournaments"] = "Turniere anzeigen",
                ["AppFeatures"] = "App-Funktionen",
                ["FeatureManage"] = "Turniere verwalten",
                ["FeatureTrack"] = "Leistungen verfolgen",
                ["FeatureAnalyze"] = "Ergebnisse analysieren",
                ["PoweredBy"] = "Unterstützt von .NET MAUI",
                ["Tournaments"] = "Turniere",
                ["NoTournaments"] = "Noch keine Turniere. Erstellen Sie eines zum Starten!",
                ["AddTournament"] = "Turnier hinzufügen",
                ["TournamentName"] = "Turniername",
                ["TournamentNamePlaceholder"] = "Turniernamen eingeben",
                ["Date"] = "Datum",
                ["CreateTournament"] = "Turnier erstellen",
                ["Cancel"] = "Abbrechen",
                ["Settings"] = "Einstellungen",
                ["Language"] = "Sprache",
                ["SelectLanguage"] = "Sprache auswählen",
                ["ChangeLanguage"] = "Sprache ändern",
                ["Back"] = "Zurück",
                ["TournamentNameRequired"] = "Turniername ist erforderlich",
            },
            ["Nederlands"] = new()
            {
                ["AppTitle"] = "Agility Score App",
                ["AppSubtitle"] = "Agility Toernooi Manager",
                ["WelcomeMessage"] = "Welkom bij Agility Scoring!",
                ["WelcomeDescription"] = "Beheer uw agility toernooien en volg prestaties",
                ["ViewTournaments"] = "Toernooien bekijken",
                ["AppFeatures"] = "App functies",
                ["FeatureManage"] = "Toernooien beheren",
                ["FeatureTrack"] = "Prestaties bijhouden",
                ["FeatureAnalyze"] = "Resultaten analyseren",
                ["PoweredBy"] = "Aangedreven door .NET MAUI",
                ["Tournaments"] = "Toernooien",
                ["NoTournaments"] = "Nog geen toernooien. Maak er een aan om te beginnen!",
                ["AddTournament"] = "Toernooi toevoegen",
                ["TournamentName"] = "Toernooinaam",
                ["TournamentNamePlaceholder"] = "Voer toernooinaam in",
                ["Date"] = "Datum",
                ["CreateTournament"] = "Toernooi aanmaken",
                ["Cancel"] = "Annuleren",
                ["Settings"] = "Instellingen",
                ["Language"] = "Taal",
                ["SelectLanguage"] = "Kies taal",
                ["ChangeLanguage"] = "Taal wijzigen",
                ["Back"] = "Terug",
                ["TournamentNameRequired"] = "Toernooinaam is verplicht",
            }
        };

        public LocalizationService()
        {
            _currentLanguage = Preferences.Default.Get("language", "English");
        }

        public string this[string key]
        {
            get
            {
                if (_translations.TryGetValue(_currentLanguage, out var dict) && dict.TryGetValue(key, out var value))
                    return value;
                if (_translations["English"].TryGetValue(key, out var fallback))
                    return fallback;
                return key;
            }
        }

        public string CurrentLanguage => _currentLanguage;

        public void SetLanguage(string language)
        {
            if (_currentLanguage == language) return;
            _currentLanguage = language;
            Preferences.Default.Set("language", language);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
