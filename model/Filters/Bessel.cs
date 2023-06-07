using System;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal class Bessel : FilterBase
    {
        public Bessel(FilterConfigurationType filterConfigurationType, int samplingFrequency, int filterLength) : base(filterConfigurationType, samplingFrequency, filterLength)
        {
            FilterType = FilterType.Bessel;
        }

        public override double CalculateGain(double frequency)
        {
            throw new NotImplementedException();
        }
    }
}
