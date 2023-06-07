using System;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal class Elliptic : FilterBase
    {
        public Elliptic(FilterConfigurationType filterConfigurationType, int samplingFrequency, int filterLength) : base(filterConfigurationType, samplingFrequency, filterLength)
        {
            FilterType = FilterType.Elliptic;
        }
        public override double CalculateGain(double frequency)
        {
            throw new NotImplementedException();
        }
    }
}
