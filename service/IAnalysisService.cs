using SignalDataPicker.model;
using System.Threading.Tasks;

namespace SignalDataPicker.service
{
    internal interface IAnalysisService
    {
        Task<DataMetrics> CalculateDataMetrics(FileData fileData, DataAxis axis);
        Task<ProcessingResult> Process(FileData fileData, ProcessingOptions processingOptions);
    }
}
