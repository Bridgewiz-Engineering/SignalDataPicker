using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal class Gaussian : FilterBase
    {
        public Gaussian(FilterConfigurationType filterConfigurationType, int samplingFrequency) : base(filterConfigurationType, samplingFrequency)
        { 
            FilterType = FilterType.Gaussian;
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
