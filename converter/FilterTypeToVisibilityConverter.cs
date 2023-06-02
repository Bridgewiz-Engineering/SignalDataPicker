using System;
using System.Globalization;
using System.Windows.Data;

namespace SignalDataPicker.converter
{
    internal class FilterTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((model.FilterType)value == model.FilterType.None) ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("FilterTypeToVisibilityConverter can only be used OneWay.");
        }
    }
}
