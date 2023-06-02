namespace SignalDataPicker.model
{
    internal class ProcessingOptions
    {
        public ICorrection? Correction { get; set; }
        public IFilter? Filter { get; set; }
        public DataWindowType DataWindowTypes { get; set; } = DataWindowType.None;
    }
}
