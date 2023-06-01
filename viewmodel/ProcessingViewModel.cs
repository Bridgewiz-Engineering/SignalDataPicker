using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SignalDataPicker.model;
using SignalDataPicker.service;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalDataPicker.viewmodel
{
    internal class ProcessingViewModel : ObservableObject
    {
        public FileData? FileData { get => m_FileData; private set => SetProperty(ref m_FileData, value); }
        public bool IsProcessing { get => m_IsProcessing; private set => SetProperty(ref m_IsProcessing, value); }
        public ISeries[] PlotSeries { get => m_PlotSeries; private set => SetProperty(ref m_PlotSeries, value); }
        public ICartesianAxis[] PlotAxesX { get => m_PlotAxesX; private set => SetProperty(ref m_PlotAxesX, value); }
        public ICartesianAxis[] PlotAxesY { get => m_PlotAxesY; private set => SetProperty(ref m_PlotAxesY, value); }
        public IAsyncRelayCommand ProcessCommand { get => m_ProcessCommand; }

        public ProcessingViewModel(IAnalysisService analysisService, IWindowService windowService)
        {
            m_AnalysisService = analysisService;
            m_WindowService = windowService;

            m_ProcessCommand = new AsyncRelayCommand(Process, ProcessCanExecute);
            m_PlotSeries = Array.Empty<ISeries>();

            m_PlotAxesX = new Axis[] { new() };
            m_PlotAxesY = new Axis[] { new() };
            
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
            var data = await m_AnalysisService.Process(FileData!);
            if (data.IsSuccess)
            {
                PlotSeries = new ISeries[]
                {
                    new LineSeries<ObservablePoint>
                    {
                        
                        Fill = null,
                        Values = data.FFTResult.ConvertAll<ObservablePoint>(q=>new ObservablePoint(q[0], q[1])),
                        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 1},
                        GeometryFill = null,
                        GeometryStroke = null,
                        LineSmoothness = 0,
                        
                        
                        TooltipLabelFormatter = (chartPoint) => $"{chartPoint.Model?.X}"
                    }
                };
                ResetAxes();

            }
            else
            {
                PlotSeries = Array.Empty<ISeries>();
                m_WindowService.ShowErrorDialog(data.ErrorMessage);
            }
            IsProcessing = false;
        }

        private void ResetAxes()
        {
            m_PlotAxesX[0].MinLimit = null;
            m_PlotAxesX[0].MaxLimit = null;
            m_PlotAxesY[0].MinLimit = null;
            m_PlotAxesY[0].MaxLimit = null;
        }

        private bool ProcessCanExecute()
        {
            return !IsProcessing;
        }


        #region Fields
        private FileData? m_FileData;
        private bool m_IsProcessing;
        private bool m_IsFileDataLoaded;
        private ICartesianAxis[] m_PlotAxesX;
        private ICartesianAxis[] m_PlotAxesY;
        private ISeries[] m_PlotSeries;
        private readonly IAnalysisService m_AnalysisService;
        private readonly IWindowService m_WindowService;
        private readonly IAsyncRelayCommand m_ProcessCommand;
        #endregion
    }
}
