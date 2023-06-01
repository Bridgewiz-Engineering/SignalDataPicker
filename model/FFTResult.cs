using System.Collections.Generic;

namespace SignalDataPicker.model
{
    internal class FFTResult
    {
        public bool IsSuccess { get; set; }
        public List<DataPoint> Data { get; set; } = new List<DataPoint>();
        public string? ErrorMessage { get; set; }
    }
}
