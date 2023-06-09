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
        protected abstract double CalculateLowPassGain(double frequency);
        protected abstract double CalculateHighPassGain(double frequency);
        protected abstract double CalculateBandPassGain(double frequency);
        protected abstract double CalculateBandStopGain(double frequency);
        #endregion

        #region Constructor
        protected FilterBase(FilterConfigurationType filterConfigurationType, double[] frequencyAxis)
        {
            FilterConfigurationType = filterConfigurationType;
            FrequencyAxis = frequencyAxis;
            Gain = new double[frequencyAxis.Length, 2];
            InitializeParameters();
            Initialize();
        }
        #endregion

        #region Properties
        public FilterType FilterType { get; protected set; }
        public FilterConfigurationType FilterConfigurationType { get; private set; }
        public Dictionary<FilterParameterName, FilterParameter> FilterParameters { get; } = new();
        protected double[] FrequencyAxis { get; private set; }
        public double[,] Gain { get; private set; }
        #endregion

        #region Virtual Methods
        private void Initialize()
        {
            for (int i = 0; i < FrequencyAxis.Length; i++)
            {
                var f = FrequencyAxis[i];
                Gain[i, 0] = f;
                Gain[i, 1] = CalculateGain(f);
            }
        }
        /// <summary>
        /// Add the default parameters
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected virtual void InitializeParameters()
        {
            FilterParameters[FilterParameterName.Order] = new FilterParameter(FilterParameterName.Order, 1, 1, 15, string.Empty);
            switch (FilterConfigurationType)
            {
                case FilterConfigurationType.LowPass:
                case FilterConfigurationType.HighPass:
                    FilterParameters[FilterParameterName.Cutoff] = new FilterParameter(FilterParameterName.Cutoff, 4d, 1, 100, "Hz");
                    break;
                case FilterConfigurationType.BandPass:
                case FilterConfigurationType.BandStop:
                    FilterParameters[FilterParameterName.Cutoff1] = new FilterParameter(FilterParameterName.Cutoff1, 1d, 1, 100, "Hz");
                    FilterParameters[FilterParameterName.Cutoff2] = new FilterParameter(FilterParameterName.Cutoff2, 10d, 1, 100, "Hz");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();

            }
        }

        public virtual double CalculateGain(double frequency)
        {
            return FilterConfigurationType switch
            {
                FilterConfigurationType.LowPass => CalculateLowPassGain(frequency),
                FilterConfigurationType.HighPass => CalculateHighPassGain(frequency),
                FilterConfigurationType.BandPass => CalculateBandPassGain(frequency),
                FilterConfigurationType.BandStop => CalculateBandStopGain(frequency),
                _ => throw new NotImplementedException()
            };
        }

        public async Task<double[]> ApplyFilter(double[] data)
        {
            if (data.Length != FrequencyAxis.Length)
                throw new ArgumentException("The data length must be equal to the frequency axis length.");

            var result = new double[data.Length];
            await Task.Run(() =>
            {
                for (int i = 0; i < data.Length; i++)
                    result[i] = data[i] * Gain[i, 1];
            });
            return result;
        }

        public void OnPropertyChanged() => Initialize();
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
