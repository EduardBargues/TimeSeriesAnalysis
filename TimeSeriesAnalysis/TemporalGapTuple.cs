using System;

namespace TimeSeriesAnalysis
{
    public class TemporalGapTuple
    {
        private TimeSeries indicator;
        private TimeSeries target;
        private TimeSpan span;

        public TimeSeries Indicator
        {
            get
            {
                return span.Ticks >= 0
                    ? indicator
                    : target;
            }
            set
            {
                indicator = value;
            }
        }
        public TimeSeries Target
        {
            get
            {
                return span.Ticks >= 0
                    ? target
                    : indicator;
            }
            set { target = value; }
        }
        public TimeSpan TemporalGap
        {
            get
            {
                return span.Ticks >= 0
                    ? span
                    : -span;
            }
            set { span = value; }
        }
        public double Correlation { get; set; }

        public TemporalGapTupleId GetId()
        {
            return new TemporalGapTupleId
            {
                TemporalGap = TemporalGap,
                IndicatorName = indicator.Name,
                TargetName = Target.Name
            };
        }
    }

    public struct TemporalGapTupleId
    {
        public string IndicatorName { get; set; }
        public string TargetName { get; set; }
        public TimeSpan TemporalGap { get; set; }
    }
}
