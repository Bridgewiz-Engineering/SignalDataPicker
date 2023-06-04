using System;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal class Elliptic : FilterBase
    {
        public Elliptic(FilterConfigurationType filterConfigurationType, int samplingFrequency) : base(filterConfigurationType, samplingFrequency)
        { 
            FilterType = FilterType.Elliptic;
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
