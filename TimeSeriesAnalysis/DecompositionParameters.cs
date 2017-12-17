using System;

namespace TimeSeriesAnalysis
{
    public class DecompositionParameters
    {
        public int MovingAverageDays { get; set; }
        public int SeasonalPeriod { get; set; }
        public bool MaintainExtremeValues { get; set; }
    }
}