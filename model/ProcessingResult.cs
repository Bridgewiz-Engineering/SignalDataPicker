using System.Collections.Generic;

namespace SignalDataPicker.model
{
    internal class ProcessingResult
    {
        public List<double[]> FFTResult { get; set; } = new List<double[]>();
        public string ErrorMessage { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
    }
}
