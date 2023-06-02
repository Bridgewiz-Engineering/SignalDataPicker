using SignalDataPicker.model.Filters;

namespace SignalDataPicker.model
{
    internal class ProcessingOptions
    {
        public ICorrection? Correction { get; set; }
        public FilterBase? Filter { get; set; }
        public DataWindowType DataWindowTypes { get; set; } = DataWindowType.None;
    }
}
