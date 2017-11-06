using System;
using System.Collections.Generic;

namespace TimeSeriesAnalysis
{
    public class CorrelationAnalysisParameters
    {
        public TimeSpan Span { get; set; }
        public int NumberOfSpansInthePast { get; set; }
        public int NumberOfSpansIntheFuture { get; set; }

        public IEnumerable<TimeSpan> GetTimeSpans()
        {
            for (int number = -NumberOfSpansInthePast; number <= NumberOfSpansIntheFuture; number++)
                yield return new TimeSpan(Span.Ticks * number);
        }
    }
}