using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace TeslaAnalysis.Indicators
{
    public class AverageDirectionalMovementMinus : Indicator
    {
        public AverageDirectionalMovementMinus(Func<CandleTimeSeries, DateTime, double> function) : base(function)
        {

        }

        public static AverageDirectionalMovementMinus Create(
            int periods,
            int smoothingPeriods
            )
        {
            DirectionalMovementMinus dmMinus = DirectionalMovementMinus.Create();

            double Function(CandleTimeSeries series, DateTime instant)
            {
                Candle candle = series[instant];
                int index = series.GetIndex(candle);
                Candle[] candles = index.GetIntegersTo(Math.Max(1, index - periods))
                    .Select(idx => series[idx])
                    .ToArray();
                double ema = candles
                    .WeightedAverage((cdl, idx) => dmMinus[series,cdl.Start], (cdl, idx) => candles.Length - idx);
                return ema;
            }

            AverageDirectionalMovementMinus plus = new AverageDirectionalMovementMinus(Function);
            return plus;
        }
    }
}
