using System.Collections.Generic;

namespace SignalDataPicker.model
{
    internal class FileData
    {
        public string FileName { get; set; } = string.Empty;
        public List<Record> AllData { get; set; } = new List<Record>();

        public List<double> FilteredData { get; set; } = new List<double>();
    }
}
