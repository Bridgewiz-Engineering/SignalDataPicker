using System;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal class Legendre : FilterBase
    {
        public Legendre(FilterConfigurationType filterConfigurationType, int samplingFrequency) : base(filterConfigurationType, samplingFrequency)
        { 
            FilterType = FilterType.Legendre;
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
                    FilterData[i, 1] = Math.Exp(-Math.Pow(i, 2) / (2 * Math.Pow(cutoff, 2))) * Math.Pow(-1, 1) * Math.Pow(1 - Math.Pow(i, 2) / Math.Pow(cutoff, 2), 1 / 2) * Math.Pow(1 - Math.Pow(i, 2) / Math.Pow(cutoff, 2), 0.25);
                }
            });
        }
    }
}
