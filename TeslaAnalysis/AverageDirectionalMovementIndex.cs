using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using MoreLinq;
using TimeSeriesAnalysis;

namespace TeslaAnalysis
{
    public class AverageDirectionalMovementIndex : Indicator
    {
        public AverageDirectionalMovementIndex(Func<CandleTimeSeries, DateTime, double> function) : base(function)
        {

        }

        public static AverageDirectionalMovementIndex Create(
            TimeSpan period, 
            TimeSpan smoothingPeriod, 
            TimeSpan seriesStep,
            CandleTimeSeries cts,
            bool exponentialMovingAverage = true)
        {
            DirectionalMovementIndex dmi = DirectionalMovementIndex.Create(period);
            TimeSeries ts = new TimeSeries();
            cts.Candles
                .ForEach(candle => ts[candle.Start] = dmi.GetValueAt(cts, candle.Start));
            TimeSeries movingAverage = exponentialMovingAverage
                ? ts.GetExponentialMovingAverage(smoothingPeriod, seriesStep)
                    .ToTimeSeries("ema")
                : ts.GetSimpleMovingAverage(smoothingPeriod)
                    .ToTimeSeries("sma");

            double Function(CandleTimeSeries series, DateTime instant)
            {
                return movingAverage[instant];
            }

            AverageDirectionalMovementIndex index = new AverageDirectionalMovementIndex(Function);
            return index;
        }
    }
}
