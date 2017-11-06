using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Common;
using MathNet.Numerics.Statistics;
using MoreLinq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace TimeSeriesAnalysis
{
    public static class TimeSeriesCorrelation
    {
        public static double GetCorrelationBetween(TimeSeries ts1, TimeSeries ts2)
        {
            DateTime[] commonDomain = ts1.GetCommonDates(ts2)
                .OrderBy(date => date)
                .ToArray();
            IEnumerable<double> x = commonDomain.Select(date => ts1[date]);
            IEnumerable<double> y = commonDomain.Select(date => ts2[date]);

            return Correlation.Pearson(x, y);
        }
        public static IEnumerable<TemporalGapTuple> DoCorrelationAnalysis(
            TimeSeries ts1,
            TimeSeries ts2,
            CorrelationAnalysisParameters parameters)
        {
            return parameters.GetTimeSpans()
                .Select(span => new TemporalGapTuple
                {
                    Indicator = ts1,
                    Target = ts2,
                    TemporalGap = -span,
                    Correlation = GetCorrelationBetween(ts1, ts2.OffsetBy(span))
                });
        }
        public static IEnumerable<TemporalGapTuple> DoCorrelationAnalysis(
            List<TimeSeries> tss,
            CorrelationAnalysisParameters parameters)
        {
            return tss
                .SelectMany(ts1 => tss, (ts1, ts2) => DoCorrelationAnalysis(ts1, ts2, parameters))
                .Select(analysis => analysis.MaxBy(tg => tg.Correlation))
                .DistinctBy(tg => tg.GetId());
        }

        public static PlotView GetCorrelationPlotView(
            IEnumerable<DateValue> values1,
            IEnumerable<DateValue> values2,
            string xlabel = "Time series 1",
            string ylabel = "Time series 2")
        {
            List<DateValue> vals = values1.ToList();
            vals.AddRange(values2);

            DataPointSeries series = Plotter.GetDataPointSeries(
                typeof(FunctionSeries),
                new Spi(nameof(FunctionSeries.LineStyle), LineStyle.None),
                new Spi(nameof(FunctionSeries.MarkerFill), OxyColor.FromRgb(0, 0, 255)),
                new Spi(nameof(FunctionSeries.MarkerType), MarkerType.Circle),
                new Spi(nameof(FunctionSeries.MarkerSize), 3)
                );
            vals
                .GroupBy(dv => dv.Date)
                .Where(g => g.Count() == 2)
                .ForEach(g =>
                {
                    DateValue dvx = g.First();
                    DateValue dvy = g.Last();
                    series.Points.Add(new DataPoint(dvx.Value, dvy.Value));
                });
            PlotModel model = new PlotModel();
            model.Series.Add(series);
            model.Axes.Clear();
            model.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Title = xlabel });
            model.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, Title = ylabel });
            return new PlotView()
            {
                Model = model,
                Dock = DockStyle.Fill,
                Visible = true,
            };
        }
        public static void PlotCorrelationGraph(
            IEnumerable<DateValue> values1,
            IEnumerable<DateValue> values2)
        {
            PlotView view = GetCorrelationPlotView(values1, values2);
            Form frm = new Form()
            {
                ShowIcon = false,
                Text = "",
            };
            frm.Controls.Add(view);
            frm.ShowDialog();
        }
        /// <summary>
        /// Plots the autocorrelation function in a plot view.
        /// </summary>
        /// <param name="plotInfo"></param>
        /// <param name="span"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        public static PlotView GetAutocorrelationFunctionPlotView(
            TimeSeriesPlotInfo plotInfo,
            TimeSpan span,
            int steps)
        {
            TimeSeries ts = plotInfo.Series;
            CorrelationAnalysisParameters parameters = new CorrelationAnalysisParameters()
            {
                Span = span,
                NumberOfSpansInthePast = 0,
                NumberOfSpansIntheFuture = steps
            };

            FunctionSeries series = new FunctionSeries
            {
                MarkerType = plotInfo.Marker,
                Color = plotInfo.Color,
                Title = plotInfo.Title,
            };
            DoCorrelationAnalysis(ts, ts, parameters)
                .Where(pair => pair.TemporalGap.Ticks > 0)
                .OrderBy(pair => pair.TemporalGap.Ticks)
                .ForEach(pair =>
                {
                    double x = (int)pair.TemporalGap.DivideBy(span);
                    DataPoint point = new DataPoint(x, pair.Correlation);
                    series.Points.Add(point);
                });
            PlotModel model = new PlotModel();
            model.Series.Add(series);
            return new PlotView
            {
                Model = model,
                Dock = DockStyle.Fill,
                Visible = true,
            };
        }
    }
}