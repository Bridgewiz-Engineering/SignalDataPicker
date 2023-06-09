using System;

namespace SignalDataPicker.model.Filters
{
    internal class Legendre : FilterBase
    {
        public Legendre(FilterConfigurationType filterConfigurationType, double[] frequencyAxis) : base(filterConfigurationType, frequencyAxis) => FilterType = FilterType.Legendre;

        protected override double CalculateBandPassGain(double frequency) => throw new NotImplementedException();

        protected override double CalculateBandStopGain(double frequency) => throw new NotImplementedException();

        protected override double CalculateHighPassGain(double frequency) => throw new NotImplementedException();

        protected override double CalculateLowPassGain(double frequency) => throw new NotImplementedException();
    }
}
