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
        public static List<OutputType> OutputTypes { get => Enum.GetValues(typeof(OutputType)).Cast<OutputType>().ToList(); }
        public IAsyncRelayCommand LoadFileCommand { get => m_LoadFileCommand; }
        public IAsyncRelayCommand SaveFileCommand { get => m_SaveFileCommand; }
        public bool IsWorking { get => m_IsWorking; private set => SetProperty(ref m_IsWorking, value); }
        public FileType SelectedFileType { get => m_SelectedFileType; set => SetProperty(ref m_SelectedFileType, value); }
        public OutputType SelectedOutputType { get => m_SelectedOutputType; set => SetProperty(ref m_SelectedOutputType, value); }
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
            m_SaveFileCommand = new AsyncRelayCommand(SaveFileAsync, SaveFileAsyncCanExecute);

            m_AsyncRelayCommands = new IAsyncRelayCommand[] {  m_LoadFileCommand, m_SaveFileCommand };
            m_RelayCommands = Array.Empty<IRelayCommand>();
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
                EndIndex = m_FileData.AllData.Count;
                StartIndexMaximum = m_FileData.AllData.Count;
                EndIndexMaximum = m_FileData.AllData.Count;
                UpdateData();
            }
        }

        async private Task SaveFileAsync()
        {
            if (m_FileData == null || m_FileData.AllData.Count == 0) return;

            if (m_StartIndex > m_EndIndex)
            {
                m_WindowService.ShowErrorDialog("Başlangıç değeri bitiş değerinden büyük olamaz.");
                return;
            }
            if (m_StartIndex < 0 || m_EndIndex < 0)
            {
                m_WindowService.ShowErrorDialog("Başlangıç ve bitiş değerleri 0'dan küçük olamaz.");
                return;
            }

            m_FileData.FilteredData = m_SelectedAxis switch
            {
                DataAxis.X => m_FileData.AllData.GetRange(m_StartIndex - 1, m_EndIndex - m_StartIndex).Select(p => p.X).ToList(),
                DataAxis.Y => m_FileData.AllData.GetRange(m_StartIndex - 1, m_EndIndex - m_StartIndex).Select(p => p.Y).ToList(),
                DataAxis.Z => m_FileData.AllData.GetRange(m_StartIndex - 1, m_EndIndex - m_StartIndex).Select(p => p.Z).ToList(),
                _ => Array.Empty<double>().ToList()
            };

            var result = await m_FileService.SaveFile(m_FileData, m_SelectedOutputType, m_SelectedAxis, m_StartIndex, m_EndIndex);
            if (!result)
                m_WindowService.ShowErrorDialog("Dosya kaydedilemedi.");
        }
        #endregion


        #region Command States
        private bool LoadFileAsyncCanExecute()
        {
            return !m_IsWorking;
        }
        private bool SaveFileAsyncCanExecute()
        {
            return !m_IsWorking && m_FileData != null;
        }
        private void UpdateCommandStates()
        {
            foreach (var command in m_AsyncRelayCommands)
            {
                command.NotifyCanExecuteChanged();
            }
            foreach (var command in m_RelayCommands)
            {
                command.NotifyCanExecuteChanged();
            }
        }
        #endregion

        #region Private Methods

        private void UpdateData()
        {
            UpdateCommandStates();
            PlotActiveAxis();
            var axisData = SelectedAxis switch
            {
                DataAxis.X => m_FileData!.AllData.Select(p => p.X).ToList(),
                DataAxis.Y => m_FileData!.AllData.Select(p => p.Y).ToList(),
                DataAxis.Z => m_FileData!.AllData.Select(p => p.Z).ToList(),
                _ => Array.Empty<double>().ToList()
            };
            _ = Task.Run(async () => DataMetrics = await m_AnalysisService.CalculateDataMetrics(axisData, SelectedAxis));
        }
        private void PlotActiveAxis()
        {
            if (m_FileData == null || m_FileData.AllData.Count == 0) return;

            double[] data = m_SelectedAxis switch
            {
                DataAxis.X => m_FileData.AllData.Select(p => p.X).ToArray(),
                DataAxis.Y => m_FileData.AllData.Select(p => p.Y).ToArray(),
                DataAxis.Z => m_FileData.AllData.Select(p => p.Z).ToArray(),
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
        private IAsyncRelayCommand m_SaveFileCommand;

        private bool m_IsWorking = false;
        private FileData? m_FileData = null;
        private FileType m_SelectedFileType = FileType.LordAccelerometer;
        private OutputType m_SelectedOutputType = OutputType.SeismoSignal;
        private DataAxis m_SelectedAxis = DataAxis.X;
        private DataMetrics? m_DataMetrics = null;


        private int m_StartIndex = 1;
        private int m_EndIndex = 1;
        private int m_StartIndexMaximum = 1;
        private int m_EndIndexMaximum = 1;

        private readonly IAsyncRelayCommand[] m_AsyncRelayCommands;
        private readonly IRelayCommand[] m_RelayCommands;

        private ISeries[] m_PlotSeries;
        #endregion
    }
}
