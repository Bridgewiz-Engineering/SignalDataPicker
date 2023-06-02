using CommunityToolkit.Mvvm.ComponentModel;

namespace SignalDataPicker.model
{
    internal class FilterParameter : ObservableObject
    {
        public string Name { get; set; } = string.Empty;
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public string Unit { get; set; }
        public double Value { get => m_Value; set => SetProperty(ref m_Value, value); }

        public FilterParameter(string name, double value, double minimum, double maximum, string unit)
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
