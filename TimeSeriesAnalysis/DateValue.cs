using System;

namespace TimeSeriesAnalysis
{
    public struct DateValue
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }

        public DateValue(DateTime date, double value)
        {
            Date = date;
            Value = value;
        }

        public DateValue Sum(double offset)
        {
            return OffsetBy(offset);
        }
        public DateValue Substract(double offset)
        {
            return Sum(-offset);
        }
        public DateValue SetValue(double newValue)
        {
            return new DateValue(Date, newValue);
        }
        public DateValue SetDate(DateTime newDate)
        {
            return new DateValue(newDate, Value);
        }
        public DateValue CreateDateValue(DateTime newDate, double newValue)
        {
            return new DateValue(newDate, newValue);
        }
        public DateValue OffsetBy(double offset)
        {
            return new DateValue(Date, Value + offset);
        }
        public DateValue OffsetBy(TimeSpan offset)
        {
            return new DateValue(Date + offset, Value);
        }
        public DateValue MultiplyBy(double factor)
        {
            return new DateValue(Date, Value * factor);
        }
        public DateValue DivideBy(double numberToDivide)
        {
            return MultiplyBy(1 / numberToDivide);
        }
        public DateValue TransformBy(Func<double, double> func)
        {
            return new DateValue(Date, func.Invoke(this.Value));
        }
        public DateValue Apply(
            Func<DateTime, double, DateTime> offsetFunction,
            Func<DateTime, double, double> valueFunction)
        {
            return new DateValue(offsetFunction?.Invoke(Date, Value) ?? Date,
                                 valueFunction?.Invoke(Date, Value) ?? Value);
        }
        public DateValue Apply(Func<DateValue, DateValue> f)
        {
            return f.Invoke(this);
        }
    }
}
