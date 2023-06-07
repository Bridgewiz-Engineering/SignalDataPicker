using System;
using System.Collections.Generic;
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
        public abstract double CalculateGain(double frequency);
        #endregion

        #region Constructor
        protected FilterBase(FilterConfigurationType filterConfigurationType, int samplingFrequency, int filterLength)
        {
            FilterConfigurationType = filterConfigurationType;
            SamplingFrequency = samplingFrequency;
            FilterLength = filterLength;
            Gain = new double[FilterLength / 2, 2] ;
            InitializeParameters();
            Initialize();
        }
        #endregion

        #region Properties
        public FilterType FilterType { get; protected set; }
        public FilterConfigurationType FilterConfigurationType { get; private set; }
        public Dictionary<FilterParameterName, FilterParameter> FilterParameters { get; } = new();
        protected int FilterLength { get; private set; }
        protected int SamplingFrequency { get; private set; }
        public double[,] Gain { get; private set; }


        #endregion

        #region Virtual Methods
        private void Initialize()
        {

            var delta = SamplingFrequency / (double)FilterLength;
            for (int i = 0; i < FilterLength / 2; i++)
            {
                var frequency = i * delta;
                Gain[i, 0] = frequency;
                var g = CalculateGain(frequency);
                Gain[i, 1] = g;
            }
        }
        /// <summary>
        /// Add the default parameters
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected virtual void InitializeParameters()
        {
            FilterParameters[FilterParameterName.Order] = new FilterParameter(FilterParameterName.Order, 1, 1, 5, string.Empty);
            switch (FilterConfigurationType)
            {
                case FilterConfigurationType.LowPass:
                case FilterConfigurationType.HighPass:
                    FilterParameters[FilterParameterName.Cutoff] = new FilterParameter(FilterParameterName.Cutoff, 4d, 0, 100, "Hz");
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

        public async Task<double[,]> ApplyFilter(double[,] data)
        {
            var dataLength = data.Length / data.Rank;
            var result = new double[dataLength, 2];

            await Task.Run(() =>
            {
                for (var i = 0; i < dataLength; i++)
                {
                    result[i, 0] = data[i, 0];
                    result[i, 1] = data[i, 1] * CalculateGain(data[i, 0]);
                }
            });
            return result;
        }

        public void SetParameter(FilterParameterName parameterName, double value)
        {
            FilterParameters[parameterName].Value = value;
            Initialize();
        }
        #endregion

        #region Fields
        // Filter default values
        protected const int DefaultOrder = 1;
        protected const double DefaultCutoff = 1d;
        protected const double DefaultCutoff1 = 1d;
        protected const double DefaultCutoff2 = 10d;
        protected const double DefaultPassbandRipple = 0.1d;
        protected const double DefaultStopbandAttenuation = 60d;
        protected const double TransitionBandwidth = 1d;
        #endregion
    }
}
