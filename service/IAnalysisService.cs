using SignalDataPicker.model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalDataPicker.service
{
    internal interface IAnalysisService
    {
        Task<DataMetrics> CalculateDataMetrics(List<double> data, DataAxis axis);
    }
}
