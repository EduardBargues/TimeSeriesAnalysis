using System;

namespace TimeSeriesAnalysis
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan MultiplyBy(this TimeSpan span, double factor)
        {
            return new TimeSpan((long)(span.Ticks * factor));
        }

        public static TimeSpan DivideBy(this TimeSpan span, double factor)
        {
            return span.MultiplyBy(1/factor);
        }

        public static double DivideBy(this TimeSpan span, TimeSpan span2)
        {
            return (double) span.Ticks / span2.Ticks;
        }
    }
}
