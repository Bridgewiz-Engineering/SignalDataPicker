using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal class Gaussian : FilterBase
    {
        public Gaussian(FilterConfigurationType filterConfigurationType, int samplingFrequency, int filterLength) : base(filterConfigurationType, samplingFrequency, filterLength)
        {
            FilterType = FilterType.Gaussian;
        }
        public override double CalculateGain(double frequency)
        {
            throw new NotImplementedException();
        }
    }
}
