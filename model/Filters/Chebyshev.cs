using System;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal class Chebyshev : FilterBase
    {
        public Chebyshev(FilterConfigurationType filterConfigurationType, int samplingFrequency, int filterLength) : base(filterConfigurationType, samplingFrequency, filterLength)
        {
            FilterType = FilterType.Chebyshev;
        }

        public override double CalculateGain(double frequency)
        {
            throw new NotImplementedException();
        }
    }
}
