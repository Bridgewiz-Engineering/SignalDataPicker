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

        public override async Task InitializeData()
        {
            await Task.Run(() =>
            {
                FilterData = new double[SamplingFrequency, 2];
                var cutoff = FilterParameters[0].Value;
                //TODO : implement Chebynshev filter
                var random = new Random();
                for (var i = 0; i < SamplingFrequency; i++)
                {
                    FilterData[i, 0] = i;
                    // for now create a cosine wave with noise
                    FilterData[i, 1] = Math.Cos(i * 2 * Math.PI * 10 / SamplingFrequency) + random.NextDouble() * 0.1;
                }
            });
        }
    }
}
