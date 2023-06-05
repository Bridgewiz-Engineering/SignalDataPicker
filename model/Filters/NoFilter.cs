using System;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    /// <summary>
    /// No filter. 
    /// Use the factory class <see cref="FilterFactory"/> to create a filter.
    /// </summary>
    internal class NoFilter : FilterBase
    {
        public NoFilter(FilterConfigurationType filterConfigurationType, int samplingFrequency) : base(filterConfigurationType, samplingFrequency)
        {
            FilterType = FilterType.NoFilter;
        }

        protected override async Task InitializeBandPass()
        {
            await Task.Run(() =>
            {
                FilterCoefficients = new double[SamplingFrequency];
                Array.Fill(FilterCoefficients, 1.0);
            });
        }

        protected override async Task InitializeBandStop()
        {
            await Task.Run(() =>
            {
                FilterCoefficients = new double[SamplingFrequency];
                Array.Fill(FilterCoefficients, 1.0);
            });
        }

        protected override async Task InitializeHighPass()
        {
            await Task.Run(() =>
            {
                FilterCoefficients = new double[SamplingFrequency];
                Array.Fill(FilterCoefficients, 1.0);
            });
        }

        protected override async Task InitializeLowPass()
        {
            await Task.Run(() =>
            {
                FilterCoefficients = new double[SamplingFrequency];
                Array.Fill(FilterCoefficients, 1.0);
            });
        }
    }
}
