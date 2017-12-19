using System;

namespace TeslaAnalysis.Indicators
{
    public interface IIndicator
    {
        double GetValueAt(CandleTimeSeries series, DateTime instant);
        string Name { get; }
        string ShortName { get; }
    }
}
