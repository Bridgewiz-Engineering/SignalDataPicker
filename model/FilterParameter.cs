namespace SignalDataPicker.model
{
    internal record FilterParameter
    {
        public string Name { get; set; } = string.Empty;
        public double Value { get; set; }

        public FilterParameter(string name, double value)
        {
            Name = name;
            Value = value;
        }
    }
}
