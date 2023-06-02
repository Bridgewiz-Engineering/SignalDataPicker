using SignalDataPicker.model;
using SignalDataPicker.model.Filters;

namespace SignalDataPicker.service
{
    internal interface IWindowService
    {
        void ShowErrorDialog(string message);
        void ShowProcessingWindow(FileData fileData);
        void ShowFilterPreviewWindow(FilterBase filter);
        void CloseFilterPreviewWindow();
        void CloseProcessingWindow();
        bool IsFilterPreviewWindowOpen();
        bool IsProcessingWindowOpen();
    }
}
