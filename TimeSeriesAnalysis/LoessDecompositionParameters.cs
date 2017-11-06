using System;

namespace TimeSeriesAnalysis
{
    public class LoessDecompositionParameters
    {
        public TimeSpan SeasonalPeriod { get; set; }
        public TimeSpan Step { get; set; }
        public TimeSeries Series { get; set; }
        public int NumberOfNeighbors { get; set; }
        public int RobustnessIterations { get; set; }
        public int LocalPolynomialDegree { get; set; }
    }
}
