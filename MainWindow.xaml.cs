using SignalDataPicker.viewmodel;

namespace SignalDataPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = App.Current.ServiceProvider.GetService(typeof(MainViewModel));
        }
    }
}
