using System;
using Strategy.Actions;
using TeslaAnalysis;

namespace Strategy.Strategies
{
    public interface IStrategy
    {
        ITransaction GetTradeInfo(CandleTimeSeries series, DateTime date);
    }
}
