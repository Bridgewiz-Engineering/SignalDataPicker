using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace SignalDataPicker.model
{
    internal class FileData : ObservableObject
    {
        public string FileName { get; set; } = string.Empty;
        public List<Record> AllData { get; set; } = new List<Record>();
        public List<double> FilteredData { get => m_FilteredData; set => SetProperty(ref m_FilteredData, value); }
        public int SamplingFrequency { get; set; } = 128;

        private List<double> m_FilteredData = new();
    }
}
