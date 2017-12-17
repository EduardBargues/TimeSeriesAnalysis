using System;

namespace TeslaAnalysis.Indicators
{
    public interface IIndicator
    {
        double GetValueAt(DateTime instant);
        string Name { get; }
        string ShortName { get; }
    }
}
