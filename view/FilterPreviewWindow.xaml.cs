using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SignalDataPicker.model.Filters;
using SkiaSharp;
using System.Collections.Generic;

namespace SignalDataPicker.view
{
    /// <summary>
    /// Interaction logic for FilterPreviewWindow.xaml
    /// </summary>
    public partial class FilterPreviewWindow
    {
        internal FilterPreviewWindow(FilterBase filter)
        {
            InitializeComponent();

            SetFilterData(filter.FilterData);
            lvcFilter.XAxes = m_XAxes;
            lvcFilter.Series = m_Series;
        }

        public void SetFilterData(double[,] filterData)
        {
            var points = new List<ObservablePoint>();
            for (var i = 0; i < filterData.GetLength(0); i++)
            {
                points.Add(new ObservablePoint() { X = filterData[i, 0], Y = filterData[i, 1] });
            }
            m_Series[0].Values = points;
        }

        private readonly ISeries[] m_Series = new ISeries[1]
        {
            new LineSeries<ObservablePoint>
            {
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.Red) {StrokeThickness = 2 },
                GeometryFill = null,
                GeometryStroke = null,
                LineSmoothness = 0,
                TooltipLabelFormatter = (chartPoint) => $"{chartPoint.Model?.X}"
            }
        };
        private readonly Axis[] m_XAxes = new Axis[] { new() { Name = "Hz" } };
    }
}
