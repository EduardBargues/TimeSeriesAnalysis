using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace TeslaAnalysis.Indicators
{
    public class DirectionalIndicatorPlus : Indicator
    {
        public DirectionalIndicatorPlus(Func<DateTime, double> function) : base(function)
        {

        }

        public static DirectionalIndicatorPlus Create(CandleTimeSeries series, int periods, int smoothingPeriods)
        {
            AverageDirectionalMovementPlus admPlus = AverageDirectionalMovementPlus.Create(series, periods, smoothingPeriods);
            AverageTrueRange atr = AverageTrueRange.Create(series, smoothingPeriods);

            double Function(DateTime instant)
            {
                return admPlus.GetValueAt(instant).DivideBy(atr.GetValueAt(instant));
            }

            DirectionalIndicatorPlus result = new DirectionalIndicatorPlus(Function);
            return result;
        }
    }
}
