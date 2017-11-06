using System;
using System.Collections.Generic;

namespace TimeSeriesAnalysis
{
    public interface ITimeSeriesPredictor
    {
        double Predict(DateTime date);
        bool Learn(IEnumerable<DateValue> ts);
        bool Learn(TimeSeries ts);
    }
}
