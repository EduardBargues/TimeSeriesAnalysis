using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Reporting;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using TeslaAnalysis;
using TimeSeriesAnalysis;

namespace DashBoard
{
    public partial class SeriesIndicatorCtl : UserControl
    {
        public SeriesIndicatorCtl()
        {
            InitializeComponent();
        }

        public void LoadData(CandleTimeSeries series, IEnumerable<(TimeSeries, Color)> indicators)
        {
            IEnumerable<TimeSeriesPlotInfo> infos = indicators
                .Select(item => TimeSeriesPlotInfo.Create(series: item.Item1, color: item.Item2));
            PlotView indicatorPlotView = TimeSeries.GetPlotView(infos);


            splitContainer.Panel2.Controls.Clear();
            splitContainer.Panel2.Controls.Add(indicatorPlotView);

            void onAxisChanged(object a, AxisChangedEventArgs args)
            {
                DateTimeAxis axis = (DateTimeAxis)a;
                Axis xaxis = indicatorPlotView.Model.Axes.FirstOrDefault();
                Axis yAxis = indicatorPlotView.Model.Axes[1];
                if (xaxis != null)
                {
                    xaxis.Reset();
                    xaxis.Minimum = axis.ActualMinimum;
                    xaxis.Maximum = axis.ActualMaximum;
                    List<DataPoint> points = indicatorPlotView.Model.Series
                        .Where(s => s is DataPointSeries)
                        .Cast<DataPointSeries>()
                        .SelectMany(s=>s.Points.Where(p=>p.X>=axis.ActualMinimum && p.X<=axis.ActualMaximum))
                        .ToList();
                    bool any = points.Any();
                    if (any)
                    {
                        double min = points
                            .Min(p => p.Y);
                        double max = points
                            .Max(p => p.Y);
                        yAxis.Reset();
                        yAxis.Minimum = min;
                        yAxis.Maximum = max;
                    }
                    indicatorPlotView.Model.InvalidatePlot(true);
                }
            }

            PlotView seriesPlotView = CandleTimeSeries.GetPlotView(CandleTimeSeriesPlotInfo.Create(series), onAxisChanged);
            splitContainer.Panel1.Controls.Clear();
            splitContainer.Panel1.Controls.Add(seriesPlotView);
        }
    }
}
