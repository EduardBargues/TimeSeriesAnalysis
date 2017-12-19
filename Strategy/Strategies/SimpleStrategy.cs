using System;
using Strategy.Actions;
using TeslaAnalysis;
using TeslaAnalysis.Indicators;

namespace Strategy.Strategies
{
    public class SimpleStrategy : Strategy
    {
        public SimpleStrategy(Func<CandleTimeSeries, DateTime, ITransaction> f) : base(f)
        {
        }

        public static SimpleStrategy Create()
        {
            AverageDirectionalMovementIndex indicator = AverageDirectionalMovementIndex.Create(14, 14);
            ITransaction Function(CandleTimeSeries series, DateTime date)
            {
                double d = indicator[series,date];
                return new Transaction();
            }

            return new SimpleStrategy(Function);
        }
    }
}
