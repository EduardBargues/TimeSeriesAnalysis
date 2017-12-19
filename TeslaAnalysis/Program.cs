using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using OxyPlot;
using TeslaAnalysis.Indicators;
using TimeSeriesAnalysis;
using Color = System.Drawing.Color;

namespace TeslaAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            CandleTimeSeries series = new CandleTimeSeries();
            TrueRange tri = TrueRange.Create();
            List<DateTime> dates = new List<DateTime>();
            List<(DateTime, double)> values = new List<(DateTime, double)>();
            foreach (DateTime date in dates)
            {
                double trValue = tri[series, date];
                (DateTime date, double trValue) tuple = (date, trValue);
                values.Add(tuple);
            }

            //TestTendecyLines();
            //TestTrueFxApi();
        }

        private static void TestTendecyLines()
        {
            // read and plot
            List<Candle> candles = ReadFile();
            TimeSeries tsOpen = new TimeSeries(candles.ToDictionary(candle => candle.Start, candle => candle.Open));
            tsOpen.Name = "Open";
            TimeSeries tsClose = new TimeSeries(candles.ToDictionary(candle => candle.Start, candle => candle.Close));
            tsClose.Name = "Close";
            TimeSeries tsHigh = new TimeSeries(candles.ToDictionary(candle => candle.Start, candle => candle.Max));
            tsHigh.Name = "High";
            TimeSeries tsLow = new TimeSeries(candles.ToDictionary(candle => candle.Start, candle => candle.Min));
            tsLow.Name = "Low";
            TimeSeries tsVolume = new TimeSeries(candles.ToDictionary(candle => candle.Start, candle => candle.BuyVolume));
            tsVolume.Name = "Volume";
            CandleTimeSeries tsCandles = CandleTimeSeries.Create(candles);
            tsCandles.Name = "Candles";

            // periodos de crecimiento y decrecimiento
            // método básico
            TimeSeries tendencyPeriods = tsOpen.Values.GetTendencyLines(50)
                .ToTimeSeries("TendencyPeriods");
            TimeSeries.Plot(
                TimeSeriesPlotInfo.Create(tsOpen, color: Color.Red),
                TimeSeriesPlotInfo.Create(tendencyPeriods, color: Color.Blue)
            );

            // método medias móviles
            int fastPeriod = 4;
            int mediumPeriod = 12;
            int slowPeriod = 30;
            TimeSeries fastSma = tsOpen.GetSimpleMovingAverage(fastPeriod)
                .ToTimeSeries($"SMA-{fastPeriod}");
            TimeSeries mediumSma = tsOpen.GetSimpleMovingAverage(mediumPeriod)
                .ToTimeSeries($"SMA-{mediumPeriod}");
            TimeSeries slowSma = tsOpen.GetSimpleMovingAverage(slowPeriod)
                .ToTimeSeries($"SMA-{slowPeriod}");
            Color colorUp = Color.Blue;
            Color colorDown = Color.Red;
            Color colorRange = Color.Black;
            Color colorSeries = Color.Green;

            string LegendSeparator(Color color)
            {
                string result;
                if (color == colorUp)
                    result = "Up";
                else if (color == colorDown)
                    result = "Down";
                else
                    result = "Range";
                return result;
            }

            Color ColorSeparator(DateValue dv)
            {
                double fastValue = fastSma[dv.Date];
                double mediumValue = mediumSma[dv.Date];
                double slowValue = slowSma[dv.Date];
                Color result;
                if (fastValue >= mediumValue && mediumValue >= slowValue)
                    result = colorUp;
                else if (fastValue < mediumValue && mediumValue < slowValue)
                    result = colorDown;
                else
                    result = colorRange;
                return result;
            }

            TimeSeries.Plot(
                TimeSeriesPlotInfo.Create(series: tsOpen, color: colorSeries, plotOrder: 5),
                TimeSeriesPlotInfo.Create(series: tsOpen, colorFunction: ColorSeparator, legendFunction: LegendSeparator,
                    plotOrder: 4),
                TimeSeriesPlotInfo.Create(series: fastSma, color: Color.FromArgb(1, 125, 125, 125), plotOrder: 1),
                TimeSeriesPlotInfo.Create(series: mediumSma, color: Color.FromArgb(1, 175, 175, 175), plotOrder: 2),
                TimeSeriesPlotInfo.Create(series: slowSma, color: Color.FromArgb(1, 225, 225, 225), plotOrder: 3)
            );

            // estadísticas de velas
            CandleTimeSeriesPlotInfo info = CandleTimeSeriesPlotInfo.Create(
                series: tsCandles
            );
            CandleTimeSeries.Plot(info);
        }

        private static List<Candle> ReadFile()
        {
            string fileName = @"C:\Users\EBJ\Desktop\Tesla.csv";
            List<string> lines = File.ReadLines(fileName)
                .ToList();
            DateTime firstDate = new DateTime(2013, 1, 1);
            List<Candle> candles = lines
                .Where(line => line != lines.First())
                .Select(line =>
                {
                    string[] items = line.Split(',');
                    return new Candle()
                    {
                        Start = DateTime.ParseExact(items[0], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Open = double.Parse(items[1], CultureInfo.InvariantCulture),
                        Max = double.Parse(items[2], CultureInfo.InvariantCulture),
                        Min = double.Parse(items[3], CultureInfo.InvariantCulture),
                        Close = double.Parse(items[4], CultureInfo.InvariantCulture),
                    };
                })
                .Where(candle => candle.Start >= firstDate)
                .OrderBy(candle => candle.Start)
                .ToList();
            DateTime date1 = candles.First().Start;
            DateTime date2 = candles[1].Start;
            TimeSpan duration = date2 - date1;
            candles
                .ForEach(candle => candle.Duration = duration);
            foreach (Candle candle in candles)
                candle.Duration = duration;
            return candles;
        }
    }

    public static class UriExtensions
    {
        public static Uri AddQuery(this Uri uri, string name, string value)
        {
            NameValueCollection httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

            httpValueCollection.Remove(name);
            httpValueCollection.Add(name, value);

            UriBuilder ub = new UriBuilder(uri) { Query = httpValueCollection.ToString() };

            return ub.Uri;
        }
    }

}
