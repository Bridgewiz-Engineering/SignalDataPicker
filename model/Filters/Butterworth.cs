using System;

namespace SignalDataPicker.model.Filters
{
    internal class Butterworth : FilterBase
    {
        public Butterworth(FilterConfigurationType filterConfigurationType, double[] frequencyAxis) : base(filterConfigurationType, frequencyAxis) => FilterType = FilterType.Butterworth;

        protected override double CalculateLowPassGain(double frequency)
        {
            var order = FilterParameters[FilterParameterName.Order].Value;
            var cutoff = FilterParameters[FilterParameterName.Cutoff].Value;
            return 1 / Math.Sqrt(1 + Math.Pow(frequency / cutoff, 2 * order));
        }
        protected override double CalculateBandPassGain(double frequency)
        {
            var order = FilterParameters[FilterParameterName.Order].Value;
            var cutoff1 = FilterParameters[FilterParameterName.Cutoff1].Value;
            var cutoff2 = FilterParameters[FilterParameterName.Cutoff2].Value;

            return 1 / Math.Sqrt(1 + Math.Pow((frequency - cutoff2) / cutoff1, order * 2));
            
        }
        protected override double CalculateBandStopGain(double frequency) => 1 - CalculateBandPassGain(frequency);
        protected override double CalculateHighPassGain(double frequency) => 1 - CalculateLowPassGain(frequency);
    }
}
