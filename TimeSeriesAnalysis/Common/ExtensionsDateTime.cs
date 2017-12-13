using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public static class ExtensionsDateTime
    {
        public static bool IsFebruary29S(this DateTime date)
        {
            return date.Day == 29 &&
                   date.Month == 2;
        }
        public static IEnumerable<DateTime> GetDaysTo(this DateTime date, DateTime lastDay)
        {
            return GetDatesTo(date, lastDay, new TimeSpan(1, 0, 0, 0));
        }
        public static IEnumerable<DateTime> GetDatesTo(this DateTime date, DateTime lastDate, TimeSpan span)
        {
            DateTime currentDate = date;
            yield return currentDate;

            int direction = date <= lastDate ? 1 : -1;
            TimeSpan increment = span.MultiplyBy(direction);
            bool IsLastDateReached(DateTime current)
            {
                return direction > 0 ? current > lastDate : current < lastDate;
            }

            currentDate += increment;
            bool lastDateReached = IsLastDateReached(currentDate);
            while (!lastDateReached)
            {
                yield return currentDate;

                currentDate += increment;
                lastDateReached = IsLastDateReached(currentDate);
            }
        }
        public static DateTime Min(params DateTime[] days)
        {
            return days.Min();
        }
        public static DateTime Max(params DateTime[] days)
        {
            return days.Max();
        }
        public static DateTime AddDaysWithoutLeapYear(this DateTime input, int days)
        {
            DateTime output = input;

            if (days == 0)
                return output;

            int increment = days > 0
                ? 1
                : -1; //this will be used to increment or decrement the date.
            int daysAbs = Math.Abs(days); //get the absolute value of days to add
            int daysAdded = 0; // save the number of days added here
            while (daysAdded < daysAbs)
            {
                output = output.AddDays(increment);
                if (!(output.Month == 2 &&
                      output.Day == 29)) //don't increment the days added if it is a leap year day
                    daysAdded++;
            }

            return output;
        }

        public static DateTime Substract(this DateTime date, TimeSpan span)
        {
            return date.Add(span.MultiplyBy(-1));
        }
    }
}
