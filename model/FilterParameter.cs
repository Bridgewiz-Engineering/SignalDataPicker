using CommunityToolkit.Mvvm.ComponentModel;

namespace SignalDataPicker.model
{
    internal class FilterParameter : ObservableObject
    {
        public string Name { get; set; } = string.Empty;
        public double Value { get => m_Value; set => SetProperty(ref m_Value, value); }

        public FilterParameter(string name, double value)
        {
            Name = name;
            m_Value = value;
        }

        #region Fields
        private double m_Value;
        #endregion
    }
}
