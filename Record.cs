using System;

namespace SignalDataPicker
{
    public class Record
    {
        public string TimeStamp { get; set; } = String.Empty;
        public int Index { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
