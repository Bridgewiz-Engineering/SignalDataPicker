using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SignalDataPicker.model;
using SignalDataPicker.service;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

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
        public IRelayCommand ResetBoundsCommand { get => m_ResetBoundsCommand; }
        public IRelayCommand ShowProcessingWindowCommand { get => m_ShowProcessingWindowCommand; }
        public bool IsWorking { get => m_IsWorking; private set { SetProperty(ref m_IsWorking, value); UpdateCommandStates(); } }
        public FileType SelectedFileType { get => m_SelectedFileType; set => SetProperty(ref m_SelectedFileType, value); }
        public OutputType SelectedOutputType { get => m_SelectedOutputType; set => SetProperty(ref m_SelectedOutputType, value); }
        public DataAxis SelectedAxis { get => m_SelectedAxis; set { SetProperty(ref m_SelectedAxis, value); UpdateData(); } }
        public FileData? FileData { get => m_FileData; private set => SetProperty(ref m_FileData, value); }
        public int StartIndex { get => m_StartIndex; set { SetProperty(ref m_StartIndex, value); RecalculateMetrics(); } }
        public int EndIndex { get => m_EndIndex; set { SetProperty(ref m_EndIndex, value); RecalculateMetrics(); } }
        public int StartIndexMaximum { get => m_StartIndexMaximum; private set => SetProperty(ref m_StartIndexMaximum, value); }
        public int EndIndexMaximum { get => m_EndIndexMaximum; private set => SetProperty(ref m_EndIndexMaximum, value); }
        public ISeries[] PlotSeries { get => m_PlotSeries; private set => SetProperty(ref m_PlotSeries, value); }
        public DataMetrics? DataMetrics { get => m_DataMetrics; private set => SetProperty(ref m_DataMetrics, value); }

        public ICartesianAxis[] PlotAxesX { get => m_PlotAxesX; private set => SetProperty(ref m_PlotAxesX, value); }

        #endregion

        #region Lifecycle
        public MainViewModel(IAnalysisService analysisService, IFileService fileService, IWindowService windowService)
        {
            m_AnalysisService = analysisService;
            m_FileService = fileService;
            m_WindowService = windowService;
            m_PlotSeries = Array.Empty<ISeries>();

            m_LoadFileCommand = new AsyncRelayCommand(LoadFileAsync, LoadFileAsyncCanExecute);
            m_SaveFileCommand = new AsyncRelayCommand(SaveFileAsync, SaveFileAsyncCanExecute);
            m_ResetBoundsCommand = new RelayCommand(ResetBounds, ResetBoundsCanExecute);
            m_ShowProcessingWindowCommand = new RelayCommand(ShowProcessingWindow, ShowProcessingWindowCanExecute);

            m_AsyncRelayCommands = new IAsyncRelayCommand[] { m_LoadFileCommand, m_SaveFileCommand };
            m_RelayCommands = new IRelayCommand[] { m_ResetBoundsCommand, m_ShowProcessingWindowCommand };

            var axis = new Axis();
            axis.PropertyChanged += Axis_PropertyChanged;

            m_PlotAxesX = new Axis[] { axis };

        }

        private void Axis_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Axis.MinLimit))
            {
                if (sender is Axis axis)
                {
                    var axisMin = axis.MinLimit;
                    if (axisMin <= 0) StartIndex = 1;
                    else if (axisMin > StartIndexMaximum) StartIndex = StartIndexMaximum;
                    else StartIndex = Convert.ToInt32(axisMin);
                }

            }
            else if (e.PropertyName == nameof(Axis.MaxLimit))
            {
                if (sender is Axis axis)
                {
                    var axisMax = axis.MaxLimit;
                    if (axisMax <= 0) EndIndex = EndIndexMaximum;
                    else if (axisMax > EndIndexMaximum) EndIndex = EndIndexMaximum;
                    else EndIndex = Convert.ToInt32(axisMax);
                }
            }

        }
        #endregion

        #region Command Handlers

        private void ShowProcessingWindow()
        {
            m_WindowService.ShowProcessingWindow(m_FileData!);
        }

        private void ResetBounds()
        {
            m_ShouldRecalculateMetrics = false; // recalculate metrics only once
            PlotAxesX[0].MinLimit = 1;
            m_ShouldRecalculateMetrics = true;
            PlotAxesX[0].MaxLimit = EndIndexMaximum;
        }
        async private Task LoadFileAsync()
        {
            IsWorking = true;
            FileData = await m_FileService.LoadFile(m_SelectedFileType);

            if (m_FileData == null)
            {
                m_WindowService.ShowErrorDialog("Dosya yüklenemedi.");
            }
            else
            {
                StartIndexMaximum = m_FileData.AllData.Count;
                EndIndexMaximum = m_FileData.AllData.Count;
                ResetBounds();
                UpdateData();
            }
            IsWorking = false;
        }

        async private Task SaveFileAsync()
        {
            IsWorking = true;
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

            var result = await m_FileService.SaveFile(m_FileData, m_SelectedOutputType, m_SelectedAxis, m_StartIndex, m_EndIndex);
            if (!result)
                m_WindowService.ShowErrorDialog("Dosya kaydedilemedi.");
            IsWorking = false;
        }
        #endregion

        #region Command States

        private bool ShowProcessingWindowCanExecute()
        {
            return !m_IsWorking && m_FileData != null && m_FileData.FilteredData.Count > 2;
        }

        private bool ResetBoundsCanExecute()
        {
            return !m_IsWorking && m_FileData != null;
        }
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
                App.Current.Dispatcher.Invoke(() => command.NotifyCanExecuteChanged());
            }
            foreach (var command in m_RelayCommands)
            {
                App.Current.Dispatcher.Invoke(() => command.NotifyCanExecuteChanged());
            }
        }
        #endregion

        #region Private Methods

        private void UpdateData()
        {
            UpdateCommandStates();
            PlotActiveAxis();
            _ = Task.Run(() => RecalculateMetrics());
        }
        private void RecalculateMetrics()
        {
            if (!m_ShouldRecalculateMetrics) return;

            IsWorking = true;
            if (m_FileData == null || m_FileData.AllData.Count == 0) DataMetrics = null;
            else if (m_StartIndex >= m_EndIndex) DataMetrics = null;
            else if (m_StartIndex < 0 || m_EndIndex < 0) DataMetrics = null;

            else
            {
                m_FileData.FilteredData = SelectedAxis switch
                {
                    DataAxis.X => m_FileData!.AllData.GetRange(m_StartIndex - 1, m_EndIndex - m_StartIndex).Select(p => p.X).ToList(),
                    DataAxis.Y => m_FileData!.AllData.GetRange(m_StartIndex - 1, m_EndIndex - m_StartIndex).Select(p => p.Y).ToList(),
                    DataAxis.Z => m_FileData!.AllData.GetRange(m_StartIndex - 1, m_EndIndex - m_StartIndex).Select(p => p.Z).ToList(),
                    _ => Array.Empty<double>().ToList()
                };
                _ = Task.Run(async () => DataMetrics = await m_AnalysisService.CalculateDataMetrics(m_FileData, SelectedAxis));
            }
            IsWorking = false;
        }
        private void PlotActiveAxis()
        {
            IsWorking = true;
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


            IsWorking = false;
        }
        #endregion

        #region Fields
        private readonly IAnalysisService m_AnalysisService;
        private readonly IFileService m_FileService;
        private readonly IWindowService m_WindowService;

        private readonly IAsyncRelayCommand m_LoadFileCommand;
        private readonly IAsyncRelayCommand m_SaveFileCommand;
        private readonly IRelayCommand m_ResetBoundsCommand;
        private readonly IRelayCommand m_ShowProcessingWindowCommand;

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
        private ICartesianAxis[] m_PlotAxesX;

        private bool m_ShouldRecalculateMetrics = true;
        #endregion
    }
}
