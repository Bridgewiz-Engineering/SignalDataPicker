using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SignalDataPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {


        int res_decimal = 5;

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
                CalculateMetrics();
            }
        }
        private void CalculateMetrics()
        {
            if (Records.Count == 0) return;


            var filteredRecords = FilterRecords();
            var selectedData = new List<double>();

            if (filteredRecords.Count == 0)
            {
                txtMin.Text = "-";
                txtMax.Text = "-";
                txtMean.Text = "-";
                txtStd.Text = "-";
                txtRMS.Text = "-";
                return;
            }

            switch (cmbDirection.SelectedIndex)
            {
                case -1:
                default:
                    break;

                case 0:
                    selectedData = filteredRecords.ConvertAll(p => p.X);
                    break;
                case 1:
                    selectedData = filteredRecords.ConvertAll((p) => p.Y);
                    break;
                case 2:
                    selectedData = filteredRecords.ConvertAll(p => p.Z);
                    break;

            }

            txtMin.Text = Math.Round(selectedData.Min(), res_decimal).ToString();
            txtMax.Text = Math.Round(selectedData.Max(), res_decimal).ToString();
            txtMean.Text = Math.Round(selectedData.Average(), res_decimal).ToString();
            txtStd.Text = Math.Round(CalculateStDev(selectedData), res_decimal).ToString();
            txtRMS.Text = Math.Round(CalculateRMS(selectedData, cmbDirection.SelectedIndex), res_decimal).ToString();
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
                            X = Convert.ToDouble(lineArray[2], CultureInfo.InvariantCulture),
                            Y = Convert.ToDouble(lineArray[3], CultureInfo.InvariantCulture),
                            Z = Convert.ToDouble(lineArray[4], CultureInfo.InvariantCulture)
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
            if (Records.Count == 0) return;
            
            var currentFolder = Path.GetDirectoryName(txtFileName.Text) ?? string.Empty;
            var currentFileName = Path.GetFileNameWithoutExtension(txtFileName.Text);
            string targetFileName;

            var filteredRecords = FilterRecords();
            if (filteredRecords.Count == 0) return;

            CalculateMetrics();

            var sfd = new SaveFileDialog()
            {
                AddExtension = true,
                InitialDirectory = currentFolder,
                FileName = $"{currentFileName}-{axes[cmbDirection.SelectedIndex]}-{txtStartIndex.Value}-{txtEndIndex.Value}.csv",
                DefaultExt = ".csv",
                Filter = "CSV|*.csv|TXT|*.txt|Hepsi|*.*"



            };
            if (sfd.ShowDialog() == true)
            {

                targetFileName = sfd.FileName;
                

                switch (cmbDirection.SelectedIndex)
                {
                    case -1:
                    default:
                    case 0:
                        {
                            File.WriteAllLines(targetFileName, filteredRecords.ConvertAll(q => q.X.ToString(CultureInfo.InvariantCulture)));
                            break;
                        }
                    case 1:
                        {
                            File.WriteAllLines(targetFileName, filteredRecords.ConvertAll(q => q.Y.ToString(CultureInfo.InvariantCulture)));
                            break;
                        }
                    case 2:
                        {
                            File.WriteAllLines(targetFileName, filteredRecords.ConvertAll(q => q.Z.ToString(CultureInfo.InvariantCulture)));
                            break;
                        }

                }
            }
        }

      

        private double CalculateRMS(List<double> selectedData, int selectedIndex)
        {
            var a = (selectedIndex == 2 ? 1 : 0);
            return Math.Sqrt(selectedData.Average(q => Math.Pow(q + a,2))); 
        }
        

        private List<Record> FilterRecords()
        {
            btnSave.Focus();
            var records = new List<Record>();
            var startIndex = Convert.ToInt32(txtStartIndex.Value == null ? 1 : txtStartIndex.Value);
            var endIndex = Convert.ToInt32(txtEndIndex.Value == null ? Records.Count - 1 : txtEndIndex.Value);

  
            if (startIndex >= endIndex)
            {
                MessageBox.Show("Son değer ilk değerden küçük olmalıdır.");
                txtStartIndex.Value = 1;
                txtEndIndex.Value = Records.Count - 1;
            }
            else if (endIndex < 1 || startIndex < 1)
            {
                MessageBox.Show("Girdiler 1'den büyük olmalıdır.");
                txtStartIndex.Value = txtStartIndex.Value < 1 ? 1 : txtStartIndex.Value;
                txtEndIndex.Value = txtEndIndex.Value < 1 ? Records.Count - 1 : txtEndIndex.Value;
            }
            else
            {
                records = Records.GetRange(startIndex - 1, endIndex - startIndex);
            }

            return records;
        }

        private double CalculateStDev(List<double> selectedData)
        {
            double res = 0;

            if (selectedData.Count > 1) {
                double mean = selectedData.Average();
                double sum = selectedData.Sum(q => Math.Pow(q-mean,2));
                res = Math.Sqrt(sum / selectedData.Count);
            }
            return res;
        }

       



        private void txtStartIndex_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                CalculateMetrics();
            }
        }
    }
}
