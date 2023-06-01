using SignalDataPicker.model;
using SignalDataPicker.viewmodel;
using System.Windows;

namespace SignalDataPicker.view
{
    /// <summary>
    /// Interaction logic for ProcessingWindow.xaml
    /// </summary>
    public partial class ProcessingWindow
    {
        internal ProcessingWindow(FileData fileData)
        {
            var viewModel = App.Current.ServiceProvider.GetService(typeof(ProcessingViewModel));
            if (viewModel is ProcessingViewModel processingViewModel)
            {
                processingViewModel.SetFileData(fileData);
                this.DataContext = processingViewModel;
            }
            InitializeComponent();
        }

        internal void UpdateFileData(FileData fileData)
        {
            ((ProcessingViewModel)this.DataContext).SetFileData(fileData);
        }
    }
}
