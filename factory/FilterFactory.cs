using SignalDataPicker.model;
using SignalDataPicker.model.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalDataPicker.factory
{
    internal class FilterFactory
    {
        public async Task<FilterBase> CreateFilterAsync(FilterType filterType, FilterConfigurationType filterConfigurationType, double[] frequencyAxis)
        {
            return await Task.Run(() => m_FilterCreators[filterType](filterConfigurationType, frequencyAxis));
            
        }
        private readonly Dictionary<FilterType, Func<FilterConfigurationType, double[], FilterBase>> m_FilterCreators = new()
        {
            { FilterType.NoFilter, (filterConfigurationType, frequencyAxis) => new NoFilter(filterConfigurationType, frequencyAxis) },
            { FilterType.Bessel, (filterConfigurationType, frequencyAxis) => new Bessel(filterConfigurationType, frequencyAxis) },
            { FilterType.Butterworth, (filterConfigurationType, frequencyAxis) => new Butterworth(filterConfigurationType, frequencyAxis) },
            { FilterType.Legendre, (filterConfigurationType, frequencyAxis) => new Legendre(filterConfigurationType, frequencyAxis) },
            { FilterType.Gaussian, (filterConfigurationType, frequencyAxis) => new Gaussian(filterConfigurationType, frequencyAxis) },
            { FilterType.Elliptic, (filterConfigurationType, frequencyAxis) => new Elliptic(filterConfigurationType, frequencyAxis) },
            { FilterType.Chebyshev, (filterConfigurationType, frequencyAxis) => new Chebyshev(filterConfigurationType, frequencyAxis) }
        };
    }
}

