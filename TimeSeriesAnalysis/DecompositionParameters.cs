using System;

namespace TimeSeriesAnalysis
{
    public class DecompositionParameters
    {
        public TimeSpan MovingAverageDays { get; set; }
        public TimeSpan SeasonalPeriod { get; set; }
        public bool MaintainExtremeValues { get; set; }
        public TimeSpan Step { get; set; }
    }

}