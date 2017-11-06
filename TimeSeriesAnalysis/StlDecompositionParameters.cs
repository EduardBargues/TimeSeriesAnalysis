namespace TimeSeriesAnalysis
{
    public class StlDecompositionParameters
    {
        public int SeasonalCycleObservations { get; set; }
        public int InnerLoopPasses { get; set; }
        public int OuterLoopPasses { get; set; }
        public int LowPassFilterSmoothingParameter { get; set; }
        public int TrendComponentSmoothingParameter { get; set; }
        public int SeasonalComponentSmoothingParameter { get; set; }
    }
}
