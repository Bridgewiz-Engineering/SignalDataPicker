using SignalDataPicker.model;

namespace SignalDataPicker.service
{
    internal interface IWindowService
    {
        void ShowErrorDialog(string message);
        void ShowProcessingWindow(FileData fileData);
        void ShowFilterPreviewWindow(double[,] filterData);
        void CloseFilterPreviewWindow();
        void CloseProcessingWindow();
        bool IsFilterPreviewWindowOpen();
        bool IsProcessingWindowOpen();
    }
}
