using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeSeriesAnalysis
{
    public class TimeSeriesPredictorNaive : ITimeSeriesPredictor
    {
        private double lastValue;
        public double Predict(DateTime date)
        {
            return lastValue;
        }

        public bool Learn(IEnumerable<DateValue> ts)
        {
            List<DateValue> values = ts.ToList();
            bool ok = values.Any();
            if (ok)
                lastValue = values
                    .OrderBy(dv => dv.Date)
                    .Last().Value;

            return ok;
        }

        public bool Learn(TimeSeries ts)
        {
            return Learn(ts.Values);
        }
    }
}
