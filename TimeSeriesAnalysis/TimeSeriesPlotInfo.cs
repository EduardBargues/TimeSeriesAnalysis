using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using OxyPlot;
using OxyPlot.Series;

namespace TimeSeriesAnalysis
{
    public class TimeSeriesPlotInfo
    {
        public TimeSeries Series { get; set; }
        public Type SeriesType { get; set; }
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                LegendTitleFunction = oxyColor => title;
            }
        }
        private Color color;
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                ColorFunction = dv => color;
            }
        }
        public Func<DateValue, Color> ColorFunction { get; set; }
        public Func<Color, string> LegendTitleFunction { get; set; }
        public MarkerType Marker { get; set; }
        public LineStyle LineStyle { get; set; }
        public int Order { get; set; }

        public static TimeSeriesPlotInfo Create(
            TimeSeries series = null,
            Type seriesType = null,
            string title = null,
            Color? color = null,
            Func<DateValue, Color> colorFunction = null,
            Func<Color, string> legendFunction = null,
            MarkerType markerType = MarkerType.None,
            LineStyle lineStyle = LineStyle.None,
            int plotOrder = 0)
        {
            TimeSeriesPlotInfo result = new TimeSeriesPlotInfo
            {
                Series = series,
                SeriesType = seriesType ?? typeof(FunctionSeries),
                LineStyle = lineStyle,
                Marker = markerType
            };

            if (color != null)
                result.Color = color.Value;
            if (colorFunction != null)
                result.ColorFunction = colorFunction;
            if (result.Color == null &&
                result.ColorFunction == null)
                result.Color = Color.Blue;

            if (!string.IsNullOrEmpty(title))
                result.Title = title;
            if (legendFunction != null)
                result.LegendTitleFunction = legendFunction;
            if (string.IsNullOrEmpty(result.Title) &&
                result.LegendTitleFunction == null)
                result.Title = string.IsNullOrEmpty(series?.Name)
                    ? "NO NAME"
                    : series.Name;

            return result;
        }
    }
}