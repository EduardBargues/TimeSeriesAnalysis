using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeSeriesAnalysis
{
    public class TimeSeriesPredictorAverage : ITimeSeriesPredictor
    {
        private double average;
        public double Predict(DateTime date)
        {
            return average;
        }
        public bool Learn(IEnumerable<DateValue> ts)
        {
            List<DateValue> values = ts.ToList();
            bool ok = values.Any();
            if (ok)
                average = values.Average(dv => dv.Value);
            return ok;
        }
        public bool Learn(TimeSeries ts)
        {
            return Learn(ts.Values);
        }
    }
}
