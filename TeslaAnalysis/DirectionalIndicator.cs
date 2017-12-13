using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common;
using MoreLinq;
using TimeSeriesAnalysis;

namespace TeslaAnalysis
{
    public class DirectionalIndicator : Indicator
    {
        public DirectionalIndicator(Func<CandleTimeSeries, DateTime, double> function) : base(function)
        {
        }

        public static DirectionalIndicator Create(TimeSpan period, bool plus = true)
        {
            double Function(CandleTimeSeries series, DateTime instant)
            {
                DateTime startInstant = instant.Substract(period);
                List<Candle> candles = series.Candles
                    .Where(candle => candle.Start <= instant &&
                                     candle.Start >= startInstant)
                    .OrderBy(candle => candle.Start)
                    .ToList();
                List<Candle> list = candles
                    .Where((candle, index) => index > 0)
                    .ToList();
                double averageDm = list
                    .Average((candle, index) =>
                    {
                        int candlesIndex = index + 1;
                        Candle previousCandle = candles[candlesIndex - 1];
                        Candle currentCandle = candles[candlesIndex];
                        double upMove = Utils.Max(currentCandle.Max - previousCandle.Max, 0);
                        double downMove = Utils.Max(previousCandle.Min - currentCandle.Min, 0);
                        if (upMove >= downMove) downMove = 0;
                        else upMove = 0;
                        double dm = plus ? upMove : downMove;
                        return dm;
                    });
                double averageTrueRange = list
                    .Average((candle, index) =>
                    {
                        int candlesIndex = index + 1;
                        Candle previousCandle = candles[candlesIndex - 1];
                        Candle currentCandle = candles[candlesIndex];
                        double trueRange = Utils.Max(
                            currentCandle.Max - currentCandle.Min,
                            currentCandle.Max - previousCandle.Close,
                            currentCandle.Min - previousCandle.Close);
                        return trueRange;
                    });
                double di = averageDm.DivideBy(averageTrueRange);
                return di;
            }
            DirectionalIndicator indicator = new DirectionalIndicator(Function);
            return indicator;
        }
    }
}
