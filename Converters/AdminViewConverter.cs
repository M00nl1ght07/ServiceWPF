using System;
using System.Globalization;
using System.Windows.Data;

namespace ServiceWPF.Converters
{
    public class AdminViewConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is string executorName && values[1] is string requestInfo)
            {
                return $"{executorName} - {requestInfo}";
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 