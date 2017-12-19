using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace TeslaAnalysis.Indicators
{
    public class AverageDirectionalMovementIndex : Indicator
    {
        public AverageDirectionalMovementIndex(Func<CandleTimeSeries, DateTime, double> function) : base(function)
        {

        }

        public static AverageDirectionalMovementIndex Create(
            int periods,
            int smoothingPeriods)
        {
            DirectionalMovementIndex dmi = DirectionalMovementIndex.Create(periods, smoothingPeriods);
            double Function(CandleTimeSeries series, DateTime instant)
            {
                Candle candle = series[instant];
                int index = series.GetIndex(candle);
                Candle[] candles = index.GetIntegersTo(Math.Max(1, index - periods))
                    .Select(idx => series[idx])
                    .ToArray();
                double ema = candles
                    .WeightedAverage((cdl, idx) => dmi[series, cdl.Start], (cdl, idx) => candles.Length - idx);
                return ema;
            }

            AverageDirectionalMovementIndex plus = new AverageDirectionalMovementIndex(Function);
            return plus;
        }

    }
}
