using SignalDataPicker.model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SignalDataPicker.converter
{
    internal class FilterParameterTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FilterParameter filterParameter)
            {
                return $"{filterParameter.Name} = {filterParameter.Value}";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
