using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Common;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

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
            candlesByDate = candles.ToDictionary(candle => candle.Start);
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
            get => GetValue(date);
            set => SetCandle(date, value);
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
            PlotView view = GetPlotView(info);

            Form form = new Form();
            form.Controls.Add(view);
            form.ShowDialog();
        }

        public static PlotView GetPlotView(CandleTimeSeriesPlotInfo info, Action<object, AxisChangedEventArgs> onAxisChangedMethod = null)
        {
            List<HighLowItem> items = info.Series.Candles
                .OrderBy(candle => candle.Start)
                .Select(candle => new HighLowItem()
                {
                    X = DateTimeAxis.ToDouble(candle.Start),
                    Open = candle.Open,
                    Close = candle.Close,
                    High = candle.Max,
                    Low = candle.Min,
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
            if (onAxisChangedMethod != null)
                timeAxis.AxisChanged += onAxisChangedMethod.Invoke;
            model.Axes.Add(timeAxis);
            model.Series.Clear();
            model.Series.Add(series);
            PlotView view = new PlotView
            {
                Model = model,
                Dock = DockStyle.Fill,
            };
            return view;
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
                .OrderBy(trade => trade.Instant)
                .ToList();
            if (sortedTrades.Any())
            {
                Trade firstTrade = sortedTrades.First();
                Candle candle = new Candle { Start = firstTrade.Instant };
                foreach (Trade trade in sortedTrades)
                {
                    if (trade.Type == TradeType.Buy)
                        candle.BuyVolume += trade.Volume;
                    if (trade.Type == TradeType.Sell)
                        candle.SellVolume += trade.Volume;
                    if (candle.Max < trade.Price)
                        candle.Max = trade.Price;
                    if (candle.Min > trade.Price)
                        candle.Min = trade.Price;
                    if (candle.Volume >= volumePerCandle)
                    {
                        candle.Duration = trade.Instant - candle.Start;
                        candle.Close = trade.Price;
                        yield return candle;
                        candle = new Candle { Start = trade.Instant };
                    }
                }
            }
        }
        private static IEnumerable<Candle> GetCandleTimeSeries(IEnumerable<Trade> trades, int numberOfTradesPerCandle)
        {
            return trades
                .OrderBy(trade => trade.Instant)
                .Select((trade, index) => new { Trade = trade, Position = index + 1 })
                .GroupBy(anonymous => anonymous.Position / numberOfTradesPerCandle, anonymous => anonymous.Trade)
                .Select(grouping =>
                {
                    Candle candle = GetCandleFromTrades(grouping);
                    Trade firstTrade = grouping.First();
                    candle.Start = firstTrade.Instant;
                    candle.Duration = grouping.Last().Instant - firstTrade.Instant;
                    return candle;
                });
        }
        private static IEnumerable<Candle> GetCandleTimeSeries(IEnumerable<Trade> trades, TimeSpan candleDuration)
        {
            List<Trade> sortedTrades = trades
                .OrderBy(trade => trade.Instant)
                .ToList();
            if (sortedTrades.Any())
            {
                DateTime firstDate = sortedTrades.First().Instant;
                Func<Trade, int> f = t =>
                {
                    DateTime date = t.Instant;
                    TimeSpan span = date - firstDate;
                    double factor = span.DivideBy(candleDuration);
                    return (int)factor;
                };
                foreach (IGrouping<int, Trade> grouping in sortedTrades.GroupBy(trade => f.Invoke(trade)))
                {
                    Candle candle = GetCandleFromTrades(grouping);
                    Trade firstTrade = grouping.First();
                    int scale = f.Invoke(firstTrade);
                    candle.Start = firstDate + candleDuration.MultiplyBy(scale);
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
                Max = list
                    .Max(trade => trade.Price),
                Open = list.First().Price,
                Min = list
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

        public DateTime? GetMinDate()
        {
            return candlesByDate.Keys.Any()
                ? (DateTime?)candlesByDate.Keys.Min()
                : null;
        }

        public DateTime? GetMaxDate()
        {
            return candlesByDate.Keys.Any()
                ? (DateTime?)candlesByDate.Keys.Max()
                : null;
        }

    }

    public enum TradeCandleTransformation
    {
        TemporalSpan,
        TradeQuantity,
        TradeVolume,
    }
}
