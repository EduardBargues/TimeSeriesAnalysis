using System;
using Common;

namespace TeslaAnalysis.Indicators
{
    public class DirectionalIndicatorMinus:Indicator
    {
        public DirectionalIndicatorMinus(Func<DateTime, double> function) : base(function)
        {

        }

        public static DirectionalIndicatorMinus Create(CandleTimeSeries series, int periods, int smoothingPeriods)
        {
            AverageDirectionalMovementMinus admMinus = AverageDirectionalMovementMinus.Create(series, periods, smoothingPeriods);
            AverageTrueRange atr = AverageTrueRange.Create(series, smoothingPeriods);

            double Function(DateTime instant)
            {
                return admMinus.GetValueAt(instant).DivideBy(atr.GetValueAt(instant));
            }

            DirectionalIndicatorMinus result = new DirectionalIndicatorMinus(Function);
            return result;
        }
    }
}
