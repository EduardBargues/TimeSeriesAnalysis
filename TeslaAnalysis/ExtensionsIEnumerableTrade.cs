using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace TeslaAnalysis
{
    public static class ExtensionsIEnumerableTrade
    {
        public static Candle ToCandle(this IEnumerable<Trade> trades, DateTime? startDate = null, TimeSpan? duration = null)
        {
            List<Trade> list = trades
                .OrderBy(trade => trade.Instant)
                .ToList();
            Candle result = new Candle();
            if (list.Any())
            {
                result.Open = list.First().Price;
                result.Close = list.Last().Price;
                result.Max = list.Max(trade => trade.Price);
                result.Min = list.Min(trade => trade.Price);
                result.BuyVolume = list
                    .Sum(trade => trade.Type == TradeType.Buy ? trade.Volume : 0);
                result.SellVolume = list
                    .Sum(trade => trade.Type == TradeType.Sell ? trade.Volume : 0);
                result.Start = startDate ?? list.First().Instant;
                result.Duration = duration ?? list.Last().Instant - result.Start;
            }
            return result;
        }

        public static IEnumerable<Candle> ToCandlesByDuration(
            this IEnumerable<Trade> trades,
            TimeSpan candlesDuration,
            DateTime? firstCandleStart = null)
        {
            List<Trade> list = trades
                .OrderBy(trade => trade.Instant)
                .ToList();
            if (list.Any())
            {
                DateTime firstTradeInstant = list.First().Instant;
                DateTime firstDate = firstCandleStart ?? firstTradeInstant;
                IEnumerable<Candle> candles = list
                    .GroupBy(trade =>
                    {
                        TimeSpan span = trade.Instant - firstDate;
                        return (int)span.DivideBy(candlesDuration);
                    })
                    .Select(grouping =>
                    {
                        int factor = grouping.Key;
                        DateTime candleStart = firstDate + candlesDuration.MultiplyBy(factor);
                        if (candleStart.Day==5)
                        {
                            
                        }
                        Candle candle = grouping.ToCandle(startDate: candleStart, duration: candlesDuration);
                        return candle;
                    });
                foreach (Candle candle in candles)
                    yield return candle;
            }
        }
    }
}
