using System;
using System.Collections.Generic;
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
        private OxyColor color;
        public OxyColor Color
        {
            get { return color; }
            set
            {
                color = value;
                ColorFunction = dv => color;
            }
        }
        public Func<DateValue, OxyColor> ColorFunction { get; set; }
        public Func<OxyColor, string> LegendTitleFunction { get; set; }
        public MarkerType Marker { get; set; }
        public LineStyle LineStyle { get; set; }
        public int Order { get; set; }

        public TimeSeriesPlotInfo(
            TimeSeries s,
            Type seriesType,
            string title,
            OxyColor color,
            int order,
            MarkerType markerType,
            LineStyle lineStyle)
        {
            Series = s;
            SeriesType = seriesType;
            Title = title;
            Color = color;
            Order = order;
            Marker = markerType;
            LineStyle = lineStyle;
        }
        public TimeSeriesPlotInfo(
            TimeSeries s,
            Type seriesType,
            string title,
            OxyColor color,
            int order)
        {
            Series = s;
            SeriesType = seriesType;
            Title = title;
            Color = color;
            Marker = MarkerType.None;
            Order = order;
        }
        public TimeSeriesPlotInfo(
            TimeSeries s,
            Type type,
            string v,
            OxyColor oxyColor)
        {
            Series = s;
            SeriesType = type;
            Title = v;
            Color = oxyColor;
            Marker = MarkerType.None;
            Order = 0;
        }
        public TimeSeriesPlotInfo(
            TimeSeries s,
            Type seriesType,
            string title)
        {
            Series = s;
            SeriesType = seriesType;
            Title = title;
            Color = OxyColors.Automatic;
            Marker = MarkerType.None;
        }
        public TimeSeriesPlotInfo(
            TimeSeries s,
            string title)
        {
            Series = s;
            SeriesType = typeof(FunctionSeries);
            Title = title;
            Color = OxyColors.Automatic;
            Marker = MarkerType.None;
        }
        public TimeSeriesPlotInfo(
            TimeSeries s,
            string title,
            OxyColor color)
        {
            Series = s;
            SeriesType = typeof(FunctionSeries);
            Title = title;
            Color = color;
            Marker = MarkerType.None;
        }
        public TimeSeriesPlotInfo(
            TimeSeries s,
            OxyColor color)
        {
            Series = s;
            SeriesType = typeof(FunctionSeries);
            Title = s.Name;
            Color = color;
            Marker = MarkerType.None;
        }
        public TimeSeriesPlotInfo(
            TimeSeries s,
            Func<DateValue, OxyColor> color)
        {
            Series = s;
            SeriesType = typeof(FunctionSeries);
            Title = s.Name;
            ColorFunction = color;
            Marker = MarkerType.None;
        }
        public TimeSeriesPlotInfo(
            TimeSeries s,
            Func<DateValue, OxyColor> color,
            Func<OxyColor, string> legendTitles)
        {
            Series = s;
            SeriesType = typeof(FunctionSeries);
            Title = s.Name;
            ColorFunction = color;
            LegendTitleFunction = legendTitles;
            Marker = MarkerType.None;
        }
        public TimeSeriesPlotInfo(TimeSeries s)
        {
            Series = s;
            SeriesType = typeof(FunctionSeries);
            Title = s.Name;
            Color = OxyColors.Automatic;
            Marker = MarkerType.None;
        }
    }
}