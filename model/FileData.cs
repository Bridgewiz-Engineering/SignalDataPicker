using System.Collections.Generic;

namespace SignalDataPicker.model
{
    internal class FileData
    {
        public string FileName { get; set; } = string.Empty;
        public List<Record> Data { get; set; } = new List<Record>();
    }
}
