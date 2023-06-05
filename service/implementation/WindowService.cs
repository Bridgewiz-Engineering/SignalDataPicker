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
                m_ProcessingWindow.UpdateFileData(fileData);
        }

        public bool IsProcessingWindowOpen()
        {
            return m_ProcessingWindow != null;
        }

        public void CloseProcessingWindow()
        {
            m_ProcessingWindow?.Close();
        }

        public void ToggleFilterPreview()
        {
            if (m_ProcessingWindow == null) return;

            if (m_ProcessingWindow.Width == m_ProcessingWindowWidthWithPreview)
                m_ProcessingWindow.Width = m_ProcessingWindowWidthNoPreview;
            else
                m_ProcessingWindow.Width = m_ProcessingWindowWidthWithPreview;
        }

        public bool IsFilterPreviewVisible() => m_ProcessingWindow?.Width == m_ProcessingWindowWidthWithPreview;

        #region Fields
        private ProcessingWindow? m_ProcessingWindow;
        private const int m_ProcessingWindowWidthNoPreview = 400;
        private const int m_ProcessingWindowWidthWithPreview = 800;
        #endregion
    }
}
