using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathNet.Filtering.FIR;

namespace SignalDataPicker.model.Filters
{
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
        public FilterBase(FilterConfigurationType filterConfigurationType, int samplingFrequency)
        {
            FilterConfigurationType = filterConfigurationType;
            SamplingFrequency = samplingFrequency;
            InitializeParameters();
        }
        #endregion

        #region Properties
        public FilterType FilterType { get; protected set; }
        public FilterConfigurationType FilterConfigurationType { get; private set; }
        public Dictionary<FilterParameterName, FilterParameter> FilterParameters { get; private set; } = new();
        protected (double[] numerator, double[] denominator) Coefficients { get; set; } = (Array.Empty<double>(), Array.Empty<double>());
        public double[,] FilterSampleData { get; protected set; } = new double[0, 0];
        public int SamplingFrequency { get; private set; }
        public bool IsDataInitialized => FilterSampleData.Length > 0;

        protected const int DefaultOrder = 1;
        protected const double DefaultCutoff = 1d;
        protected const double DefaultCutoff1 = 1d;
        protected const double DefaultCutoff2 = 10d;
        protected const double DefaultPassbandRipple = 0.1d;
        protected const double DefaultStopbandAttenuation = 60d;
        protected const double TransitionBandwidth = 1d;

        #endregion

        #region Virtual Methods
        public virtual async Task InitializeData()
        {
            switch (FilterConfigurationType)
            {
                case FilterConfigurationType.LowPass: await InitializeLowPass(); break;
                case FilterConfigurationType.HighPass: await InitializeHighPass(); break;
                case FilterConfigurationType.BandPass: await InitializeBandPass(); break;
                case FilterConfigurationType.BandStop: await InitializeBandStop(); break;
                default: throw new NotImplementedException();
            };
        }
        protected virtual void InitializeParameters()
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
        }
        public virtual async Task<double[,]> ApplyFilter()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
