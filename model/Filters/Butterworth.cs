using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal class Butterworth : IFilter
    {
        public Butterworth(FilterConfigurationType filterConfiguration)
        {
            m_FilterConfiguration = filterConfiguration;
            m_FilterParameters = new List<FilterParameter> { new FilterParameter("Order", 1d) };

            switch (filterConfiguration)
            {
                case FilterConfigurationType.LowPass:
                case FilterConfigurationType.HighPass:
                    m_FilterParameters.Add(new FilterParameter("Cutoff", 1d));
                    break;
                case FilterConfigurationType.BandPass:
                case FilterConfigurationType.BandStop:
                    m_FilterParameters.Add(new FilterParameter("Cutoff1", 1d));
                    m_FilterParameters.Add(new FilterParameter("Cutoff2", 1d));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterConfiguration), filterConfiguration, null);
            }
        }
        public FilterType FilterType => FilterType.Butterworth;

        public FilterConfigurationType FilterConfiguration => m_FilterConfiguration;

        public List<FilterParameter> FilterParameters => m_FilterParameters;

        public Task<double[,]> CreateFilterData()
        {
            throw new NotImplementedException();
        }


        #region Fields
        private readonly FilterConfigurationType m_FilterConfiguration;
        private readonly List<FilterParameter> m_FilterParameters;
        #endregion
    }
}
