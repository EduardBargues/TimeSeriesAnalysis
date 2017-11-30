using System.Collections.Generic;
using System.Linq;

namespace TeslaAnalysis
{
    public static class ExtensionsIEnumerableCandle
    {
        public static CandleTimeSeries ToCandleTimeSeries(this IEnumerable<Candle> candles, string name = "")
        {
            return new CandleTimeSeries(candles)
            {
                Name = name,
            };
        }
    }

    public static class ExtensionsIEnumerableTrade
    {
        public static Candle ToCandle(this IEnumerable<Trade> trades)
        {
            List<Trade> list = trades
                .OrderBy(trade => trade.Instant)
                .ToList();
            Candle result = new Candle();
            if (list.Any())
            {
                result.Open = list.First().Price;
                result.Close = list.Last().Price;
                result.High = list.Max(trade => trade.Price);
                result.Low = list.Min(trade => trade.Price);
                result.StartDate = list.First().Instant;
                result.Duration = list.Last().Instant - list.First().Instant;
                result.BuyVolume = list.Where(trade => trade.Type == TradeType.Buy).Sum(trade => trade.Volume);
                result.SellVolume = list.Where(trade => trade.Type == TradeType.Sell).Sum(trade => trade.Volume);
            }
            return result;
        }
    }
}
