using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SignalDataPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public List<Record> Records { get; set; } = new List<Record>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "CSV|*.csv|TXT|*.txt|Hepsi|*.*";
            if (ofd.ShowDialog() == true)
            {
                txtFileName.Text = ofd.FileName;
                TryReadFile(ofd.FileName);
                txtEndIndex.Value = Records.Count;
                txtEndIndex.Maximum = Records.Count;
            }
        }

        private void TryReadFile(string fileName)
        {
            Records.Clear();
            var allData = File.ReadAllLines(fileName);
            for (int i = 0; i < allData.Length; i++)
            {
                var line = allData[i];
                var lineArray = line.Split(',');
                if (lineArray.Length >= 5)
                {
                    Records.Add(new Record
                    {
                        TimeStamp = lineArray[0],
                        Index = Convert.ToInt32(lineArray[1]),
                        X = Convert.ToDouble(lineArray[2]),
                        Y = Convert.ToDouble(lineArray[3]),
                        Z = Convert.ToDouble(lineArray[4])
                    });
                }
            }
        }
    }
}
