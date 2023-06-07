using SignalDataPicker.model;
using SignalDataPicker.model.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalDataPicker.factory
{
    internal class FilterFactory
    {
        public async Task<FilterBase> CreateFilterAsync(FilterType filterType, FilterConfigurationType filterConfigurationType, int samplingFrequency, int filterLength)
        {
            return await Task.Run(() => m_FilterCreators[filterType](filterConfigurationType, samplingFrequency, filterLength));
            
        }
        private readonly Dictionary<FilterType, Func<FilterConfigurationType, int, int, FilterBase>> m_FilterCreators = new()
        {
            { FilterType.NoFilter, (filterConfigurationType, samplingFrequency, filterLength) => new NoFilter(filterConfigurationType, samplingFrequency, filterLength) },
            { FilterType.Butterworth, (filterConfigurationType, samplingFrequency, filterLength) => new Butterworth(filterConfigurationType, samplingFrequency, filterLength) },
            { FilterType.Legendre, (filterConfigurationType, samplingFrequency, filterLength) => new Legendre(filterConfigurationType, samplingFrequency, filterLength) },
            { FilterType.Elliptic, (filterConfigurationType, samplingFrequency, filterLength) => new Elliptic(filterConfigurationType, samplingFrequency, filterLength) },
            { FilterType.Chebyshev, (filterConfigurationType, samplingFrequency, filterLength) => new Chebyshev(filterConfigurationType, samplingFrequency, filterLength) },
            { FilterType.Gaussian, (filterConfigurationType, samplingFrequency, filterLength) => new Gaussian(filterConfigurationType, samplingFrequency, filterLength) },
            { FilterType.Bessel, (filterConfigurationType, samplingFrequency, filterLength) => new Bessel(filterConfigurationType, samplingFrequency, filterLength) }
        };
    }
}

