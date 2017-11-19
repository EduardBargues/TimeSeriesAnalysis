using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Bitfinex;
using Bitfinex.Models;
using MoreLinq;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using TimeSeriesAnalysis;

namespace TeslaAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestTendecyLines();
            //TestBitFinexApi();
            TestAutoCorrelation();
        }

        private static void TestAutoCorrelation()
        {
            double period = 365 / 2.0;
            DateTime dateTime = new DateTime(2000, 1, 1);
            DateTime dateTime2 = new DateTime(2000, 12, 31);
            TimeSeries ts = TimeSeries.CreateDailySinusoidalTimeSeries(1, 2 * Math.PI / period, 0, dateTime, dateTime2);

            CorrelationAnalysisParameters parameters = new CorrelationAnalysisParameters
            {
                NumberOfSpansIntheFuture = 10,
                NumberOfSpansInthePast = 10,
                Span = TimeSpan.FromDays(1)
            };
            IEnumerable<TemporalGapTuple> items = TimeSeriesCorrelation.DoCorrelationAnalysis(ts, ts.Copy(), parameters);
            FunctionSeries f = new FunctionSeries();
            items
                .OrderBy(item => item.TemporalGap)
                .Select(item => new DataPoint(item.TemporalGap.Days, item.Correlation))
                .ForEach(dataPoint => f.Points.Add(dataPoint));
            PlotModel model = new PlotModel();
            model.Series.Add(f);
            PlotView view = new PlotView
            {
                Dock = DockStyle.Fill,
                Visible = true,
                Model = model
            };
            Form form = new Form();
            form.Controls.Add(view);
            form.ShowDialog();
        }

        private static void TestBitFinexApi()
        {
            //BitfinexRestClient client = new BitfinexRestClient();
            //List<ITicker> tickers = client.GetTickers(new string[] { "tBTCUSD" });
            WebClient webClient = new WebClient();
            using (webClient)
            {
                //webClient.BaseAddress = "http://api.bitcoincharts.com/v1/trades.csv?symbol=bitstampUSD";
                string downloadString = webClient.DownloadString("http://api.bitcoincharts.com/v1/trades.csv?symbol=bitstampUSD");
            }
        }

        private static void TestTendecyLines()
        {
            // read and plot
            List<Candle> candles = ReadFile();
            TimeSeries tsOpen = new TimeSeries(candles.ToDictionary(candle => candle.StartDate, candle => candle.Open));
            tsOpen.Name = "Open";
            TimeSeries tsClose = new TimeSeries(candles.ToDictionary(candle => candle.StartDate, candle => candle.Close));
            tsClose.Name = "Close";
            TimeSeries tsHigh = new TimeSeries(candles.ToDictionary(candle => candle.StartDate, candle => candle.High));
            tsHigh.Name = "High";
            TimeSeries tsLow = new TimeSeries(candles.ToDictionary(candle => candle.StartDate, candle => candle.Low));
            tsLow.Name = "Low";
            TimeSeries tsVolume = new TimeSeries(candles.ToDictionary(candle => candle.StartDate, candle => candle.BuyVolume));
            tsVolume.Name = "Volume";
            CandleTimeSeries tsCandles = CandleTimeSeries.Create(candles);
            tsCandles.Name = "Candles";

            // periodos de crecimiento y decrecimiento
            // método básico
            TimeSeries tendencyPeriods = tsOpen.Values.GetTendencyLines(50)
                .ToTimeSeries("TendencyPeriods");
            TimeSeries.Plot(
                new TimeSeriesPlotInfo(tsOpen, OxyColor.FromRgb(255, 0, 0)),
                new TimeSeriesPlotInfo(tendencyPeriods, OxyColor.FromRgb(0, 0, 255))
            );

            // método medias móviles
            int fastPeriod = 4;
            int mediumPeriod = 12;
            int slowPeriod = 30;
            TimeSeries fastSma = tsOpen.GetSimpleMovingAverage(TimeSpan.FromDays(fastPeriod)).ToTimeSeries($"SMA-{fastPeriod}");
            TimeSeries mediumSma = tsOpen.GetSimpleMovingAverage(TimeSpan.FromDays(mediumPeriod))
                .ToTimeSeries($"SMA-{mediumPeriod}");
            TimeSeries slowSma = tsOpen.GetSimpleMovingAverage(TimeSpan.FromDays(slowPeriod)).ToTimeSeries($"SMA-{slowPeriod}");
            OxyColor colorUp = OxyColor.FromRgb(0, 0, 255);
            OxyColor colorDown = OxyColor.FromRgb(255, 0, 0);
            OxyColor colorRange = OxyColor.FromRgb(0, 0, 0);
            OxyColor colorSeries = OxyColor.FromRgb(0, 255, 0);
            Func<OxyColor, string> legendSeparator = color =>
            {
                string result;
                if (color == colorUp)
                    result = "Up";
                else if (color == colorDown)
                    result = "Down";
                else
                    result = "Range";
                return result;
            };
            Func<DateValue, OxyColor> colorSeparator = dv =>
            {
                double fastValue = fastSma[dv.Date];
                double mediumValue = mediumSma[dv.Date];
                double slowValue = slowSma[dv.Date];
                OxyColor result;
                if (fastValue >= mediumValue &&
                    mediumValue >= slowValue)
                    result = colorUp;
                else if (fastValue < mediumValue &&
                         mediumValue < slowValue)
                    result = colorDown;
                else
                    result = colorRange;
                return result;
            };
            TimeSeries.Plot(
                TimeSeriesPlotInfo.Create(series: tsOpen, color: colorSeries, plotOrder: 5),
                TimeSeriesPlotInfo.Create(series: tsOpen, colorFunction: colorSeparator, legendFunction: legendSeparator,
                    plotOrder: 4),
                TimeSeriesPlotInfo.Create(series: fastSma, color: OxyColor.FromRgb(125, 125, 125), plotOrder: 1),
                TimeSeriesPlotInfo.Create(series: mediumSma, color: OxyColor.FromRgb(175, 175, 175), plotOrder: 2),
                TimeSeriesPlotInfo.Create(series: slowSma, color: OxyColor.FromRgb(225, 225, 225), plotOrder: 3)
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
                        StartDate = DateTime.ParseExact(items[0], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Open = double.Parse(items[1], CultureInfo.InvariantCulture),
                        High = double.Parse(items[2], CultureInfo.InvariantCulture),
                        Low = double.Parse(items[3], CultureInfo.InvariantCulture),
                        Close = double.Parse(items[4], CultureInfo.InvariantCulture),
                    };
                })
                .Where(candle => candle.StartDate >= firstDate)
                .OrderBy(candle => candle.StartDate)
                .ToList();
            DateTime date1 = candles.First().StartDate;
            DateTime date2 = candles[1].StartDate;
            TimeSpan duration = date2 - date1;
            candles
                .ForEach(candle => candle.Duration = duration);
            foreach (Candle candle in candles)
                candle.Duration = duration;
            return candles;
        }
    }
}
