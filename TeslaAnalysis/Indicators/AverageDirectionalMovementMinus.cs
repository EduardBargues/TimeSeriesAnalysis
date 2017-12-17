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
        public AverageDirectionalMovementMinus(Func<DateTime, double> function) : base(function)
        {

        }

        public static AverageDirectionalMovementMinus Create(
            CandleTimeSeries series,
            int periods,
            int smoothingPeriods
            )
        {
            DirectionalMovementMinus dmMinus = DirectionalMovementMinus.Create(series);

            double Function(DateTime instant)
            {
                Candle candle = series[instant];
                int index = series.GetIndex(candle);
                Candle[] candles = index.GetIntegersTo(Math.Max(1, index - periods))
                    .Select(idx => series[idx])
                    .ToArray();
                double ema = candles
                    .WeightedAverage((cdl, idx) => dmMinus.GetValueAt(cdl.Start), (cdl, idx) => candles.Length - idx);
                return ema;
            }

            AverageDirectionalMovementMinus plus = new AverageDirectionalMovementMinus(Function);
            return plus;
        }
    }
}
