using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.WindowsForms;
using TimeSeriesAnalysis;
using System.Windows.Forms;
using OxyPlot.Series;

namespace TeslaAnalysis
{
    public class CandleTimeSeries
    {
        private readonly Dictionary<DateTime, Candle> candlesByDate = new Dictionary<DateTime, Candle>();

        public IEnumerable<DateTime> Dates => candlesByDate.Keys;
        public IEnumerable<Candle> Candles => candlesByDate.Keys
            .Select(date => candlesByDate[date]);
        public string Name { get; set; }

        public CandleTimeSeries()
        { }
        public CandleTimeSeries(IEnumerable<Candle> candles)
        {
            candlesByDate = candles.ToDictionary(candle => candle.StartDate);
        }
        public void SetCandle(DateTime date, Candle candle)
        {
            if (!ContainsValueAt(date))
                candlesByDate.Add(date, candle);
            candlesByDate[date] = candle;
        }
        public Candle GetValue(DateTime date)
        {
            return candlesByDate[date];
        }
        public bool TryGetValue(DateTime day, out Candle value)
        {
            return candlesByDate.TryGetValue(day, out value);
        }
        public Candle this[DateTime date]
        {
            get
            {
                return GetValue(date);
            }
            set
            {
                SetCandle(date, value);
            }
        }
        public bool ContainsValueAt(DateTime date)
        {
            return candlesByDate.ContainsKey(date);
        }
        public bool HasCandles()
        {
            return Dates.Any();
        }

        public static void Plot(CandleTimeSeriesPlotInfo info)
        {
            List<HighLowItem> items = info.Series.Candles
                .OrderBy(candle => candle.StartDate)
                .Select(candle => new HighLowItem()
                {
                    X = DateTimeAxis.ToDouble(candle.StartDate),
                    Open = candle.Open,
                    Close = candle.Close,
                    High = candle.High,
                    Low = candle.Low,
                })
                .ToList();
            CandleStickSeries series = new CandleStickSeries
            {
                RenderInLegend = true,
                Title = info.Title,
                Background = OxyColor.FromArgb(
                    info.BackgroundColor.A,
                    info.BackgroundColor.R,
                    info.BackgroundColor.G,
                    info.BackgroundColor.B),
                //CandleWidth = info.CandleWidth,
                IsVisible = true,
                DecreasingColor = OxyColor.FromArgb(
                    info.DecreasingColor.A,
                    info.DecreasingColor.R,
                    info.DecreasingColor.G,
                    info.DecreasingColor.B),
                IncreasingColor = OxyColor.FromArgb(
                    info.IncreasingColor.A,
                    info.IncreasingColor.R,
                    info.IncreasingColor.G,
                    info.IncreasingColor.B),
                Color = OxyColor.FromArgb(
                    info.BackgroundColor.A,
                    info.BackgroundColor.R,
                    info.BackgroundColor.G,
                    info.BackgroundColor.B),
                ItemsSource = items,
                //Hollow = info.PositiveCandlesHollow,
            };

            PlotModel model = new PlotModel();
            model.Axes.Clear();
            DateTimeAxis timeAxis = new DateTimeAxis();
            model.Axes.Add(timeAxis);
            model.Series.Clear();
            model.Series.Add(series);
            PlotView view = new PlotView
            {
                Model = model,
                Dock = DockStyle.Fill,
            };

            Form form = new Form();
            form.Controls.Add(view);
            form.ShowDialog();
        }
        public static CandleTimeSeries Create(IEnumerable<Candle> candles)
        {
            CandleTimeSeries series = new CandleTimeSeries(candles);
            return series;
        }

    }
}
