using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Common;
using MathNet.Numerics.Statistics;
using MoreLinq;
using OfficeOpenXml;

namespace TimeSeriesAnalysis
{
    public static class ExtensionsDateValue
    {
        private static double tolerance = 1e-15;

        /// <summary>
        /// Transform the IEnumerable<DateValue> into a Tseries object taking the value of the first entry of each date.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TimeSeries ToTimeSeries(
            this IEnumerable<DateValue> list, string name = "TimeSeries")
        {
            return list
                .ToTimeSeries(dvs => dvs.First().Value, name);
        }

        /// <summary>
        /// Transform the IEnumerable<DateValue> into a Tseries object taking the value of the function "f" applied to the 
        /// first entry of each date.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="f"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TimeSeries ToTimeSeries(
            this IEnumerable<DateValue> list,
            Func<IEnumerable<DateValue>, double> f, string name)
        {
            Dictionary<DateTime, double> values = list
                .GroupBy(dv => dv.Date)
                .ToDictionary(g => g.Key,
                              g => f.Invoke(g));

            return new TimeSeries(values)
            {
                Name = name,
            };
        }
        /// <summary>
        /// Transforms the IEnumerable<DateValue> into a dictionary of pairs DateTime - double.
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static Dictionary<DateTime, double> ToDictionary(
            this IEnumerable<DateValue> l)
        {
            return l
                .ToDictionary(dv => dv.Date, dv => dv.Value);
        }
        /// <summary>
        /// Transforms the IEnumerable<DateValue> into a dictionary of pairs double-double. The key elements are the DateTimes transformed to double.
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static Dictionary<double, double> ToNumberDictionary(
            this IEnumerable<DateValue> l)
        {
            return l
                .ToDictionary(dv => dv.Date.ToOADate(),
                    dv => dv.Value);
        }

        #region OPERATIONS

        /// <summary>
        /// Sums the values that are in both IEnumerable<DateValue>.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> Sum(
            this IEnumerable<DateValue> l,
            IEnumerable<DateValue> l2)
        {
            List<DateValue> together = l.ToList();
            together.AddRange(l2);
            return together
                .GroupBy(dv => dv.Date)
                .Where(g => g.Count() > 1)
                .Select(g => new DateValue(g.Key, g.Sum(dv => dv.Value)));
        }
        /// <summary>
        /// Sums to each element of the IEnumerable<DateValue> the offset quantity.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> Sum(
            this IEnumerable<DateValue> l,
            double offset)
        {
            return l.OffsetBy(offset);
        }
        /// <summary>
        /// Multiply each element of the IEnumerable<DateValue> by the factor quantity.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> MultiplyBy(
            this IEnumerable<DateValue> l,
            double factor)
        {
            return l
                .Select(dv => dv.MultiplyBy(factor));
        }
        /// <summary>
        /// Divide each element of the IEnumerable<DateValue> by the factor quantity.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> DivideBy(
            this IEnumerable<DateValue> l,
            double factor)
        {
            return l
                .Select(dv => dv.MultiplyBy(1.0 / factor));
        }
        /// <summary>
        /// Substract the values that are in both IEnumerable<DateValue>.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> Substract(
            this IEnumerable<DateValue> l,
            IEnumerable<DateValue> l2)
        {
            return l.Sum(l2.MultiplyBy(-1));
        }
        /// <summary>
        /// Substract to each element of the IEnumerable<DateValue> the offset quantity.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> Substract(
            this IEnumerable<DateValue> l,
            double offset)
        {
            return l.Sum(-offset);
        }
        /// <summary>
        /// Add values that are missing in the IEnumerable<DateValue> using the functino f. The step parameter is used to define the missing elements in the series. 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="f"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> FillEmptyValuesWith(
            this IEnumerable<DateValue> l,
            Func<DateTime, double> f,
            TimeSpan step)
        {
            Dictionary<DateTime, double> values = l
                .ToDictionary();
            DateTime firstDate = values.Keys
                .Min(date => date);
            DateTime lastDate = values.Keys
                .Max(date => date);

            return ExtensionsDateTime.GetDatesTo(firstDate, lastDate, step)
                .Select(date => new DateValue(date, values.ContainsKey(date) ? values[date] : f.Invoke(date)));
        }
        /// <summary>
        /// Move, a span amount of time, each element of the IEnumerable<DateValue>.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> OffsetBy(
            this IEnumerable<DateValue> l,
            TimeSpan span)
        {
            return l
                .Select(dv => dv.OffsetBy(span));
        }
        /// <summary>
        /// Set the year of each element of the IEnumerable<DateValue>. The method takes into account leap years.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="destinyYear"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> SetYearTo(
            this IEnumerable<DateValue> l,
            int destinyYear)
        {
            IEnumerable<DateValue> listToProcess = !DateTime.IsLeapYear(destinyYear)
                ? l.Where(dv => !ExtensionsDateTime.IsFebruary29S(dv.Date))
                : l;
            return listToProcess
                .Select(dv =>
                {
                    DateTime newDate = new DateTime(destinyYear, dv.Date.Month, dv.Date.Day, dv.Date.Hour, dv.Date.Minute, dv.Date.Second);
                    return dv.SetDate(newDate);
                });
        }
        /// <summary>
        /// Sums to each element of the IEnumerable<DateValue> an offset quantit.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> OffsetBy(
            this IEnumerable<DateValue> l,
            double offset)
        {
            return l
                .Select(dv => dv.OffsetBy(offset));
        }
        /// <summary>
        /// Apply a certain function f to each element of the IEnumerable<DateValue>.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> Apply(
            this IEnumerable<DateValue> l,
            Func<DateValue, DateValue> f)
        {
            return l
                .Select(dv => dv.Apply(f));
        }
        /// <summary>
        /// Returns the first entry of each date in the IEnumerable<DateValue>.
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> Collapse(
            this IEnumerable<DateValue> l)
        {
            return l
                .Collapse(dvs => dvs.First());
        }
        /// <summary>
        /// Returns the entry defined by the function f applied to the IEnumerable<DateValue>.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> Collapse(
            this IEnumerable<DateValue> l,
            Func<IEnumerable<DateValue>, DateValue> f)
        {
            return l
                .GroupBy(dv => dv.Date)
                .Select(f.Invoke);
        }
        /// <summary>
        /// Returns the elements with diferent DateTime in both IEnumerable<DateValue>.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> Merge(
            this IEnumerable<DateValue> l,
            IEnumerable<DateValue> other)
        {
            List<DateValue> dateValues = l
                .ToList();
            dateValues.AddRange(other);
            return dateValues
                .DistinctBy(dv => dv.Date);
        }
        #endregion

        /// <summary>
        /// Remove the average and divides by the deviation.
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> Normalize(this IEnumerable<DateValue> l)
        {
            List<DateValue> values = l.ToList();
            double average = values.Average(dv => dv.Value);
            double deviation = values.StandardDeviation();
            double numberToDivide = Math.Abs(deviation) < tolerance
                ? 1
                : deviation;
            return values
                .Select(dv => dv.Substract(average)
                    .DivideBy(numberToDivide));
        }
        /// <summary>
        /// Transforms the time series to have all values between minValue and maxValue.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> NormalizeBetween(
            this IEnumerable<DateValue> l,
            double minValue,
            double maxValue)
        {
            double newAmplitude = maxValue - minValue;
            double newMiddle = (maxValue + minValue) / 2.0;

            List<DateValue> values = l
                .ToList();
            double max = values
                .Max(dv => dv.Value);
            double min = values
                .Min(dv => dv.Value);
            double middle = (max + min) / 2.0;
            double amplitude = max - min;

            return values
                .Select(dv => dv
                                .Substract(middle)
                                .MultiplyBy(newAmplitude / amplitude)
                                .Sum(newMiddle));
        }
        public static IEnumerable<DateValue> TransformBy(this IEnumerable<DateValue> l, Func<double, double> f)
        {
            return l
                .Select(dv => dv.TransformBy(f));
        }
        public static IEnumerable<DateValue> LogaritmicTransformation(this IEnumerable<DateValue> l)
        {
            return l
                .TransformBy(Math.Log);
        }
        public static IEnumerable<DateValue> ExponentialTransformation(this IEnumerable<DateValue> l)
        {
            return l
                .TransformBy(Math.Exp);
        }
        public static IEnumerable<DateValue> BoxCoxTransformation(this IEnumerable<DateValue> l, double lambda)
        {
            Func<double, double> function = number => Math.Abs(lambda) > tolerance
                ? (Math.Pow(number, lambda) - 1) / lambda
                : Math.Log(number);

            return l
                .TransformBy(function);
        }
        public static IEnumerable<DateValue> BoxCoxInverseTransformation(this IEnumerable<DateValue> l, double lambda)
        {
            Func<double, double> function = number => Math.Abs(lambda) > tolerance
                ? Math.Pow(lambda * number + 1, 1 / lambda)
                : Math.Exp(number);

            return l
                .TransformBy(function);
        }
        public static IEnumerable<DateValue> AdjustBy(this IEnumerable<DateValue> series,
            IEnumerable<DateValue> adjustmentSeries)
        {
            List<DateValue> values = series.ToList();
            values.AddRange(adjustmentSeries);
            return values
                .GroupBy(dv => dv.Date)
                .Select(group =>
                {
                    DateValue value = @group.First();
                    DateValue adjustmentValue = @group.Last();
                    return value.DivideBy(adjustmentValue.Value);
                });
        }

        #region STATISTICS

        /// <summary>
        /// Returns the max value of the IEnumerable<DateValue>.
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static double Max(this IEnumerable<DateValue> l)
        {
            return l.Max(dv => dv.Value);
        }
        /// <summary>
        /// Returns the min value of the IEnumerable<DateValue>.
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static double Min(this IEnumerable<DateValue> l)
        {
            return l.Min(dv => dv.Value);
        }
        /// <summary>
        /// Returns the average of all the values in the IEnumerable<DateValue>.
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static double Average(this IEnumerable<DateValue> l)
        {
            return l.Average(dv => dv.Value);
        }
        /// <summary>
        /// Computes the median of the values in a time series.
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static double Median(this IEnumerable<DateValue> l)
        {
            return l
                .Select(dv => dv.Value)
                .Median();
        }
        /// <summary>
        /// Computes the percentile of the values in a time series.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static double Percentile(
            this IEnumerable<DateValue> l,
            double percentage)
        {
            return l
                .Select(dv => dv.Value)
                .Quantile(percentage);
        }
        /// <summary>
        /// Computes the IQR of a time series values.
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static double InterQuartileRange(this IEnumerable<DateValue> l)
        {
            List<DateValue> values = l.ToList();
            return values.Percentile(0.75) - values.Percentile(0.25);
        }
        /// <summary>
        /// Computes the deviation of a series
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static double StandardDeviation(this IEnumerable<DateValue> l)
        {
            return l
                .Select(dv => dv.Value)
                .Deviation();
        }


        #endregion

        #region FILE INTERACTION

        /// <summary>
        /// Writes a csv of all the entries in the IEnumerable<DateValue> using the specified formats.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="fileName"></param>
        /// <param name="dateFormat">if null, invariant culture is used</param>
        /// <param name="valueFormat">if null, invariant culture is used</param>
        public static void ToCsv(this IEnumerable<DateValue> l, string fileName, string dateFormat = null, string valueFormat = null)
        {
            File.Delete(fileName);
            TextWriter tw = File.AppendText(fileName);
            using (tw)
            {
                l
                    .OrderBy(dv => dv.Date)
                    .ForEach(dv =>
                    {
                        string value = valueFormat == null
                            ? dv.Value.ToString(CultureInfo.InvariantCulture)
                            : dv.Value.ToString(valueFormat);
                        string date = dateFormat == null
                            ? dv.Date.ToString(CultureInfo.InvariantCulture)
                            : dv.Date.ToString(valueFormat);
                        tw.WriteLine($"{date};{value}");
                    });
                tw.Flush();
                tw.Close();
            }
        }
        /// <summary>
        /// Export time series to an excel file.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="fileName"></param>
        /// <param name="sheetName"></param>
        public static void ToExcelFile(this IEnumerable<DateValue> l,
            string fileName, string sheetName)
        {
            using (ExcelPackage file = new ExcelPackage(new FileInfo(fileName)))
            {
                ExcelWorksheet ws = file.Workbook.Worksheets
                                        .FirstOrDefault(worksheet => worksheet.Name == sheetName)
                                    ?? file.Workbook.Worksheets.Add(sheetName);
                ws.Cells[1, 1].Value = nameof(DateValue.Date);
                ws.Cells[1, 2].Value = nameof(DateValue.Value);
                int rowIndex = 2;
                l
                    .OrderBy(dv => dv.Date)
                    .ForEach(dv =>
                    {
                        ws.Cells[rowIndex, 1].Value = dv.Date;
                        ws.Cells[rowIndex, 2].Value = dv.Value;
                        rowIndex++;
                    });

                file.Save();
            }
        }

        #endregion

        /// <summary>
        /// Gets the top values of a series. Those the its neighbours are smaller.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> GetTopValues(this IEnumerable<DateValue> list)
        {
            List<DateValue> values = list
                .ToList();
            int count = values.Count;

            return values
                .OrderBy(dv => dv.Date)
                .Where((dv, index) =>
                {
                    double previousSlope = GetPreviousSlope(index, values, dv);
                    double nextSlope = GetNextSlope(index, count, values, dv);
                    return nextSlope < previousSlope;
                });
        }

        private static double GetNextSlope(int index, int count, List<DateValue> values, DateValue dv)
        {
            double nextSlope = 0;
            if (index < count - 1)
            {
                DateValue dv2 = values[index + 1];
                nextSlope = (dv2.Value - dv.Value) / (dv2.Date.ToOADate() - dv.Date.ToOADate());
            }
            return nextSlope;
        }

        private static double GetPreviousSlope(
            int index,
            IReadOnlyList<DateValue> values,
            DateValue dv)
        {
            double previousSlope = 0;
            if (index > 0)
            {
                DateValue dv1 = values[index - 1];
                previousSlope = (dv.Value - dv1.Value) / (dv.Date.ToOADate() - dv1.Date.ToOADate());
            }

            return previousSlope;
        }

        /// <summary>
        /// Gets the bottom values of a series. Those the its neighbours are larger.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<DateValue> GetBottomValues(this IEnumerable<DateValue> list)
        {
            List<DateValue> values = list
                .ToList();
            int count = values.Count;
            return values
                .OrderBy(dv => dv.Date)
                .Where((dv, index) =>
                {
                    double previousSlope = GetPreviousSlope(index, values, dv);
                    double nextSlope = GetNextSlope(index, count, values, dv);

                    return nextSlope > previousSlope;
                });
        }

        public static IEnumerable<DateValue> GetTendencyLines(
            this IEnumerable<DateValue> list,
            double tol)
        {
            List<DateValue> values = list
                .OrderBy(dv => dv.Date)
                .ToList();
            if (values.Count > 0)
            {
                DateValue currentEdge = values[0];
                yield return currentEdge;
                bool increasing = true;
                for (int index = 1; index < values.Count; index++)
                {
                    DateValue value = values[index];
                    bool valueIsLarger = value.Value >= currentEdge.Value;
                    if (increasing && valueIsLarger ||
                        !increasing && !valueIsLarger)
                        currentEdge = value;
                    if ((increasing && !valueIsLarger ||
                        !increasing && valueIsLarger) &&
                        Math.Abs(value.Value - currentEdge.Value) >= tol)
                    {
                        yield return currentEdge;
                        currentEdge = value;
                        increasing = !increasing;
                    }
                }
                yield return values.Last();
            }
        }

        public static IEnumerable<DateValue> GetIncrements(this IEnumerable<DateValue> list)
        {
            List<DateValue> orderedValues = list
                .OrderBy(dv => dv.Date)
                .ToList();
            for (int index = 1; index < orderedValues.Count; index++)
            {
                DateValue dv = orderedValues[index];
                double increment = dv.Value - orderedValues[index - 1].Value;
                yield return new DateValue(dv.Date, increment);
            }
        }
    }
}
