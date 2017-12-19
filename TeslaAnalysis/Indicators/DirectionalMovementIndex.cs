using System;
using Common;

namespace TeslaAnalysis.Indicators
{
    public class DirectionalMovementIndex : Indicator
    {
        public DirectionalMovementIndex(Func<CandleTimeSeries, DateTime, double> function) : base(function)
        {

        }

        public static DirectionalMovementIndex Create(int periods, int smoothingPeriods)
        {
            DirectionalIndicatorPlus diPlus = DirectionalIndicatorPlus.Create(periods, smoothingPeriods);
            DirectionalIndicatorMinus diMinus = DirectionalIndicatorMinus.Create(periods, smoothingPeriods);
            double Function(CandleTimeSeries series, DateTime instant)
            {
                double diDiff = Math.Abs(diPlus[series, instant] - diMinus[series, instant]);
                double diSum = diPlus[series, instant] + diMinus[series, instant];
                double dx = diDiff.DivideBy(diSum);
                return dx;
            }

            DirectionalMovementIndex index = new DirectionalMovementIndex(Function);
            return index;
        }
    }
}
