using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    /// <summary>
    /// The base class for all filters.
    /// Use the factory class <see cref="FilterFactory"/> to create a filter.
    /// </summary>
    internal abstract class FilterBase
    {
        #region Abstract Methods
        /// <summary>
        /// Initialize the filter coefficients and the sample data for the low pass filter
        /// </summary>
        protected abstract Task InitializeLowPass();
        /// <summary>
        /// Initialize the filter coefficients and the sample data for the high pass filter
        /// </summary>
        protected abstract Task InitializeHighPass();
        /// <summary>
        /// Initialize the filter coefficients and the sample data for the band pass filter
        /// </summary>
        protected abstract Task InitializeBandPass();
        /// <summary>
        /// Initialize the filter coefficients and the sample data for the band stop filter
        /// </summary>       
        protected abstract Task InitializeBandStop();
        #endregion

        #region Constructors
        protected FilterBase(FilterConfigurationType filterConfigurationType, int samplingFrequency)
        {
            FilterConfigurationType = filterConfigurationType;
            SamplingFrequency = samplingFrequency;
            Initialize();
        }
        #endregion

        #region Properties
        public FilterType FilterType { get; protected set; }
        public FilterConfigurationType FilterConfigurationType { get; private set; }
        public Dictionary<FilterParameterName, FilterParameter> FilterParameters { get; } = new();
        public double[,] FilterSampleData { get => m_FilterSampleData; private set => m_FilterSampleData = value; }
        public int SamplingFrequency { get; private set; }
        protected double[] FilterCoefficients { get; set; } = Array.Empty<double>();


        protected const int DefaultOrder = 1;
        protected const double DefaultCutoff = 1d;
        protected const double DefaultCutoff1 = 1d;
        protected const double DefaultCutoff2 = 10d;
        protected const double DefaultPassbandRipple = 0.1d;
        protected const double DefaultStopbandAttenuation = 60d;
        protected const double TransitionBandwidth = 1d;

        #endregion

        #region Virtual Methods
        /// <summary>
        /// This method should be called after the filter has been initialized.
        /// </summary>
        private void Initialize()
        {
            InitializeParameters();

            switch (FilterConfigurationType)
            {
                case FilterConfigurationType.LowPass: InitializeLowPass(); break;
                case FilterConfigurationType.HighPass: InitializeHighPass(); break;
                case FilterConfigurationType.BandPass: InitializeBandPass(); break;
                case FilterConfigurationType.BandStop: InitializeBandStop(); break;
            };

            InitializeFilterSampleData();
        }
        private void InitializeParameters()
        {
            FilterParameters[FilterParameterName.Cutoff] = new FilterParameter(FilterParameterName.Order, 1, 1, 5, string.Empty);
            switch (FilterConfigurationType)
            {
                case FilterConfigurationType.LowPass:
                case FilterConfigurationType.HighPass:
                    FilterParameters[FilterParameterName.Cutoff] = new FilterParameter(FilterParameterName.Cutoff, 1d, 0, 100, "Hz");
                    break;
                case FilterConfigurationType.BandPass:
                case FilterConfigurationType.BandStop:
                    FilterParameters[FilterParameterName.Cutoff1] = new FilterParameter(FilterParameterName.Cutoff1, 1d, 0, 100, "Hz");
                    FilterParameters[FilterParameterName.Cutoff2] = new FilterParameter(FilterParameterName.Cutoff2, 10d, 0, 100, "Hz");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            m_SampleFrequencyAxis = new double[Convert.ToInt32(SamplingFrequency / m_SampleFrequencyDelta / 2)];

            Parallel.For(0, m_SampleFrequencyAxis.Length, i => m_SampleFrequencyAxis[i] = i * m_SampleFrequencyDelta);
        }
        private void InitializeFilterSampleData()
        {
            // TODO: Apply filter to sample data
            m_FilterSampleData = new double[m_SampleFrequencyAxis.Length, 2];
            Parallel.For(0, m_SampleFrequencyAxis.Length, i => {
                m_FilterSampleData[i, 0] = m_SampleFrequencyAxis[i];
                m_FilterSampleData[i, 1] = 1;
            });
        }
        public virtual async Task<double[,]> ApplyFilter()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Fields
        private double[,] m_FilterSampleData = new double[0, 0];
        private double[] m_SampleFrequencyAxis = Array.Empty<double>();
        private const double m_SampleFrequencyDelta = 0.1;
        #endregion
    }
}
