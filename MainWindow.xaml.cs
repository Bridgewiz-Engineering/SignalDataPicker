using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Win32;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SignalDataPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public List<Record> Records { get; set; } = new List<Record>();
        private readonly string[] axes = {"X", "Y","Z"};
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "CSV|*.csv|TXT|*.txt|Hepsi|*.*";
            if (ofd.ShowDialog() == true)
            {
                txtFileName.Text = ofd.FileName;
                TryReadFile(ofd.FileName);
                txtStartIndex.Value = 1;
                txtStartIndex.Maximum = Records.Count;
                txtEndIndex.Value = Records.Count;
                txtEndIndex.Maximum = Records.Count;
                PlotActiveAxis();
            }
        }

        private void PlotActiveAxis()
        {
            if (Records.Count == 0) return;
            double[] data;
            switch (cmbDirection.SelectedIndex)
            {
                case -1:
                default:
                case 0: data = Records.ConvertAll(q => q.X).ToArray(); break;
                case 1: data = Records.ConvertAll(q => q.Y).ToArray(); break;
                case 2: data = Records.ConvertAll(q => q.Z).ToArray(); break;
            }
            var series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Fill = null,
                    Values = data,
                    Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 1},
                    GeometryFill = null,
                    GeometryStroke = null,
                    LineSmoothness = 0,
                    TooltipLabelFormatter = (chartPoint) => $"{chartPoint.Context.Entity.EntityIndex}"
                  
                }
            };
            lvcChart.Series = series;   
        }

        private void TryReadFile(string fileName)
        {
            try
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
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void cmbDirection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PlotActiveAxis();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var startIndex = Convert.ToInt32(txtStartIndex.Value);
            var endIndex = Convert.ToInt32(txtEndIndex.Value);
            if (startIndex >=endIndex)
            {
                MessageBox.Show("Son değer ilk değerden küçük olmalıdır.");
                return;
            }

            var currentFolder = Path.GetDirectoryName(txtFileName.Text) ?? string.Empty;
            var currentFileName = Path.GetFileNameWithoutExtension(txtFileName.Text);
            string targetFileName;
            var sfd = new SaveFileDialog()
            {
                AddExtension = true,
                InitialDirectory = currentFolder,
                FileName = $"{currentFileName}-{axes[cmbDirection.SelectedIndex]}-{startIndex}-{endIndex}.csv",
                DefaultExt = ".csv",
                Filter = "CSV|*.csv|TXT|*.txt|Hepsi|*.*"



            };
            if (sfd.ShowDialog() == true)
            {
                targetFileName = sfd.FileName;
                var filteredRecords = Records.GetRange(startIndex - 1, endIndex); // it gets startIndex, count so we do not subtract 1 from endIndex
                switch (cmbDirection.SelectedIndex)
                {
                    case -1:
                    default:
                    case 0:
                        {
                            File.WriteAllLines(targetFileName, filteredRecords.ConvertAll(q => q.X.ToString()));
                            break;
                        }
                    case 1:
                        {
                            File.WriteAllLines(targetFileName, filteredRecords.ConvertAll(q => q.Y.ToString()));
                            break;
                        }
                    case 2:
                        {
                            File.WriteAllLines(targetFileName, filteredRecords.ConvertAll(q => q.Z.ToString()));
                            break;
                        }

                }
            }
        }
    }
}
