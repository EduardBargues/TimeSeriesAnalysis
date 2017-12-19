using System;
using Common;

namespace TeslaAnalysis.Indicators
{
    public class DirectionalIndicatorMinus : Indicator
    {
        public DirectionalIndicatorMinus(Func<CandleTimeSeries, DateTime, double> function) : base(function)
        {

        }

        public static DirectionalIndicatorMinus Create(int periods, int smoothingPeriods)
        {
            AverageDirectionalMovementMinus admMinus = AverageDirectionalMovementMinus.Create(periods, smoothingPeriods);
            AverageTrueRange atr = AverageTrueRange.Create(smoothingPeriods);

            double Function(CandleTimeSeries series, DateTime instant)
            {
                return admMinus[series, instant].DivideBy(atr[series, instant]);
            }

            DirectionalIndicatorMinus result = new DirectionalIndicatorMinus(Function);
            return result;
        }
    }
}
