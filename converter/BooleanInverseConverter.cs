using System;
using System.Globalization;
using System.Windows.Data;

namespace SignalDataPicker.converter
{
    internal class BooleanInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            else throw new InvalidOperationException("BooleanInverseConverter can only be used with a boolean value.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("BooleanInverseConverter can only be used OneWay.");
        }
    }
}
