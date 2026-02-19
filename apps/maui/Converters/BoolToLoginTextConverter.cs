using System.Globalization;

namespace AgilityScoring.Maui.Converters
{
    public class BoolToLoginTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAuthenticated)
            {
                return isAuthenticated ? "View Your Tournaments" : "Login to Continue";
            }
            return "Login to Continue";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}