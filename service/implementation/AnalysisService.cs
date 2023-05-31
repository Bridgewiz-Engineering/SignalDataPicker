using SignalDataPicker.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalDataPicker.service.implementation
{
    internal class AnalysisService : IAnalysisService
    {
        async public Task<DataMetrics> CalculateDataMetrics(FileData fileData, DataAxis axis)
        {
            return await Task.Run(() =>
            {
                var metrics = new DataMetrics
                {
                    Minimum = fileData.FilteredData.Min(),
                    Maximum = fileData.FilteredData.Max(),
                    Mean = fileData.FilteredData.Average(),
                };
                metrics.StandardDeviation = StdDev(fileData.FilteredData, metrics.Mean);
                metrics.Variance = metrics.StandardDeviation * metrics.StandardDeviation;
                metrics.RMS = RMS(fileData.FilteredData, axis);
                return metrics;
            });
        }

        private static double StdDev(IEnumerable<double> values, double average)
        {
            return Math.Sqrt(values.Average(v => Math.Pow(v - average, 2)));
        }

        private static double RMS(IEnumerable<double> values, DataAxis axis)
        {
            var a = (axis == DataAxis.Z ? 1 : 0);
            return Math.Sqrt(values.Average(q => Math.Pow(q + a, 2)));
        }
    }
}
