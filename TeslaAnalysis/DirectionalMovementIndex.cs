using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace TeslaAnalysis
{
    public class DirectionalMovementIndex : Indicator
    {
        public DirectionalMovementIndex(Func<CandleTimeSeries, DateTime, double> function) : base(function)
        {

        }

        public static DirectionalMovementIndex Create(TimeSpan period)
        {
            DirectionalIndicator diPlus = DirectionalIndicator.Create(period);
            DirectionalIndicator diMinus = DirectionalIndicator.Create(period, plus: false);
            double Function(CandleTimeSeries series, DateTime instant)
            {
                double diPlusValue = diPlus.GetValueAt(series, instant);
                double diMinusValue = diMinus.GetValueAt(series, instant);
                double diDiff = Math.Abs(diPlusValue - diMinusValue);
                double diSum = diPlusValue + diMinusValue;
                double dx = diDiff.DivideBy(diSum);
                return dx;
            }

            DirectionalMovementIndex index = new DirectionalMovementIndex(Function);
            return index;
        }
    }
}
