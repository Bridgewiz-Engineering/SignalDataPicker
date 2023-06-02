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
using SignalDataPicker.service;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SignalDataPicker.viewmodel
{
    internal class ProcessingViewModel : ObservableObject
    {
        public static List<CorrectionType> CorrectionTypes { get => Enum.GetValues(typeof(CorrectionType)).Cast<CorrectionType>().ToList(); }
        public static List<FilterType> FilterTypes { get => Enum.GetValues(typeof(FilterType)).Cast<FilterType>().ToList(); }
        public static List<FilterConfigurationType> FilterConfigurationTypes { get => Enum.GetValues(typeof(FilterConfigurationType)).Cast<FilterConfigurationType>().ToList(); }
        public static List<DataWindowType> DataWindowTypes { get => Enum.GetValues(typeof(DataWindowType)).Cast<DataWindowType>().ToList(); }
        public FileData? FileData { get => m_FileData; private set => SetProperty(ref m_FileData, value); }
        public bool IsProcessing { get => m_IsProcessing; private set => SetProperty(ref m_IsProcessing, value); }
        public double FFTMaxFrequency { get => m_FFTMaxFrequency; private set => SetProperty(ref m_FFTMaxFrequency, value); }
        public ISeries[] FFTSeries { get => m_FFTSeries; private set => SetProperty(ref m_FFTSeries, value); }
        public ICartesianAxis[] FFTAxesX { get => m_FFTAxesX; private set => SetProperty(ref m_FFTAxesX, value); }
        public ICartesianAxis[] FFTAxesY { get => m_FFTAxesY; private set => SetProperty(ref m_FFTAxesY, value); }
        public IAsyncRelayCommand ProcessCommand { get => m_ProcessCommand; }
        public IAsyncRelayCommand ApplyFilterCommand { get => m_ApplyFilterCommand; } 
        public int FFTDCCutoff { get => m_FFTDCCutoff; set { SetProperty(ref m_FFTDCCutoff, value); CutFFT(); } }
        public FilterType SelectedFilterType { get => m_SelectedFilterType; set { SetProperty(ref m_SelectedFilterType, value); UpdateCommandStates(); InitializeFilter();  } }
        public LabelVisual FFTTitle { get; } = new() { Text = "FFT", TextSize = 25, Padding = new LiveChartsCore.Drawing.Padding(15), Paint = new SolidColorPaint(SKColors.DarkSlateGray) };
        public IFilter? Filter { get => m_Filter; private set => SetProperty(ref m_Filter, value); }
        public FilterConfigurationType SelectedFilterConfigurationType { get => m_SelectedFilterConfigurationType; set { SetProperty(ref m_SelectedFilterConfigurationType, value); UpdateCommandStates(); InitializeFilter(); } }

        public ProcessingViewModel(IAnalysisService analysisService, IWindowService windowService)
        {
            m_AnalysisService = analysisService;
            m_WindowService = windowService;

            m_ProcessCommand = new AsyncRelayCommand(Process, ProcessCanExecute);
            m_ApplyFilterCommand = new AsyncRelayCommand(ApplyFilter, ApplyFilterCanExecute);
            m_Commands = new[] { m_ProcessCommand, m_ApplyFilterCommand };
            m_AsyncCommands = Array.Empty<IAsyncRelayCommand>();




            m_FFTSeries = Array.Empty<ISeries>();

            m_FFTAxesX = new Axis[] { new() { Name = "Hz" } };
            m_FFTAxesY = new Axis[] { new() };

        }

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

        private async Task Process()
        {
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
                ResetAxes();
                CutFFT();
            }
            else
            {
                FFTSeries = Array.Empty<ISeries>();
                m_WindowService.ShowErrorDialog(data.ErrorMessage);
            }
            IsProcessing = false;
        }

        private void CutFFT()
        {
            var cutData = m_FFTPoints?.Where(p => p.X > FFTDCCutoff).ToList();
            var maxAmp = cutData?.Max(p => p.Y);
            FFTMaxFrequency = cutData?.Find(q => q.Y == maxAmp)?.X ?? -1;
            FFTSeries[0].Values = cutData;
        }

        private void ResetAxes()
        {
            m_FFTAxesX[0].MinLimit = null;
            m_FFTAxesX[0].MaxLimit = null;
            m_FFTAxesY[0].MinLimit = null;
            m_FFTAxesY[0].MaxLimit = null;
        }

        #region Command States
        private bool ProcessCanExecute()
        {
            return !IsProcessing;
        }
        private bool ApplyFilterCanExecute()
        {
            return !IsProcessing && m_SelectedFilterType != FilterType.None;
        }
        private void UpdateCommandStates()
        {
            foreach (var command in m_Commands)
                command.NotifyCanExecuteChanged();
            foreach (var command in m_AsyncCommands)
                command.NotifyCanExecuteChanged();
        }
        #endregion

        private void InitializeFilter()
        {
            switch (m_SelectedFilterType)
            {
                case FilterType.None:
                    Filter = null;
                    break;
                case FilterType.Butterworth:
                    Filter = m_FilterFactory.CreateFilter(m_SelectedFilterType, m_SelectedFilterConfigurationType);
                    break;
                case FilterType.Chebyshev:
                    break;
                case FilterType.Bessel:
                    break;
                case FilterType.Elliptic:
                    break;
                case FilterType.Legendre:
                    break;
                case FilterType.Gaussian:
                    break;
                default:
                    break;
            }

            
        }

        private Task ApplyFilter()
        {
            Debug.WriteLine($"Filter: {Filter}");
            return Task.CompletedTask;
        }


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
        private readonly IRelayCommand[] m_Commands;
        private readonly IAsyncRelayCommand[] m_AsyncCommands;
        private int m_FFTDCCutoff = 1;
        private double m_FFTMaxFrequency;
        private List<ObservablePoint>? m_FFTPoints;
        

        private IFilter? m_Filter;
        private FilterType m_SelectedFilterType = FilterType.None;
        private FilterConfigurationType m_SelectedFilterConfigurationType = FilterConfigurationType.LowPass;
        private FilterFactory m_FilterFactory = new();

        #endregion
    }
}
