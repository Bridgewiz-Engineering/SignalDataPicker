using SignalDataPicker.model;
using SignalDataPicker.view;
using System.Windows;

namespace SignalDataPicker.service.implementation
{
    internal class WindowService : IWindowService
    {
        public void ShowErrorDialog(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowProcessingWindow(FileData fileData)
        {
            if (m_ProcessingWindow == null)
            {
                m_ProcessingWindow = new ProcessingWindow(fileData);
                m_ProcessingWindow.Closed += (sender, e) => m_ProcessingWindow = null;
                m_ProcessingWindow.Show();
            }
            else
            {
                m_ProcessingWindow.UpdateFileData(fileData);
            }
        }

        public bool IsProcessingWindowOpen()
        {
            return m_ProcessingWindow != null;
        }

        public void ShowFilterPreviewWindow(double[,] filterData)
        {
            m_FilterPreviewWindow = new FilterPreviewWindow(filterData);
            m_FilterPreviewWindow.Closed += (sender, e) => m_FilterPreviewWindow = null;
            m_FilterPreviewWindow.Show();
        }

        public void CloseFilterPreviewWindow()
        {
            m_FilterPreviewWindow?.Close();
        }

        public void CloseProcessingWindow()
        {
            m_ProcessingWindow?.Close();
        }

        public bool IsFilterPreviewWindowOpen()
        {
            return m_FilterPreviewWindow != null;
        }

        #region Fields
        private ProcessingWindow? m_ProcessingWindow;
        private FilterPreviewWindow? m_FilterPreviewWindow;
        #endregion
    }
}
