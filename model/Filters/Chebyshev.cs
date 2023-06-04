using System;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal class Chebyshev : FilterBase
    {
        public Chebyshev(FilterConfigurationType filterConfigurationType, int samplingFrequency) : base(filterConfigurationType, samplingFrequency)
        { 
            FilterType = FilterType.Chebyshev;
        }

        protected override Task<double[,]> InitializeBandPass()
        {
            throw new NotImplementedException();
        }

        protected override Task<double[,]> InitializeBandStop()
        {
            throw new NotImplementedException();
        }

        protected override Task<double[,]> InitializeHighPass()
        {
            throw new NotImplementedException();
        }

        protected override Task<double[,]> InitializeLowPass()
        {
            throw new NotImplementedException();
        }
    }
}
