using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SignalDataPicker.factory;
using SignalDataPicker.model;
using SignalDataPicker.model.Filters;
using SignalDataPicker.service;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalDataPicker.viewmodel
{
    internal class ProcessingViewModel : ObservableObject
    {
        #region Properties

        // Combobox ItemsSource properties
        public static List<CorrectionType> CorrectionTypes { get => Enum.GetValues(typeof(CorrectionType)).Cast<CorrectionType>().ToList(); }
        public static List<FilterType> FilterTypes { get => Enum.GetValues(typeof(FilterType)).Cast<FilterType>().ToList(); }
        public static List<DataWindowType> DataWindowTypes { get => Enum.GetValues(typeof(DataWindowType)).Cast<DataWindowType>().ToList(); }
        public static List<FilterConfigurationType> FilterConfigurationTypes { get => Enum.GetValues(typeof(FilterConfigurationType)).Cast<FilterConfigurationType>().ToList(); }

        // Commands
        public IAsyncRelayCommand ProcessCommand { get => m_ProcessCommand; }
        public IAsyncRelayCommand ApplyFilterCommand { get => m_ApplyFilterCommand; }
        public IAsyncRelayCommand UpdateFilterCommand { get => m_UpdateFilterCommand; }
        public IAsyncRelayCommand RecalculateFilterCommand { get => m_RecalculateFilterCommand; }
        // Command Processing
        public FileData? FileData { get => m_FileData; private set => SetProperty(ref m_FileData, value); }
        public bool IsProcessing { get => m_IsProcessing; private set { SetProperty(ref m_IsProcessing, value); UpdateCommandStates(); } }

        // FFT Properties
        public double FFTMaxFrequency { get => m_FFTMaxFrequency; private set => SetProperty(ref m_FFTMaxFrequency, value); }
        public ISeries[] FFTSeries { get => m_FFTSeries; private set => SetProperty(ref m_FFTSeries, value); }
        public ICartesianAxis[] FFTAxesX { get => m_FFTAxesX; private set => SetProperty(ref m_FFTAxesX, value); }
        public ICartesianAxis[] FFTAxesY { get => m_FFTAxesY; private set => SetProperty(ref m_FFTAxesY, value); }
        public LabelVisual FFTTitle { get; } = new() { Text = "FFT", TextSize = 25, Padding = new LiveChartsCore.Drawing.Padding(15), Paint = new SolidColorPaint(SKColors.DarkSlateGray) };
        public int FFTDCCutoff { get => m_FFTDCCutoff; set { SetProperty(ref m_FFTDCCutoff, value); CutFFT(); } }

        // Filter Properties
        public FilterType SelectedFilterType { get => m_SelectedFilterType; set => SetProperty(ref m_SelectedFilterType, value); }
        public FilterBase? Filter { get => m_Filter; private set { SetProperty(ref m_Filter, value); UpdateCommandStates(); UpdateFilterChart(); } }
        public LabelVisual FilterTitle { get => m_FilterTitle; }
        public ISeries[] FilterSeries { get => m_FilterSeries; private set => SetProperty(ref m_FilterSeries, value); }
        public ICartesianAxis[] FilterAxesX { get => m_FilterAxesX; set => SetProperty(ref m_FilterAxesX, value); }
        public ICartesianAxis[] FilterAxesY { get => m_FilterAxesY; set => SetProperty(ref m_FilterAxesY, value); }
        public FilterConfigurationType SelectedFilterConfigurationType { get => m_SelectedFilterConfigurationType; set => SetProperty(ref m_SelectedFilterConfigurationType, value); }
        #endregion

        #region Public methods
        public void SetFileData(FileData fileData)
        {
            FileData = fileData;
            if (!m_IsFileDataLoaded)
            {
                FileData.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(FileData.FilteredData))
                    {
                        m_IsFileDataLoaded = true;
                        m_ProcessCommand.Execute(null);
                    }
                };
            }
        }
        #endregion

        #region Lifecycle
        public ProcessingViewModel(IAnalysisService analysisService, IWindowService windowService)
        {
            m_AnalysisService = analysisService;
            m_WindowService = windowService;

            m_ProcessCommand = new AsyncRelayCommand(Process, ProcessCanExecute);
            m_ApplyFilterCommand = new AsyncRelayCommand(ApplyFilter, ApplyFilterCanExecute);
            m_UpdateFilterCommand = new AsyncRelayCommand(InitializeFilter, InitializeFilterCanExecute);
            m_RecalculateFilterCommand = new AsyncRelayCommand(RecalculateFilter, RecalculateFilterCanExecute);

            m_Commands = Array.Empty<IAsyncRelayCommand>();
            m_AsyncCommands = new[] { m_ProcessCommand, m_ApplyFilterCommand, m_RecalculateFilterCommand, m_UpdateFilterCommand, m_RecalculateFilterCommand };

            m_FFTSeries = Array.Empty<ISeries>();

            m_FFTAxesX = new Axis[] { new() { Name = "Hz" } };
            m_FFTAxesY = new Axis[] { new() };
        }
        #endregion

        #region Command Handlers

        private async Task RecalculateFilter()
        {
            if (Filter != null)
            {
                IsProcessing = true;
                await Task.Run(() => Filter.OnPropertyChanged());
                IsProcessing = false;
                UpdateFilterChart();
            }
        }
        private async Task ApplyFilter()
        {
            try
            {
                double[] amplitudes = m_FFTPoints?.Select(q => q.Y ?? 1d).ToArray() ?? Array.Empty<double>();
                IsProcessing = true;
                double[]? filteredData = await Filter!.ApplyFilter(amplitudes);

                if (filteredData == null || filteredData.Length == 0)
                    m_WindowService.ShowErrorDialog("Hata!");
                else
                {
                    m_FilteredPoints = new List<ObservablePoint>(filteredData.Length);
                    await Task.Run(() =>
                    {
                        for (int i = 0; i < filteredData.Length; i++)
                            m_FilteredPoints.Add(new ObservablePoint(m_FFTPoints![i].X, filteredData[i]));
                    });
                    FFTSeries = new ISeries[]
                    {
                        new LineSeries<ObservablePoint>
                        {
                            Fill = null,
                            Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 1},
                            GeometryFill = null,
                            GeometryStroke = null,
                            LineSmoothness = 0,
                            Values = m_FilteredPoints,
                            TooltipLabelFormatter = (chartPoint) => $"{chartPoint.Model?.X} - {chartPoint.Model?.Y}"
                        }
                    };
                    m_IsFilterApplied = true;
                    ResetFFTAxes();
                    CutFFT();
                }
            }
            catch (ArgumentException ex)
            {
                m_WindowService.ShowErrorDialog(ex.Message);
            }
            IsProcessing = false;
        }

        private void UpdateFilterChart()
        {
            if (Filter?.Gain?.Length > 0)
            {

                var chartPoints = new List<ObservablePoint>();
                var chartData = Filter!.Gain!;
                for (int i = 0; i < chartData.GetLength(0); i++)
                {
                    chartPoints.Add(new ObservablePoint(chartData[i, 0], chartData[i, 1]));
                }

                FilterSeries = new ISeries[]
                {
                    new LineSeries<ObservablePoint>
                    {
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 1},
                        GeometryFill = null,
                        GeometryStroke = null,
                        LineSmoothness = 0,
                        Values = chartPoints,
                        TooltipLabelFormatter = (chartPoint) => $"{chartPoint.Model?.X} - {chartPoint.Model?.Y}"
                    }
                };
                ResetFilterAxies();
            }
        }

        private async Task Process()
        {
            m_IsFilterApplied = false;
            IsProcessing = true;
            var data = await m_AnalysisService.Process(FileData!, new());
            if (data.IsSuccess)
            {
                m_FFTPoints = data.FFTResult.ConvertAll(q => new ObservablePoint(q[0], q[1]));

                FFTSeries = new ISeries[]
                {
                    new LineSeries<ObservablePoint>
                    {
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 1},
                        GeometryFill = null,
                        GeometryStroke = null,
                        LineSmoothness = 0,
                        TooltipLabelFormatter = (chartPoint) => $"{chartPoint.Model?.X}"
                    }
                };
                ResetFFTAxes();
                CutFFT();
            }
            else
            {
                FFTSeries = Array.Empty<ISeries>();
                m_WindowService.ShowErrorDialog(data.ErrorMessage);
            }

            await InitializeFilter();

            IsProcessing = false;
        }

        #endregion

        #region Command States
        private bool RecalculateFilterCanExecute()
        {
            return !IsProcessing && Filter != null;
        }
        private bool ProcessCanExecute()
        {
            return !IsProcessing;
        }
        private bool ApplyFilterCanExecute()
        {
            return !IsProcessing && Filter != null;
        }
        private void UpdateCommandStates()
        {
            foreach (var command in m_Commands)
                command.NotifyCanExecuteChanged();
            foreach (var command in m_AsyncCommands)
                command.NotifyCanExecuteChanged();
        }
        private bool InitializeFilterCanExecute()
        {
            return !IsProcessing;
        }
        #endregion

        #region Private methods

        private void CutFFT()
        {
            List<ObservablePoint>? cutData;
            if (m_IsFilterApplied)
                cutData = m_FilteredPoints?.Where(p => p.X > FFTDCCutoff).ToList();
            else
                cutData = m_FFTPoints?.Where(p => p.X > FFTDCCutoff).ToList();

            var maxAmp = cutData?.Max(p => p.Y);

            FFTMaxFrequency = cutData?.Find(q => q.Y == maxAmp)?.X ?? -1;
            FFTSeries[0].Values = cutData;
        }
        private void ResetFFTAxes()
        {
            m_FFTAxesX[0].MinLimit = null;
            m_FFTAxesX[0].MaxLimit = null;
            m_FFTAxesY[0].MinLimit = null;
            m_FFTAxesY[0].MaxLimit = null;
        }
        private void ResetFilterAxies()
        {
            m_FilterAxesX[0].MinLimit = null;
            m_FilterAxesX[0].MaxLimit = null;
            m_FilterAxesY[0].MinLimit = null;
            m_FilterAxesY[0].MaxLimit = null;
        }
        private async Task InitializeFilter()
        {
            try
            {
                var freqAxis = m_FFTPoints?.Select(q => q.X ?? 1d).ToArray();
                IsProcessing = true;
                Filter = await m_FilterFactory.CreateFilterAsync(m_SelectedFilterType, m_SelectedFilterConfigurationType, freqAxis ?? Array.Empty<double>());
                UpdateFilterChart();
            }
            catch (NotImplementedException)
            {
                m_WindowService.ShowErrorDialog("Bu filtre henüz eklenmemiştir.");
            }
            finally
            {
                IsProcessing = false;
            }
        }
        #endregion

        #region Fields

        private FileData? m_FileData;
        private bool m_IsProcessing;
        private bool m_IsFileDataLoaded;
        private ICartesianAxis[] m_FFTAxesX;
        private ICartesianAxis[] m_FFTAxesY;
        private ISeries[] m_FFTSeries;
        private readonly IAnalysisService m_AnalysisService;
        private readonly IWindowService m_WindowService;
        private readonly IAsyncRelayCommand m_ProcessCommand;
        private readonly IAsyncRelayCommand m_ApplyFilterCommand;
        private readonly IAsyncRelayCommand m_UpdateFilterCommand;
        private readonly IAsyncRelayCommand m_RecalculateFilterCommand;
        private readonly IRelayCommand[] m_Commands;
        private readonly IAsyncRelayCommand[] m_AsyncCommands;
        private int m_FFTDCCutoff = 1;
        private double m_FFTMaxFrequency;
        private List<ObservablePoint>? m_FFTPoints;

        private FilterBase? m_Filter;
        private FilterType m_SelectedFilterType = FilterType.NoFilter;
        private FilterConfigurationType m_SelectedFilterConfigurationType = FilterConfigurationType.LowPass;
        private LabelVisual m_FilterTitle = new() { Text = "Filter", TextSize = 12, Padding = new LiveChartsCore.Drawing.Padding(10), Paint = new SolidColorPaint(SKColors.DarkSlateGray) };
        private ISeries[] m_FilterSeries = Array.Empty<ISeries>();
        private ICartesianAxis[] m_FilterAxesX = new Axis[] { new() };
        private ICartesianAxis[] m_FilterAxesY = new Axis[] { new() };
        private List<ObservablePoint>? m_FilteredPoints;
        private bool m_IsFilterApplied;

        private readonly FilterFactory m_FilterFactory = new();



        #endregion
    }
}
