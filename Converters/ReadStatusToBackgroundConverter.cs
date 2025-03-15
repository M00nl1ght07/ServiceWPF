using System;
using System.Windows.Data;
using System.Windows.Media;

namespace ServiceWPF.Converters
{
    public class ReadStatusToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isRead = (bool)value;
            return isRead ? new SolidColorBrush(Colors.Transparent) : new SolidColorBrush(Color.FromRgb(232, 245, 253));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}