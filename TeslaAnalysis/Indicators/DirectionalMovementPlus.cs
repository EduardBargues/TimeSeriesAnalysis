using System;
using Common;

namespace TeslaAnalysis.Indicators
{
    public class DirectionalMovementPlus : Indicator
    {
        public DirectionalMovementPlus(Func<CandleTimeSeries, DateTime, double> function) : base(function)
        {
        }

        public static DirectionalMovementPlus Create()
        {
            double Function(CandleTimeSeries series, DateTime instant)
            {
                Candle currentCandle = series[instant];
                int index = series.GetIndex(currentCandle);
                Candle previousCandle = series[index - 1];
                double um = Utils.Max(currentCandle.Max - previousCandle.Max, 0);
                double dm = Utils.Max(previousCandle.Min - currentCandle.Min, 0);
                return um > dm && 
                       um > 0
                    ? um
                    : 0;
            }

            DirectionalMovementPlus result = new DirectionalMovementPlus(Function);
            return result;
        }
    }
}
