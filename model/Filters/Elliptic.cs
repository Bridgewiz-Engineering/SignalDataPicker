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

        public override async Task InitializeData()
        {
            await Task.Run(() =>
            {
                FilterData = new double[SamplingFrequency, 2];
                var cutoff = FilterParameters[0].Value;
                var random = new Random();
                for (var i = 0; i < SamplingFrequency; i++)
                {
                    FilterData[i, 0] = i;
                    FilterData[i, 1] = Math.Exp(-Math.Pow(i, 2) / (2 * Math.Pow(cutoff, 2))) * Math.Sin(2 * Math.PI * cutoff * i / SamplingFrequency);
                }
            });
        }
    }
}
