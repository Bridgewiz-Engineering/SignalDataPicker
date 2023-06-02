using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal class Butterworth : IFilter
    {
        public Butterworth(FilterConfigurationType filterConfiguration)
        {
            m_FilterConfigurationType = filterConfiguration;
            m_FilterParameters = new List<FilterParameter> { new FilterParameter("Order", 1d) };

            switch (filterConfiguration)
            {
                case FilterConfigurationType.LowPass:
                case FilterConfigurationType.HighPass:
                    m_FilterParameters.Add(new FilterParameter("Cutoff", 3.334d));
                    break;
                case FilterConfigurationType.BandPass:
                case FilterConfigurationType.BandStop:
                    m_FilterParameters.Add(new FilterParameter("Cutoff1", 3.1d));
                    m_FilterParameters.Add(new FilterParameter("Cutoff2", 3.4d));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterConfiguration), filterConfiguration, null);
            }
        }
        public FilterType FilterType { get => m_FilterType; }

        public FilterConfigurationType FilterConfigurationType { get => m_FilterConfigurationType; }

        public List<FilterParameter> FilterParameters { get => m_FilterParameters; }

        public Task<double[,]> CreateFilterData()
        {
            throw new NotImplementedException();
        }


        #region Fields
        private readonly FilterConfigurationType m_FilterConfigurationType;
        private readonly List<FilterParameter> m_FilterParameters;

        private readonly FilterType m_FilterType;
        #endregion
    }
}
