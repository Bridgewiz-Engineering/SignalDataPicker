using SignalDataPicker.model;
using SignalDataPicker.model.Filters;
using System;
using System.Collections.Generic;

namespace SignalDataPicker.factory
{
    internal class FilterFactory
    {
        public FilterFactory()
        {
            m_FilterCreators = new Dictionary<FilterType, Func<FilterConfigurationType, IFilter>>
            {
                { FilterType.Butterworth, (filterConfiguration) => new Butterworth(filterConfiguration) }
            };
        }

        public IFilter CreateFilter(FilterType filterType, FilterConfigurationType filterConfiguration)
        {
            return m_FilterCreators[filterType](filterConfiguration);
        }
        #region Fields
        private readonly Dictionary<FilterType, Func<FilterConfigurationType, IFilter>> m_FilterCreators;
        #endregion
    }
}
