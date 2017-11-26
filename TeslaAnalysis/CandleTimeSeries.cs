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
using Common;
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

        public static IEnumerable<Candle> FromTrades(
            IEnumerable<Trade> trades,
            TimeSpan? candleDuration = null,
            int? numberOfTradesPerCandle = null,
            double? volumePerCandle = null)
        {
            if (candleDuration == null &&
                numberOfTradesPerCandle == null &&
                volumePerCandle == null)
                return new List<Candle>();

            if (candleDuration != null)
                return GetCandleTimeSeries(trades, candleDuration.Value);
            if (numberOfTradesPerCandle != null)
                return GetCandleTimeSeries(trades, numberOfTradesPerCandle.Value);

            return GetCandleTimeSeries(trades, volumePerCandle.Value);
        }
        private static IEnumerable<Candle> GetCandleTimeSeries(IEnumerable<Trade> trades, double volumePerCandle)
        {
            List<Trade> sortedTrades = trades
                .OrderBy(trade => trade.Date)
                .ToList();
            if (sortedTrades.Any())
            {
                Trade firstTrade = sortedTrades.First();
                Candle candle = new Candle { StartDate = firstTrade.Date };
                foreach (Trade trade in sortedTrades)
                {
                    if (trade.Type == TradeType.Buy)
                        candle.BuyVolume += trade.Volume;
                    if (trade.Type == TradeType.Sell)
                        candle.SellVolume += trade.Volume;
                    if (candle.High < trade.Price)
                        candle.High = trade.Price;
                    if (candle.Low > trade.Price)
                        candle.Low = trade.Price;
                    if (candle.Volume >= volumePerCandle)
                    {
                        candle.Duration = trade.Date - candle.StartDate;
                        candle.Close = trade.Price;
                        yield return candle;
                        candle = new Candle { StartDate = trade.Date };
                    }
                }
            }
        }
        private static IEnumerable<Candle> GetCandleTimeSeries(IEnumerable<Trade> trades, int numberOfTradesPerCandle)
        {
            return trades
                .OrderBy(trade => trade.Date)
                .Select((trade, index) => new { Trade = trade, Position = index + 1 })
                .GroupBy(anonymous => anonymous.Position / numberOfTradesPerCandle, anonymous => anonymous.Trade)
                .Select(grouping =>
                {
                    Candle candle = GetCandleFromTrades(grouping);
                    Trade firstTrade = grouping.First();
                    candle.StartDate = firstTrade.Date;
                    candle.Duration = grouping.Last().Date - firstTrade.Date;
                    return candle;
                });
        }
        private static IEnumerable<Candle> GetCandleTimeSeries(IEnumerable<Trade> trades, TimeSpan candleDuration)
        {
            List<Trade> sortedTrades = trades
                .OrderBy(trade => trade.Date)
                .ToList();
            if (sortedTrades.Any())
            {
                DateTime firstDate = sortedTrades.First().Date;
                Func<Trade, int> f = t =>
                {
                    DateTime date = t.Date;
                    TimeSpan span = date - firstDate;
                    double factor = span.DivideBy(candleDuration);
                    return (int)factor;
                };
                foreach (IGrouping<int, Trade> grouping in sortedTrades.GroupBy(trade => f.Invoke(trade)))
                {
                    Candle candle = GetCandleFromTrades(grouping);
                    Trade firstTrade = grouping.First();
                    int scale = f.Invoke(firstTrade);
                    candle.StartDate = firstDate + candleDuration.MultiplyBy(scale);
                    candle.Duration = candleDuration;
                    yield return candle;
                }
            }
        }
        private static Candle GetCandleFromTrades(IEnumerable<Trade> trades)
        {
            List<Trade> list = trades
                .ToList();

            Candle candle = new Candle
            {
                High = list
                    .Max(trade => trade.Price),
                Open = list.First().Price,
                Low = list
                    .Min(trade => trade.Price),
                Close = list.Last().Price,
                SellVolume = list
                    .Where(trade => trade.Type == TradeType.Sell)
                    .Sum(trade => trade.Volume),
                BuyVolume = list
                    .Where(trade => trade.Type == TradeType.Buy)
                    .Sum(trade => trade.Volume)
            };

            return candle;
        }
    }

    public enum TradeCandleTransformation
    {
        TemporalSpan,
        TradeQuantity,
        TradeVolume,
    }
}
