using System.Globalization;

namespace AgilityScoring.Maui.Converters
{
    public class BoolToDisqualifiedColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDisqualified)
            {
                return isDisqualified 
                    ? Color.FromArgb("#dc3545") // Red when disqualified
                    : Color.FromArgb("#6c757d"); // Gray when not disqualified
            }
            return Color.FromArgb("#6c757d");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
