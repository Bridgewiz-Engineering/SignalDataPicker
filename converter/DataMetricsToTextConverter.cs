using System;
using System.Windows.Data;

namespace SignalDataPicker.converter
{
    class DataMetricsToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "Min: - Max: - Mean: - StdDev: - Variance: - RMS: -";
            var metrics = (model.DataMetrics)value;
            return $"Min: {metrics.Minimum:F5} Max: {metrics.Maximum:F5} Mean: {metrics.Mean:F5} StdDev: {metrics.StandardDeviation:F5} Variance: {metrics.Variance:F5} RMS: {metrics.RMS:F5}";
        }

        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

    }
}
