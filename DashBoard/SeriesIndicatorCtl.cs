using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot.Axes;
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
            void EventHandler(object a, AxisChangedEventArgs args)
            {
                DateTimeAxis axis = (DateTimeAxis)a;
                Axis xaxis = indicatorPlotView.Model.Axes.FirstOrDefault();
                if (xaxis != null)
                {
                    xaxis.Reset();
                    xaxis.Minimum = axis.ActualMinimum;
                    xaxis.Maximum = axis.ActualMaximum;
                    indicatorPlotView.Model.InvalidatePlot(true);
                }
            }
            splitContainer.Panel2.Controls.Clear();
            splitContainer.Panel2.Controls.Add(indicatorPlotView);

            PlotView seriesPlotView = CandleTimeSeries.GetPlotView(CandleTimeSeriesPlotInfo.Create(series), EventHandler);
            splitContainer.Panel1.Controls.Clear();
            splitContainer.Panel1.Controls.Add(seriesPlotView);
        }
    }
}
