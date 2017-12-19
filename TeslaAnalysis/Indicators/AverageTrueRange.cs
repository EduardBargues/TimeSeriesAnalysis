using System;
using System.Linq;
using Common;

namespace TeslaAnalysis.Indicators
{
    public class AverageTrueRange : Indicator
    {
        public AverageTrueRange(Func<CandleTimeSeries, DateTime, double> function) : base(function)
        {
        }

        public static AverageTrueRange Create(int smoothingPeriods)
        {
            TrueRange tr = TrueRange.Create();

            double Function(CandleTimeSeries series, DateTime date)
            {
                Candle candle = series[date];
                int index = series.GetIndex(candle);
                Candle[] candles = index.GetIntegersTo(Math.Max(1, index - smoothingPeriods))
                    .Select(idx => series[idx])
                    .ToArray();
                double ema = candles
                    .WeightedAverage((cdl, idx) => tr[series, cdl.Start],
                                     (cdl, idx) => candles.Length - idx);
                return ema;
            }

            AverageTrueRange ind = new AverageTrueRange(Function);
            return ind;
        }
    }
}
