using NWaves.Transforms;
using SignalDataPicker.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalDataPicker.service.implementation
{
    internal class AnalysisService : IAnalysisService
    {
        public AnalysisService(ILogService logService)
        {
            m_LogService = logService;
        }

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

        public async Task<ProcessingResult> Process(FileData fileData)
        {
            var result = new ProcessingResult();
            try
            {
                if (fileData == null || fileData.FilteredData.Count < 2)
                    result.ErrorMessage = "İşlenecek veri yok";
                else
                {
                    var nearestPowerOfTwo = GetNextPowerOfTwo(fileData.FilteredData.Count);
                    var fft = new Fft(nearestPowerOfTwo);

                    float[] real = new float[nearestPowerOfTwo];
                    float[] imag = new float[nearestPowerOfTwo];

                    fileData.FilteredData.Select(q => Convert.ToSingle(q)).ToArray().CopyTo(real, 0);

                    await Task.Run(() => fft.Direct(real, imag));

                    List<double[]> fftResult = new();

                    var frequency = GenerateFrequencyAxis(nearestPowerOfTwo, fileData.SamplingFrequency);
                    // do not take first 5 data point (DC component)
                    // take only first half of the spectrum
                    for (int i = 0; i < nearestPowerOfTwo / 2; i++)
                    {
                        var point = new double[2];
                        point[0] = frequency[i];
                        point[1] = Math.Sqrt(real[i] * real[i] + imag[i] * imag[i]);
                        fftResult.Add(point);
                    }
                    result.FFTResult = fftResult;
                    result.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                m_LogService.LogError($"Error in processing result: {ex.Message}");
                result.ErrorMessage = $"Hata: {ex.Message}";
            }
            return result;
        }

        private static int GetNextPowerOfTwo(int n)
        {
            var k = 1;
            while (k < n)
                k <<= 1;
            return k;
        }

        private static double[] GenerateFrequencyAxis(int length, int samplingFrequency)
        {
            var frequencyAxis = new double[length];
            for (int i = 0; i < length; i++)
            {
                frequencyAxis[i] = i * (double)samplingFrequency / length;
            }
            return frequencyAxis;
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

        #region Fields
        private readonly ILogService m_LogService;
        #endregion
    }
}
