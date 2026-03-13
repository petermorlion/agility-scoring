using AgilityScoring.Maui.Services;
using System.ComponentModel;
using System.Windows.Input;

namespace AgilityScoring.Maui.ViewModels
{
    public class ObstacleRowViewModel : INotifyPropertyChanged
    {
        private readonly LocalizationService _locService;
        private readonly string _locKey;
        private readonly Func<Task> _onChanged;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Key { get; }

        public string DisplayName => _locService[_locKey];

        private int _refusals;
        public int Refusals
        {
            get => _refusals;
            private set { _refusals = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Refusals))); }
        }

        private int _faults;
        public int Faults
        {
            get => _faults;
            private set { _faults = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Faults))); }
        }

        public ICommand IncrementRefusalsCommand { get; }
        public ICommand DecrementRefusalsCommand { get; }
        public ICommand IncrementFaultsCommand { get; }
        public ICommand DecrementFaultsCommand { get; }

        public ObstacleRowViewModel(string key, string locKey, LocalizationService locService, Func<Task> onChanged)
        {
            Key = key;
            _locKey = locKey;
            _locService = locService;
            _onChanged = onChanged;

            locService.PropertyChanged += (_, _) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));

            IncrementRefusalsCommand = new Command(async () => { Refusals++; await _onChanged(); });
            DecrementRefusalsCommand = new Command(async () => { if (Refusals > 0) { Refusals--; await _onChanged(); } });
            IncrementFaultsCommand = new Command(async () => { Faults++; await _onChanged(); });
            DecrementFaultsCommand = new Command(async () => { if (Faults > 0) { Faults--; await _onChanged(); } });
        }

        public void SetScores(int refusals, int faults)
        {
            _refusals = refusals;
            _faults = faults;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Refusals)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Faults)));
        }
    }
}
