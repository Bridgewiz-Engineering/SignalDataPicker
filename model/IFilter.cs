using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalDataPicker.model
{
    internal interface IFilter
    {
        FilterType FilterType { get; }
        FilterConfigurationType FilterConfiguration { get; }
        List<FilterParameter> FilterParameters { get; }
        Task<double[,]> CreateFilterData();
    }
}
