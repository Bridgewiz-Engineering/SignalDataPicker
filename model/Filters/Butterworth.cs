using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal class Butterworth : FilterBase
    {
        public Butterworth(FilterConfigurationType filterConfigurationType, int samplingFrequency, int filterLength) : base(filterConfigurationType, samplingFrequency, filterLength)
        {
            FilterType = FilterType.Butterworth;
        }

        public override double CalculateGain(double frequency)
        {
            return FilterConfigurationType switch
            {
                FilterConfigurationType.LowPass => CalculateLowPassGain(frequency),
                FilterConfigurationType.HighPass => CalculateHighPassGain(frequency),
                FilterConfigurationType.BandPass => CalculateBandPassGain(frequency),
                FilterConfigurationType.BandStop => CalculateBandStopGain(frequency),
                _ => throw new NotImplementedException()
            };
        }

        private double CalculateLowPassGain(double frequency)
        {
            var order = FilterParameters[FilterParameterName.Order].Value;
            var cutoff = FilterParameters[FilterParameterName.Cutoff].Value;
            return Math.Round(1 / Math.Sqrt(1 + Math.Pow(frequency / cutoff, 2 * order)), 5);
        }
        private double CalculateHighPassGain(double frequency)
        {
            return 1 - CalculateLowPassGain(frequency);
        }
        private double CalculateBandPassGain(double frequency)
        {
            var order = FilterParameters[FilterParameterName.Order].Value;
            var cutoff1 = FilterParameters[FilterParameterName.Cutoff1].Value;
            var cutoff2 = FilterParameters[FilterParameterName.Cutoff2].Value;
            return Math.Round(Math.Sqrt(1 / (1 + Math.Pow(frequency / cutoff1, 2 * order)) * (1 / (1 + Math.Pow(cutoff2 / frequency, 2 * order)))), 5);
        }
        private double CalculateBandStopGain(double frequency)
        {
            return 1 - CalculateBandPassGain(frequency);
        }
    }
}
