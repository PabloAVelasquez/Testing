using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace BnPBank.Utilities
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool bValue && bValue)
            {
                return new SolidColorBrush(Colors.Red);
            }

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
