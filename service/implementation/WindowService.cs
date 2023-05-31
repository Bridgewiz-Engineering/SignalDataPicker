using System.Windows;

namespace SignalDataPicker.service.implementation
{
    internal class WindowService : IWindowService
    {
        public void ShowErrorDialog(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
