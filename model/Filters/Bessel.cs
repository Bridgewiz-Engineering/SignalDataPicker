using System;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal class Bessel : FilterBase
    {
        public Bessel(FilterConfigurationType filterConfigurationType, int samplingFrequency) : base(filterConfigurationType, samplingFrequency)
        {
            FilterType = FilterType.Bessel;
        }

        protected override Task InitializeBandPass()
        {
            throw new NotImplementedException();
        }

        protected override Task InitializeBandStop()
        {
            throw new NotImplementedException();
        }

        protected override Task InitializeHighPass()
        {
            throw new NotImplementedException();
        }

        protected override Task InitializeLowPass()
        {
            throw new NotImplementedException();
        }
    }
}
