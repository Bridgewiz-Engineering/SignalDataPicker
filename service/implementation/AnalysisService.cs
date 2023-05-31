using SignalDataPicker.model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SignalDataPicker.service.implementation
{
    internal class AnalysisService : IAnalysisService
    {
        async public Task<DataMetrics> CalculateDataMetrics(List<double> data, DataAxis axis)
        {
            return await Task.Run(() =>
            {
                var metrics = new DataMetrics
                {
                    Minimum = data.Min(),
                    Maximum = data.Max(),
                    Mean = data.Average(),
                };
                metrics.StandardDeviation = StdDev(data, metrics.Mean);
                metrics.Variance = metrics.StandardDeviation * metrics.StandardDeviation;
                metrics.RMS = RMS(data, axis);
                return metrics;
            });
        }

        private static double StdDev(IEnumerable<double> values, double average)
        {
            var avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }

        private static double RMS(IEnumerable<double> values, DataAxis axis)
        {
            var a = (axis == DataAxis.Z ? 1 : 0);
            return Math.Sqrt(values.Average(q => Math.Pow(q + a, 2)));
        }
    }
}
