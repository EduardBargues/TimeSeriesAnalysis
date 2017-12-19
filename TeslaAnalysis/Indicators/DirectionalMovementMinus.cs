using System;
using Common;

namespace TeslaAnalysis.Indicators
{
    public class DirectionalMovementMinus : Indicator
    {
        public DirectionalMovementMinus(Func<CandleTimeSeries, DateTime, double> function) : base(function)
        {
        }

        public static DirectionalMovementMinus Create()
        {
            double Function(CandleTimeSeries series,DateTime instant)
            {
                Candle currentCandle = series[instant];
                int index = series.GetIndex(currentCandle);
                Candle previousCandle = series[index - 1];
                double um = Utils.Max(currentCandle.Max - previousCandle.Max, 0);
                double dm = Utils.Max(previousCandle.Min - currentCandle.Min, 0);
                return dm > um &&
                       dm > 0
                    ? dm
                    : 0;
            }

            DirectionalMovementMinus result = new DirectionalMovementMinus(Function);
            return result;
        }
    }
}
