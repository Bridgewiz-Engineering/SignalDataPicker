using System;
using System.Globalization;
using System.Windows.Data;

namespace SignalDataPicker.converter
{
    internal class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value != null) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("NullToVisibilityConverter can only be used OneWay.");
        }
    }
}
