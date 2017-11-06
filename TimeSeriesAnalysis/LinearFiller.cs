using System;

namespace TimeSeriesAnalysis
{
    public class LinearFiller : IDateValuesFiller
    {
        public DateValue GetValueAt(DateTime day, TimeSeries ts)
        {
            DateTime? leftDay = ts.GetClosestDay(day: day, lookInTheFuture: false);
            DateTime? rightDay = ts.GetClosestDay(day: day, lookInTheFuture: true);

            double? valueLeft = leftDay != null
                ? ts[date: leftDay.Value]
                : (double?)null;
            double? valueRight = rightDay != null
                ? ts[date: rightDay.Value]
                : (double?)null;

            double? value;
            if (valueLeft != null &&
                valueRight != null)
            {
                double timeSpan = (rightDay.Value - leftDay.Value).Hours;
                double leftTimeSpan = (day - leftDay.Value).Hours;
                double increment = valueLeft.Value - valueRight.Value;
                value = valueLeft.Value + increment / timeSpan * leftTimeSpan;
            }
            else
            {
                value = valueLeft 
                    ?? valueRight.Value;
            }

            return new DateValue(date: day, value: value.Value);
        }
    }
}