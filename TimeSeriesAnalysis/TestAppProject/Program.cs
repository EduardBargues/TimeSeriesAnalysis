using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Windows.Forms;
using Common;
using MoreLinq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using QuandlCS.Connection;
using QuandlCS.Requests;
using QuandlCS.Types;
using TeslaAnalysis;
using TimeSeriesAnalysis;
using DateValue = TimeSeriesAnalysis.DateValue;
using Plotter = TimeSeriesAnalysis.Plotter;
using Spi = TimeSeriesAnalysis.Spi;
using TimeSeriesCorrelation = TimeSeriesAnalysis.TimeSeriesCorrelation;
using TimeSeriesPlotInfo = TimeSeriesAnalysis.TimeSeriesPlotInfo;

namespace TestAppProject
{
    public static class Program
    {
        static void Main(string[] args)
        {
            ReadTicks();
            //TestAluminum();
            //TestLinearPlusSinus();
            //TestPredictor();
            //TestLoessDecompositionLinear();
            //TestLoessDecompositionSinus();
            //IsNasdaqPeriodic();
            //TestNormalization();
            //TestTopBottomValues();
            //TestTendencyLines();
            //TestMultiplePlots();
            //TestHistogram();
            //TestGenericPlotter();
            //TestCorrelationPlot();
            //TestMovingAverages();
            //TestMultiColorSeries();
        }

        private static void ReadTicks()
        {
            string destinyPath = @"C:\Users\EBJ\Desktop\BitCoinData\BitCoinDataByDay";
            Directory.Delete(destinyPath);
            Directory.CreateDirectory(destinyPath);

            string path = @"C:\Users\EBJ\Desktop\BitCoinData";
            Directory.GetFiles(path)
                .Select(fileName => new FileInfo(fileName))
                .Where(file => file.Extension == "csv")
                .SelectMany(ReadFile)
                .DistinctBy(trade => trade.Date)
                .GroupBy(trade => trade.Date.Date)
                .ForEach(grouping => WriteData(grouping.Key, grouping, destinyPath));
        }

        private static void WriteData(DateTime day, IEnumerable<Trade> trades, string path)
        {
            string fileName = $"BitCoin {day.Year}-{day.Month}-{day.Day}.csv";
            string fullFileName = Path.Combine(path, fileName);
            using (TextWriter tw = new StreamWriter(fullFileName))
            {
                tw.WriteLine("Date; Volume; Price; Type");
                foreach (Trade trade in trades.OrderBy(trade => trade.Date))
                    tw.WriteLine(trade.ToString());
                tw.Flush();
                tw.Close();
            }
        }

        private static IEnumerable<Trade> ReadFile(FileInfo file)
        {
            DateTime initialDate = new DateTime(1970, 1, 1);
            string[] lines = File.ReadAllLines(file.FullName);
            for (int lineHandle = 1; lineHandle < lines.Length; lineHandle++)
            {
                List<double> values = lines[lineHandle]
                    .Split(';')
                    .Select(word => double.Parse(word, CultureInfo.InvariantCulture))
                    .ToList();
                yield return new Trade
                {
                    Date = initialDate.AddMilliseconds(values[0]),
                    Volume = values[1],
                    Price = values[2],
                    Type = values[1] > 0 ? TradeType.Buy : TradeType.Sell
                };
            }
        }

        private static void TestMultiColorSeries()
        {
            DateTime firstDate = new DateTime(2000, 1, 1);
            DateTime lastDate = new DateTime(2000, 12, 31);
            TimeSeries random = TimeSeries.CreateDailyNormallyRandomSeries(0, 10, firstDate, lastDate);
            TimeSeries sinus = TimeSeries.CreateDailySinusoidalTimeSeries(100, 2 * Math.PI / 365, 0, firstDate, lastDate);
            TimeSeries sum = sinus.Sum(random);

            Func<DateValue, OxyColor> colorFunction = dv => dv.Value >= 0
                ? OxyColor.FromRgb(255, 0, 0)
                : OxyColor.FromRgb(0, 0, 255);
            Func<OxyColor, string> legendColor = color => color == OxyColor.FromRgb(255, 0, 0)
                ? "T(t)>=0"
                : "T(t)<0";
            TimeSeriesPlotInfo info = TimeSeriesPlotInfo.Create(series: sum, colorFunction: colorFunction, legendFunction: legendColor);
            TimeSeries.Plot(info);
        }

        private static void TestMovingAverages()
        {
            DateTime firstDate = new DateTime(2000, 1, 1);
            DateTime lastDate = new DateTime(2000, 12, 31);
            TimeSeries random = TimeSeries.CreateDailyNormallyRandomSeries(0, 10, firstDate, lastDate);
            TimeSeries sinus = TimeSeries.CreateDailySinusoidalTimeSeries(100, 2 * Math.PI / 365, 0, firstDate, lastDate);
            TimeSeries sum = sinus.Sum(random);
            sum.Name = "Series";
            const int period = 15;
            TimeSeries sma = sum.GetSimpleMovingAverage(TimeSpan.FromDays(period))
                .ToTimeSeries($"SMA-{period}");
            TimeSeries ema = sum.GetExponentialMovingAverage(TimeSpan.FromDays(period), TimeSpan.FromDays(1))
                .ToTimeSeries($"EMA-{period}");
            TimeSeries.Plot(
                new TimeSeriesPlotInfo(sum, OxyColor.FromRgb(255, 0, 0)),
                new TimeSeriesPlotInfo(sma, OxyColor.FromRgb(0, 255, 0)),
                new TimeSeriesPlotInfo(ema, OxyColor.FromRgb(0, 0, 255))
                );
        }

        private static void TestCorrelationPlot()
        {
            DateTime firstDate = new DateTime(2000, 1, 1);
            DateTime lastDate = new DateTime(2000, 12, 31);
            TimeSeries ts1 = TimeSeries.CreateDailyLinearTimeSeries(0, 100, firstDate, lastDate);
            TimeSeries ts2 = TimeSeries.CreateDailyLinearTimeSeries(100, 0, firstDate, lastDate);
            TimeSeriesCorrelation.PlotCorrelationGraph(ts1.Values, ts2.Values);
        }

        private static void TestGenericPlotter()
        {
            DataPointSeries series = Plotter.GetDataPointSeries(
                typeof(FunctionSeries),
                new Spi(nameof(FunctionSeries.LineStyle), LineStyle.Automatic),
                new Spi(nameof(FunctionSeries.MarkerType), MarkerType.Square),
                new Spi(nameof(FunctionSeries.Color), OxyColor.FromRgb(255, 0, 0)),
                new Spi(nameof(FunctionSeries.MarkerFill), OxyColor.FromRgb(0, 0, 255)),
                new Spi(nameof(FunctionSeries.MarkerSize), 10)
                );

            Random random = new Random();
            for (int i = 0; i < 100; i++)
                series.Points.Add(new DataPoint(random.NextDouble(), random.NextDouble()));

            Plotter.Plot(series);
        }

        private static void TestHistogram()
        {
            string fileName = "NasdaqQuandl.csv";
            string executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            FileInfo exe = new FileInfo(executablePath);
            string path = Path.Combine(exe.Directory.FullName, fileName);

            if (!File.Exists(path))
                CreateFile(path);

            TimeSeries values = TimeSeries.FromCsvFile(path).ToTimeSeries();
            TimeSeries normValues = values.Values
                .Normalize()
                .ToTimeSeries();
            TimeSeries normIncrements = values.Values
                .GetIncrements()
                .Normalize()
                .ToTimeSeries();
            TimeSeries.Plot(
                new TimeSeriesPlotInfo(normIncrements, typeof(LinearBarSeries), "increments", OxyColor.FromArgb(100, 0, 0, 255)),
                new TimeSeriesPlotInfo(normValues, typeof(FunctionSeries), "series", OxyColor.FromRgb(255, 0, 0))
                );
        }

        private static void TestMultiplePlots()
        {
            DateTime firstDate = new DateTime(2000, 1, 1);
            DateTime lastDate = new DateTime(2000, 12, 31);
            TimeSeries line = TimeSeries.CreateDailyLinearTimeSeries(0, 100, firstDate, lastDate);
            TimeSeries sinus = TimeSeries.CreateDailySinusoidalTimeSeries(50, 2 * Math.PI / 365, 1, firstDate, lastDate);

            TimeSeries.Plot(
                new TimeSeriesPlotInfo(line, typeof(FunctionSeries), "line", OxyColor.FromRgb(255, 0, 0), 1),
                new TimeSeriesPlotInfo(sinus, typeof(LinearBarSeries), "bars", OxyColor.FromArgb(255, 0, 0, 255), 2)
                );
        }

        private static void TestTendencyLines()
        {
            string fileName = "NasdaqQuandl.csv";
            string executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            FileInfo exe = new FileInfo(executablePath);
            string path = Path.Combine(exe.Directory.FullName, fileName);

            if (!File.Exists(path))
                CreateFile(path);

            TimeSeries ts = TimeSeries.FromCsvFile(path)
                .ToTimeSeries();
            TimeSeries tendencyLines = ts.Values
                .GetTendencyLines(100)
                .ToTimeSeries();
            TimeSeries.Plot(
                new TimeSeriesPlotInfo(tendencyLines, typeof(FunctionSeries), "Tendencies", OxyColor.FromRgb(0, 0, 255)),
                new TimeSeriesPlotInfo(ts, typeof(FunctionSeries), "Series", OxyColor.FromRgb(255, 0, 0))
                );
        }

        private static void TestTopBottomValues()
        {
            string fileName = "NasdaqQuandl.csv";
            string executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            FileInfo exe = new FileInfo(executablePath);
            string path = Path.Combine(exe.Directory.FullName, fileName);

            if (!File.Exists(path))
                CreateFile(path);

            TimeSeries ts = TimeSeries.FromCsvFile(path)
                .ToTimeSeries();
            TimeSeries topValues = ts.Values.GetTopValues()
                .ToTimeSeries();
            TimeSeries bottomValues = ts.Values.GetBottomValues()
                .ToTimeSeries();

            TimeSeries.Plot(
                new TimeSeriesPlotInfo(ts, "series", OxyColor.FromRgb(255, 0, 0)),
                new TimeSeriesPlotInfo(topValues, "top values", OxyColor.FromRgb(0, 255, 0)),
                new TimeSeriesPlotInfo(bottomValues, "bottom values", OxyColor.FromRgb(0, 0, 255))
                );
        }

        private static void TestNormalization()
        {
            string fileName = "NasdaqQuandl.csv";
            string executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            FileInfo exe = new FileInfo(executablePath);
            string path = Path.Combine(exe.Directory.FullName, fileName);

            if (!File.Exists(path))
                CreateFile(path);

            IEnumerable<DateValue> values = TimeSeries.FromCsvFile(path);
            TimeSeries series = values.ToTimeSeries();
            TimeSeries normalized = series.Values
                .Normalize()
                .ToTimeSeries();
            TimeSeries.Plot(new TimeSeriesPlotInfo(normalized));
        }
        private static void IsNasdaqPeriodic()
        {
            string fileName = "NasdaqQuandl.csv";
            string executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            FileInfo exe = new FileInfo(executablePath);
            string path = Path.Combine(exe.Directory.FullName, fileName);

            if (!File.Exists(path))
                CreateFile(path);

            IEnumerable<DateValue> values = TimeSeries.FromCsvFile(path);
            TimeSeries series = values.ToTimeSeries();
            series.Name = "Series";
            TimeSeries trend = series.GetCenteredMovingAverage(
                 span: TimeSpan.FromDays(4 * 7))
                 .ToTimeSeries();
            LoessDecompositionParameters parameters = new LoessDecompositionParameters()
            {
                Series = series,
                Step = TimeSpan.FromDays(1),
                LocalPolynomialDegree = 1,
                NumberOfNeighbors = 4 * 7,
                RobustnessIterations = 1,
                SeasonalPeriod = TimeSpan.FromDays(4 * 30)
            };
            TimeSeriesDecomposition decomp = TimeSeriesDecomposition.DoLoessDecomposition(parameters);
            trend.Name = "Trend";
            TimeSeries.Plot(new TimeSeriesPlotInfo(series, OxyColor.FromRgb(255, 0, 0)),
                         new TimeSeriesPlotInfo(decomp.Trend, OxyColor.FromRgb(0, 0, 255)),
                         new TimeSeriesPlotInfo(decomp.Seasonal, OxyColor.FromRgb(0, 255, 0)));
            Random r = new Random();
            int minYear = series.Dates.Min(dv => dv.Year);
            List<TimeSeriesPlotInfo> infos = trend.Values
                .GroupBy(dv => dv.Date.Year)
                .Where(g => g.Key >= 2010 &&
                            g.Key <= 2017)
                .Select(g =>
                {
                    int yearNumber = g.Key;
                    List<DateValue> vals = g.ToList();
                    double max = vals.Max();
                    double min = vals.Min();
                    double amplitude = max - min;
                    TimeSeries s = vals
                        .Substract(min)
                        .DivideBy(amplitude)
                        .SetYearTo(minYear)
                        .ToTimeSeries();

                    return new TimeSeriesPlotInfo(
                        s,
                        typeof(FunctionSeries),
                        $"{yearNumber}",
                        OxyColor.FromRgb((byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255)));
                })
                .ToList();
            TimeSeries.Plot(infos);
        }

        private static void CreateFile(string path)
        {
            QuandlDownloadRequest request = new QuandlDownloadRequest
            {
                Datacode = new Datacode("NASDAQOMX", "COMP"),
                APIKey = "yxJppmZ4vXcDu1JQxtuz",
                StartDate = DateTime.MinValue,
                EndDate = DateTime.MaxValue,
                Format = FileFormats.CSV,
                Frequency = Frequencies.Daily,
                Headers = false,
            };
            QuandlConnection con = new QuandlConnection();
            string result = con.Request(request);
            result.Split('\n')
                .Where(line => !string.IsNullOrEmpty(line))
                .Select(line =>
                {
                    string[] strings = line.Split(',').ToArray();
                    double value = Convert.ToDouble(strings[1], CultureInfo.InvariantCulture);
                    DateTime date;
                    DateTime.TryParse(strings[0], out date);
                    return new DateValue(date, value);
                })
                .ToCsv(path, "yyyy-MM-dd");
        }

        private static void TestLoessDecompositionLinear()
        {
            DateTime firstDate = new DateTime(2000, 1, 1);
            DateTime lastDate = new DateTime(2000, 12, 31);
            TimeSeries trend = TimeSeries.CreateDailyLinearTimeSeries(0, 100, firstDate, lastDate);
            TimeSeries remainder = TimeSeries.CreateDailyNormallyRandomSeries(0, 2.5, firstDate, lastDate);
            TimeSeries series = trend.Sum(remainder);
            TimeSeries.Plot(series);
            LoessDecompositionParameters parameters = new LoessDecompositionParameters()
            {
                Step = new TimeSpan(1, 0, 0, 0),
                Series = series,
                LocalPolynomialDegree = 1,
                NumberOfNeighbors = 10,
                SeasonalPeriod = new TimeSpan(30, 0, 0, 0),
                RobustnessIterations = 1
            };
            TimeSeriesDecomposition decomp = TimeSeriesDecomposition.DoLoessDecomposition(parameters);
            TimeSeries.Plot(
                new TimeSeriesPlotInfo(series, "Series", OxyColor.FromRgb(255, 0, 0)),
                new TimeSeriesPlotInfo(decomp.Trend, "Trend", OxyColor.FromRgb(0, 0, 255)),
                new TimeSeriesPlotInfo(decomp.Seasonal, "Seasonal", OxyColor.FromRgb(0, 255, 0)),
                new TimeSeriesPlotInfo(decomp.Remainder, "Remainder", OxyColor.FromRgb(0, 100, 0))
                );
        }

        private static void TestLoessDecompositionSinus()
        {
            DateTime firstDate = new DateTime(2000, 1, 1);
            DateTime lastDate = new DateTime(2000, 12, 31);
            int period = 30; //days
            TimeSeries trend = TimeSeries.CreateDailyLinearTimeSeries(0, 100, firstDate, lastDate);
            TimeSeries seasonal = TimeSeries.CreateDailySinusoidalTimeSeries(20, 2 * Math.PI / period, 0, firstDate, lastDate);
            TimeSeries remainder = TimeSeries.CreateDailyNormallyRandomSeries(0, 2.5, firstDate, lastDate);
            TimeSeries series = trend
                .Sum(seasonal)
                .Sum(remainder);
            LoessDecompositionParameters parameters = new LoessDecompositionParameters()
            {
                Step = TimeSpan.FromDays(1),
                Series = series,
                LocalPolynomialDegree = 1,
                NumberOfNeighbors = 30,
                SeasonalPeriod = TimeSpan.FromDays(30),
                RobustnessIterations = 1
            };
            TimeSeriesDecomposition decomp = TimeSeriesDecomposition.DoLoessDecomposition(parameters);
            TimeSeries.Plot(
                new TimeSeriesPlotInfo(series, "Series", OxyColor.FromRgb(255, 0, 0)),
                new TimeSeriesPlotInfo(decomp.Trend, "Trend", OxyColor.FromRgb(0, 0, 255)),
                new TimeSeriesPlotInfo(decomp.Seasonal, "Seasonal", OxyColor.FromRgb(0, 255, 0)),
                new TimeSeriesPlotInfo(decomp.Remainder, "Remainder", OxyColor.FromRgb(0, 100, 0))
            );
        }

        //private static void TestLinearPlusSinus()
        //{
        //    DateTime firstDate = new DateTime(2000, 1, 1);
        //    DateTime lastDate = new DateTime(2005, 12, 31);
        //    Tseries linearTs = Tseries.CreateLinearTimeSeries(0, 100, firstDate, lastDate);
        //    Tseries sinTs = Tseries.CreateSinusoidalTimeSeries(20, 2 * Math.PI / 365, 0, firstDate, lastDate);
        //    Tseries ts = linearTs.Sum(sinTs);
        //    //TimeSeries.Plot(linearTs);
        //    //TimeSeries.Plot(sinTs);
        //    Tseries.Plot(ts);

        //    StlDecompositionParameters parameters = GetDefaultStlDecompositionParameters();
        //    ts.RemoveFebruary29S();
        //    TimeSeriesDecomposition decomposition = TimeSeriesDecomposition.DoStlDecomposition(ts, parameters);
        //    PlotDecomposition(decomposition, "Linear+Sinus");
        //}
        //private static void TestAluminum()
        //{
        //    TimeSeriesDecomposition csvTsd = ReadCsv(@"C:\Users\EBJ\Desktop\STL decomposition\STLDecomposition.Test\STL_Aluminum.csv");
        //    Tseries csvTseries = csvTsd.Tseries;
        //    csvTseries.RemoveFebruary29S();
        //    StlDecompositionParameters parameters = GetDefaultStlDecompositionParameters();
        //    TimeSeriesDecomposition mineTsd = TimeSeriesDecomposition.DoStlDecomposition(csvTseries, parameters);

        //    IDateValuesFiller filler = new LinearFiller();
        //    List<DateTime> missingDates = mineTsd.Trend.GetMissingDates()
        //        .ToList();
        //    mineTsd.Trend.AddMissingValuesWith(filler);
        //    mineTsd.Seasonal.AddMissingValuesWith(filler);
        //    Tseries tseries = csvTsd.Tseries;
        //    foreach (DateTime missingDate in missingDates)
        //        mineTsd.Remainder[missingDate] = tseries[missingDate] -
        //                                         mineTsd.Trend[missingDate] -
        //                                         mineTsd.Seasonal[missingDate];

        //    PlotDecomposition(csvTsd, mineTsd);
        //}

        private static void TestPredictor()
        {
            TimeSeries ts1 = TimeSeries.CreateDailyLinearTimeSeries(0, 1, new DateTime(2000, 1, 1), new DateTime(2000, 12, 31));
            TimeSeries sum = ts1.Sum(ts1);
        }
        private static StlDecompositionParameters GetDefaultStlDecompositionParameters()
        {
            StlDecompositionParameters parameters = new StlDecompositionParameters
            {
                InnerLoopPasses = 2,// n(i): iterations in the inner loop
                OuterLoopPasses = 1,// n(o): iterations in the outer loop
                SeasonalCycleObservations = 365// n(p): pdays in a seasonal period
            };
            parameters.LowPassFilterSmoothingParameter =
                parameters.SeasonalCycleObservations.GetNextOddNumber(); // n(l): smoothing parameter for low pass filter

            parameters.SeasonalComponentSmoothingParameter = 7; // n(s): days to use for the smooth

            int nt = (int)Math.Ceiling(1.5 * parameters.SeasonalCycleObservations /
                       (1 - 1.5 / parameters.SeasonalComponentSmoothingParameter));
            parameters.TrendComponentSmoothingParameter = nt.GetNextOddNumber(); // n(t): smoothing parameter for trend component
            parameters.TrendComponentSmoothingParameter = 31;

            return parameters;
        }

        public static void PlotDecomposition(TimeSeriesDecomposition csvTsd, TimeSeriesDecomposition mineTsd)
        {
            PlotTrendComparison(csvTsd, mineTsd);
            PlotComparison(csvTsd.Seasonal, mineTsd.Seasonal, "Seasonal");
        }

        public static void PlotDecomposition(TimeSeriesDecomposition dec, string label)
        {
            FunctionSeries series = new FunctionSeries
            {
                MarkerType = MarkerType.None,
                Color = OxyColor.FromRgb(150, 0, 0),
                Title = $"{label} - Series"
            };
            FunctionSeries trend = new FunctionSeries
            {
                MarkerType = MarkerType.None,
                Color = OxyColor.FromRgb(255, 0, 0),
                Title = $"{label} - Trend"
            };
            FunctionSeries seasonal = new FunctionSeries
            {
                MarkerType = MarkerType.None,
                Color = OxyColor.FromRgb(0, 255, 0),
                Title = $"{label} - Seasonal"
            };
            FunctionSeries remainder = new FunctionSeries
            {
                MarkerType = MarkerType.None,
                Color = OxyColor.FromRgb(0, 0, 255),
                Title = $"{label} - Remainder"
            };
            TimeSeries ts = dec.Tseries;
            foreach (DateTime day in ts.Dates.OrderBy(day => day))
            {
                double dayNumeric = DateTimeAxis.ToDouble(day);
                series.Points.Add(new DataPoint(dayNumeric, ts[day]));
                trend.Points.Add(new DataPoint(dayNumeric, dec.Trend[day]));
                seasonal.Points.Add(new DataPoint(dayNumeric, dec.Seasonal[day]));
                remainder.Points.Add(new DataPoint(dayNumeric, dec.Remainder[day]));
            }

            PlotModel model = new PlotModel();
            model.Series.Add(series);
            model.Series.Add(trend);
            model.Series.Add(seasonal);
            model.Series.Add(remainder);
            model.Axes.Clear();
            model.Axes.Add(new DateTimeAxis());

            PlotView view = new PlotView
            {
                Model = model,
                Dock = DockStyle.Fill
            };

            Form form = new Form();
            form.Controls.Add(view);
            form.Text = label;
            form.ShowDialog();
        }

        private static void PlotComparison(TimeSeries ts1, TimeSeries ts2, string label)
        {
            FunctionSeries series1 = new FunctionSeries
            {
                MarkerType = MarkerType.None,
                Color = OxyColor.FromRgb(255, 0, 0),
                Title = $"{label} 1"
            };
            FunctionSeries series2 = new FunctionSeries
            {
                MarkerType = MarkerType.None,
                Color = OxyColor.FromRgb(0, 255, 0),
                Title = $"{label} 2"
            };

            foreach (DateTime day in ts1.Dates.OrderBy(day => day))
            {
                double dayNumeric = DateTimeAxis.ToDouble(day);
                series1.Points.Add(new DataPoint(dayNumeric, ts1[day]));
                series2.Points.Add(new DataPoint(dayNumeric, ts2[day]));
            }

            PlotModel model = new PlotModel();
            model.Series.Add(series1);
            model.Series.Add(series2);
            model.Axes.Clear();
            model.Axes.Add(new DateTimeAxis());

            PlotView view = new PlotView
            {
                Model = model,
                Dock = DockStyle.Fill
            };

            Form form = new Form();
            form.Controls.Add(view);
            form.Text = label;
            form.ShowDialog();
        }

        private static void PlotTrendComparison(TimeSeriesDecomposition csvTsd, TimeSeriesDecomposition mineTsd)
        {
            FunctionSeries series1 = new FunctionSeries
            {
                MarkerType = MarkerType.None,
                Color = OxyColor.FromRgb(255, 0, 0),
                Title = "Series"
            };
            FunctionSeries trend1 = new FunctionSeries
            {
                MarkerType = MarkerType.None,
                Color = OxyColor.FromRgb(0, 255, 0),
                Title = "Trend 1"
            };
            FunctionSeries trend2 = new FunctionSeries
            {
                MarkerType = MarkerType.None,
                Color = OxyColor.FromRgb(0, 0, 255),
                Title = "Trend 2"
            };
            TimeSeries csvTs = csvTsd.Tseries;
            foreach (DateTime day in csvTs.Dates.OrderBy(day => day))
            {
                double dayNumeric = DateTimeAxis.ToDouble(day);
                series1.Points.Add(new DataPoint(dayNumeric, csvTs[day]));
                trend1.Points.Add(new DataPoint(dayNumeric, csvTsd.Trend[day]));
                trend2.Points.Add(new DataPoint(dayNumeric, mineTsd.Trend[day]));
            }

            PlotModel model = new PlotModel();
            model.Series.Add(series1);
            model.Series.Add(trend1);
            model.Series.Add(trend2);
            model.Axes.Clear();
            model.Axes.Add(new DateTimeAxis());

            PlotView view = new PlotView
            {
                Model = model,
                Dock = DockStyle.Fill
            };

            Form form = new Form();
            form.Controls.Add(view);
            form.Text = "Trend components comparison";
            form.ShowDialog();
        }

        private static TimeSeriesDecomposition ReadCsv(string fileName)
        {
            TimeSeriesDecomposition tsd = new TimeSeriesDecomposition();

            string text = File.ReadAllText(fileName);
            List<string> lines = text.Split(Environment.NewLine.ToCharArray())
                .Where(line => !string.IsNullOrEmpty(line))
                .ToList();

            for (int lineIndex = 1; lineIndex < lines.Count; lineIndex++)
            {
                string[] values = lines[lineIndex].Split(';')
                    .Where(str => !string.IsNullOrEmpty(str))
                    .ToArray();

                DateTime day = DateTime.ParseExact(values[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                double seasonal = Convert.ToDouble(values[2], CultureInfo.InvariantCulture);
                double trend = Convert.ToDouble(values[3], CultureInfo.InvariantCulture);
                double remainder = Convert.ToDouble(values[4], CultureInfo.InvariantCulture);

                tsd.Trend[day] = trend;
                tsd.Seasonal[day] = seasonal;
                tsd.Remainder[day] = remainder;
            }

            return tsd;
        }
    }
}
