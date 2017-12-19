using System;
using System.Linq;
using Common;

namespace TeslaAnalysis.Indicators
{
    public class AverageDirectionalMovementPlus : Indicator
    {
        public AverageDirectionalMovementPlus(Func<CandleTimeSeries, DateTime, double> function) : base(function)
        {

        }

        public static AverageDirectionalMovementPlus Create(
            int periods,
            int smoothingPeriods)
        {
            DirectionalMovementPlus dmPlus = DirectionalMovementPlus.Create();

            double Function(CandleTimeSeries series, DateTime instant)
            {
                Candle candle = series[instant];
                int index = series.GetIndex(candle);
                Candle[] candles = index.GetIntegersTo(Math.Max(1, index - periods))
                    .Select(idx => series[idx])
                    .ToArray();
                double ema = candles
                    .WeightedAverage((cdl, idx) => dmPlus[series, cdl.Start], 
                                     (cdl, idx) => candles.Length - idx);
                return ema;
            }

            AverageDirectionalMovementPlus plus = new AverageDirectionalMovementPlus(Function);
            return plus;
        }
    }
}
