using System;

namespace TimeSeriesAnalysis
{
    public interface IDateValuesFiller
    {
        DateValue GetValueAt(DateTime day, TimeSeries ts);
    }
}