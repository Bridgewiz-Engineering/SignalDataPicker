using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalDataPicker.model.Filters
{
    internal abstract class FilterBase
    {
        #region Abstract Methods
        protected abstract Task<double[,]> InitializeLowPass();
        protected abstract Task<double[,]> InitializeHighPass();
        protected abstract Task<double[,]> InitializeBandPass();
        protected abstract Task<double[,]> InitializeBandStop();
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
        public List<FilterParameter> FilterParameters { get; private set; } = new();
        public double[,] FilterData { get; protected set; } = new double[0, 0];
        public int SamplingFrequency { get; private set; }
        public bool IsFilterDataInitialized => FilterData.Length > 0;
        #endregion

        #region Virtual Methods
        public virtual async Task InitializeData()
        {
            switch (FilterConfigurationType)
            {
                case FilterConfigurationType.LowPass:
                    FilterData = await InitializeLowPass();
                    break;
                case FilterConfigurationType.HighPass:
                    FilterData = await InitializeHighPass();
                    break;
                case FilterConfigurationType.BandPass:
                    FilterData = await InitializeBandPass();
                    break;
                case FilterConfigurationType.BandStop:
                    FilterData = await InitializeBandStop();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        protected virtual void InitializeParameters()
        {
            switch (FilterConfigurationType)
            {
                case FilterConfigurationType.LowPass:
                case FilterConfigurationType.HighPass:
                    FilterParameters.Add(new FilterParameter("Cutoff", 1d, 0, 100, "Hz"));
                    break;
                case FilterConfigurationType.BandPass:
                case FilterConfigurationType.BandStop:
                    FilterParameters.Add(new FilterParameter("Cutoff1", 1d, 0, 100, "Hz"));
                    FilterParameters.Add(new FilterParameter("Cutoff2", 10d, 0, 100, "Hz"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion
    }
}
