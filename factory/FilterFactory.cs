using SignalDataPicker.model;
using SignalDataPicker.model.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalDataPicker.factory
{
    internal class FilterFactory
    {
        public async Task<FilterBase> CreateFilterAsync(FilterType filterType, FilterConfigurationType filterConfigurationType, int samplingFrequency)
        {
            var f = await Task.Run(() => m_FilterCreators[filterType](filterConfigurationType, samplingFrequency));
            return f;
        }
        private readonly Dictionary<FilterType, Func<FilterConfigurationType, int, FilterBase>> m_FilterCreators = new()
        {
            { FilterType.NoFilter,  (filterConfigurationType, samplingFrequency) => new NoFilter(filterConfigurationType, samplingFrequency) },
            { FilterType.Elliptic, (filterConfigurationType, samplingFrequency) => new Elliptic(filterConfigurationType, samplingFrequency) },
            { FilterType.Legendre, (filterConfigurationType, samplingFrequency) => new Legendre(filterConfigurationType, samplingFrequency) },
            { FilterType.Gaussian, (filterConfigurationType, samplingFrequency) => new Gaussian(filterConfigurationType, samplingFrequency) },
            { FilterType.Butterworth, (filterConfigurationType, samplingFrequency) => new Butterworth(filterConfigurationType, samplingFrequency) },
            { FilterType.Chebyshev, (filterConfigurationType, samplingFrequency) => new Chebyshev(filterConfigurationType, samplingFrequency) },
            { FilterType.Bessel, (filterConfigurationType, samplingFrequency) => new Bessel(filterConfigurationType, samplingFrequency) }
        };
    }
}

