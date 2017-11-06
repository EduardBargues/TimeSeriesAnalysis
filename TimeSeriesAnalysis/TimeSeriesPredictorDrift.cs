using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeSeriesAnalysis
{
    public class TimeSeriesPredictorDrift : ITimeSeriesPredictor
    {
        private DateValue firstValue;
        private DateValue lastValue;
        public double Predict(DateTime date)
        {
            double slope = (lastValue.Value - firstValue.Value) / (lastValue.Date - firstValue.Date).TotalHours;
            return lastValue.Value + slope * (date - lastValue.Date).TotalHours;
        }
        public bool Learn(IEnumerable<DateValue> ts)
        {
            List<DateValue> values = ts.ToList();
            bool ok = values.Count >= 2;
            if (ok)
            {
                List<DateValue> orderedValues = values
                    .OrderBy(dv => dv.Date)
                    .ToList();
                firstValue = orderedValues.First();
                lastValue = orderedValues.Last();

                ok = (firstValue.Date - lastValue.Date).Ticks > 0;
            }

            return ok;
        }
        public bool Learn(TimeSeries ts)
        {
            throw new NotImplementedException();
        }
    }
}
