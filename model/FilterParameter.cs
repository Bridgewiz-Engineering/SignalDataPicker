using CommunityToolkit.Mvvm.ComponentModel;
using SignalDataPicker.model.Filters;

namespace SignalDataPicker.model
{
    internal class FilterParameter : ObservableObject
    {
        public FilterParameterName Name { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public string Unit { get; set; }
        public double Value { get => m_Value; set => SetProperty(ref m_Value, value); }

        public FilterParameter(FilterParameterName name, double value, double minimum, double maximum, string unit)
        {
            Name = name;
            m_Value = value;
            Minimum = minimum;
            Maximum = maximum;
            Unit = unit;

        }

        #region Fields
        private double m_Value;
        #endregion
    }
}
