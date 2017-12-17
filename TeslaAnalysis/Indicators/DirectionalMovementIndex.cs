using System;
using Common;

namespace TeslaAnalysis.Indicators
{
    public class DirectionalMovementIndex : Indicator
    {
        public DirectionalMovementIndex(Func<DateTime, double> function) : base(function)
        {

        }

        public static DirectionalMovementIndex Create(CandleTimeSeries series, int periods, int smoothingPeriods)
        {
            DirectionalIndicatorPlus diPlus = DirectionalIndicatorPlus.Create(series, periods, smoothingPeriods);
            DirectionalIndicatorMinus diMinus = DirectionalIndicatorMinus.Create(series, periods, smoothingPeriods);
            double Function(DateTime instant)
            {
                double diDiff = Math.Abs(diPlus[instant] - diMinus[instant]);
                double diSum = diPlus[instant] + diMinus[instant];
                double dx = diDiff.DivideBy(diSum);
                return dx;
            }

            DirectionalMovementIndex index = new DirectionalMovementIndex(Function);
            return index;
        }
    }
}
