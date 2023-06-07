using System;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal class Legendre : FilterBase
    {
        public Legendre(FilterConfigurationType filterConfigurationType, int samplingFrequency, int filterLength) : base(filterConfigurationType, samplingFrequency, filterLength)
        {
            FilterType = FilterType.Legendre;
        }

        public override double CalculateGain(double frequency)
        {
            throw new NotImplementedException();
        }
    }
}
