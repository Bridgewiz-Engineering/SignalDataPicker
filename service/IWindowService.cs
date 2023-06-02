using SignalDataPicker.model;

namespace SignalDataPicker.service
{
    internal interface IWindowService
    {
        void ShowErrorDialog(string message);

        void ShowProcessingWindow(FileData fileData);
        bool IsProcessingWindowOpen();
    }
}
