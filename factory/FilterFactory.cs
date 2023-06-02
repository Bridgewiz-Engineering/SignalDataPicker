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
                { FilterType.Butterworth, (filterConfigurationType) => new Butterworth(filterConfigurationType) }
            };
        }

        public IFilter CreateFilter(FilterType filterType, FilterConfigurationType filterConfigurationType)
        {
            return m_FilterCreators[filterType](filterConfigurationType);
        }
        #region Fields
        private readonly Dictionary<FilterType, Func<FilterConfigurationType, IFilter>> m_FilterCreators;
        #endregion
    }
}
