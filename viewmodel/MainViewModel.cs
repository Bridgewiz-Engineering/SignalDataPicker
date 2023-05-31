using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SignalDataPicker.model;
using SignalDataPicker.service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace SignalDataPicker.viewmodel
{
    internal class MainViewModel : ObservableObject
    {
        #region Properties
        public static List<FileType> FileTypes { get => Enum.GetValues(typeof(FileType)).Cast<FileType>().ToList(); }
        public static List<DataAxis> Axes { get => Enum.GetValues(typeof(DataAxis)).Cast<DataAxis>().ToList(); }
        public IAsyncRelayCommand LoadFileCommand { get => m_LoadFileCommand; }
        public bool IsWorking { get => m_IsWorking; private set => SetProperty(ref m_IsWorking, value); }
        public FileType SelectedFileType { get => m_SelectedFileType; set => SetProperty(ref m_SelectedFileType, value); }
        public DataAxis SelectedAxis { get => m_SelectedAxis; set { SetProperty(ref m_SelectedAxis, value); UpdateData(); } }
        public FileData? FileData { get => m_FileData; private set => SetProperty(ref m_FileData, value); }
        public int StartIndex { get => m_StartIndex; set => SetProperty(ref m_StartIndex, value); }
        public int EndIndex { get => m_EndIndex; set => SetProperty(ref m_EndIndex, value); }
        public int StartIndexMaximum { get => m_StartIndexMaximum; private set => SetProperty(ref m_StartIndexMaximum, value); }
        public int EndIndexMaximum { get => m_EndIndexMaximum; private set => SetProperty(ref m_EndIndexMaximum, value); }
        public ISeries[] PlotSeries { get => m_PlotSeries; private set => SetProperty(ref m_PlotSeries, value); }
        public DataMetrics? DataMetrics { get => m_DataMetrics; private set => SetProperty(ref m_DataMetrics, value); }

        #endregion

        public MainViewModel(IAnalysisService analysisService, IFileService fileService, ILogService logService, IWindowService windowService)
        {
            m_AnalysisService = analysisService;
            m_FileService = fileService;
            m_LogService = logService;
            m_WindowService = windowService;
            m_PlotSeries = Array.Empty<ISeries>();

            m_LoadFileCommand = new AsyncRelayCommand(LoadFileAsync, LoadFileAsyncCanExecute);
        }


        #region Command Handlers
        async private Task LoadFileAsync()
        {
            FileData = await m_FileService.LoadFile(m_SelectedFileType);

            if (m_FileData == null)
            {
                m_WindowService.ShowErrorDialog("Dosya yüklenemedi.");
            }
            else
            {
                EndIndex = m_FileData.Data.Count;
                StartIndexMaximum = m_FileData.Data.Count;
                EndIndexMaximum = m_FileData.Data.Count;
                UpdateData();
            }

        }
        #endregion


        #region Command States
        private bool LoadFileAsyncCanExecute()
        {
            return !m_IsWorking;
        }
        #endregion

        #region Private Methods

        private void UpdateData()
        {
            PlotActiveAxis();
            var axisData = SelectedAxis switch
            {
                DataAxis.X => m_FileData!.Data.Select(p => p.X).ToList(),
                DataAxis.Y => m_FileData!.Data.Select(p => p.Y).ToList(),
                DataAxis.Z => m_FileData!.Data.Select(p => p.Z).ToList(),
                _ => Array.Empty<double>().ToList()
            };
            _ = Task.Run(async () => DataMetrics = await m_AnalysisService.CalculateDataMetrics(axisData, SelectedAxis));
        }
        private void PlotActiveAxis()
        {
            if (m_FileData == null || m_FileData.Data.Count == 0) return;

            double[] data = m_SelectedAxis switch
            {
                DataAxis.X => m_FileData.Data.Select(p => p.X).ToArray(),
                DataAxis.Y => m_FileData.Data.Select(p => p.Y).ToArray(),
                DataAxis.Z => m_FileData.Data.Select(p => p.Z).ToArray(),
                _ => Array.Empty<double>()
            };

            PlotSeries = new ISeries[]
            {
                new LineSeries<double>()
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

        }
        #endregion


        #region Fields
        private readonly IAnalysisService m_AnalysisService;
        private readonly IFileService m_FileService;
        private readonly ILogService m_LogService;
        private readonly IWindowService m_WindowService;


        private IAsyncRelayCommand m_LoadFileCommand;

        private bool m_IsWorking = false;
        private FileData? m_FileData = null;
        private FileType m_SelectedFileType = FileType.LordAccelerometer;
        private DataAxis m_SelectedAxis = DataAxis.X;
        private DataMetrics? m_DataMetrics = null;


        private int m_StartIndex = 1;
        private int m_EndIndex = 1;
        private int m_StartIndexMaximum = 1;
        private int m_EndIndexMaximum = 1;

        private ISeries[] m_PlotSeries;
        #endregion
    }
}
