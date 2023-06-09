namespace SignalDataPicker.model.Filters
{
    /// <summary>
    /// No filter. 
    /// Use the factory class <see cref="FilterFactory"/> to create a filter.
    /// </summary>
    internal class NoFilter : FilterBase
    {
        public NoFilter(FilterConfigurationType filterConfigurationType, double[] frequencyAxis) : base(filterConfigurationType, frequencyAxis) => FilterType = FilterType.NoFilter;

        protected override double CalculateBandPassGain(double frequency) => 1d;

        protected override double CalculateBandStopGain(double frequency) => 1d;

        protected override double CalculateHighPassGain(double frequency) => 1d;

        protected override double CalculateLowPassGain(double frequency) => 1d;
    }
}
