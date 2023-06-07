namespace SignalDataPicker.model.Filters
{
    /// <summary>
    /// No filter. 
    /// Use the factory class <see cref="FilterFactory"/> to create a filter.
    /// </summary>
    internal class NoFilter : FilterBase
    {
        public NoFilter(FilterConfigurationType filterConfigurationType, int samplingFrequency, int filterLength) : base(filterConfigurationType, samplingFrequency, filterLength) => FilterType = FilterType.NoFilter;
        public override double CalculateGain(double frequency)
        {
            return 1d;
        }
    }
}
