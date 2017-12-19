using System;
using Strategy.Actions;
using TeslaAnalysis;

namespace Strategy.Strategies
{
    public abstract class Strategy : IStrategy
    {
        private readonly Func<CandleTimeSeries, DateTime, ITransaction> f;
        protected Strategy(Func<CandleTimeSeries, DateTime, ITransaction> f)
        {
            this.f = f;
        }

        public ITransaction GetTradeInfo(CandleTimeSeries series, DateTime date)
        {
            return f(series, date);
        }
    }
}
