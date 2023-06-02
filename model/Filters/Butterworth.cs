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
            m_FilterParameters = new List<FilterParameter> { new FilterParameter("Order", 1, 1, 4, string.Empty) };

            switch (filterConfiguration)
            {
                case FilterConfigurationType.LowPass:
                case FilterConfigurationType.HighPass:
                    m_FilterParameters.Add(new FilterParameter("Cutoff", 1d, 0, 100, "Hz"));
                    break;
                case FilterConfigurationType.BandPass:
                case FilterConfigurationType.BandStop:
                    m_FilterParameters.Add(new FilterParameter("Cutoff1", 1d, 0, 100, "Hz"));
                    m_FilterParameters.Add(new FilterParameter("Cutoff2", 10d, 0, 100, "Hz"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterConfiguration), filterConfiguration, null);
            }
        }
        public FilterType FilterType { get => m_FilterType; }

        public FilterConfigurationType FilterConfigurationType { get => m_FilterConfigurationType; }

        public List<FilterParameter> FilterParameters { get => m_FilterParameters; }

        public async Task<double[,]> CreateFilterData(int samplingFrequency)
        {
            // TODO Cache filter data for performance
            return await Task.Run(() =>
            {
                var filterData = new double[samplingFrequency, 2];
                //  TODO Implement filter data creation - check
                //  https://filtering.mathdotnet.com/api/MathNet.Filtering.Butterworth/Designer.htm
                //  https://github.com/ar1st0crat/NWaves#filters-and-effects
                // for now return random data
                var random = new Random();
                for (var i = 0; i < samplingFrequency; i++)
                {
                    filterData[i, 0] = random.NextDouble();
                    filterData[i, 1] = random.NextDouble();
                }
                return filterData;
            });
        }


        #region Fields
        private readonly FilterConfigurationType m_FilterConfigurationType;
        private readonly List<FilterParameter> m_FilterParameters;

        private readonly FilterType m_FilterType;
        #endregion
    }
}
