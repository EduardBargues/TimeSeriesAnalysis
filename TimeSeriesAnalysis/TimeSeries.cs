using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Common;
using MoreLinq;
using OfficeOpenXml;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace TimeSeriesAnalysis
{
    public class TimeSeries
    {
        private readonly Dictionary<DateTime, double> values = new Dictionary<DateTime, double>();

        public IEnumerable<DateTime> TimeCoordinates => values.Keys;
        public IEnumerable<DateValue> Values => values.Keys
            .Select(date => new DateValue(date, this[date]));
        public string Name { get; set; }
        // CONSTRUCTORS
        public TimeSeries()
        {

        }
        public TimeSeries(Dictionary<DateTime, double> values)
        {
            this.values = values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        public static TimeSeries CreateDailyConstantTimeSeries(double constantValue, DateTime firstDate, DateTime lastDate)
        {
            return CreateDailyTimeSeries(day => constantValue, firstDate, lastDate);
        }
        public static TimeSeries CreateDailyLinearTimeSeries(double firstValue, double lastValue, DateTime firstDay, DateTime lastDay)
        {
            double range = (lastDay - firstDay).TotalDays;
            double increment = lastValue - firstValue;
            Func<DateTime, double> function = day => firstValue + increment * (day - firstDay).TotalDays / range;

            return CreateDailyTimeSeries(function, firstDay, lastDay);
        }


        /// <summary>
        /// Creates a time series with the form : ts= A * sin{alpha * (day - firstDay) + betha}
        /// </summary>
        /// <param name="a"></param>
        /// <param name="alpha"></param>
        /// <param name="betha"></param>
        /// <param name="firstDay"></param>
        /// <param name="lastDay"></param>
        /// <returns></returns>
        public static TimeSeries CreateDailySinusoidalTimeSeries(double a, double alpha, double betha, DateTime firstDay, DateTime lastDay)
        {
            Func<DateTime, double> function = day => a * Math.Sin(alpha * (day - firstDay).TotalDays + betha);
            return CreateDailyTimeSeries(function, firstDay, lastDay);
        }
        public static TimeSeries CreateDailyUniformlyRandomSeries(double average, double amplitude, DateTime firstDate, DateTime lastDate)
        {
            Random random = new Random();
            return CreateDailyTimeSeries(
                function: date => average + (random.NextDouble() - 0.5) * amplitude / 2,
                firstDate: firstDate,
                lastDate: lastDate);
        }

        public static TimeSeries CreateDailyNormallyRandomSeries(double average, double deviation, DateTime firstDate,
            DateTime lastDate)
        {
            Random random = new Random();
            return CreateDailyTimeSeries(
                function: date => random.NextGaussian(average, deviation),
                firstDate: firstDate,
                lastDate: lastDate);
        }

        public static TimeSeries CreateDailyTimeSeries(Func<DateTime, double> function, DateTime firstDate, DateTime lastDate)
        {
            Dictionary<DateTime, double> vals = firstDate.GetDaysTo(lastDate)
                .ToDictionary(day => day,
                              function.Invoke);

            return new TimeSeries(vals);
        }
        // SET, GET VALUES
        public void SetValue(DateTime date, double value)
        {
            if (!ContainsValueAt(date))
                values.Add(date, 0);
            values[date] = value;
        }
        public double GetValue(DateTime date)
        {
            return values[date];
        }
        public bool TryGetValue(DateTime day, out double value)
        {
            value = 0;
            return values.TryGetValue(day, out value);
        }
        public double this[DateTime date]
        {
            get
            {
                return GetValue(date);
            }
            set
            {
                SetValue(date, value);
            }
        }
        public bool ContainsValueAt(DateTime date)
        {
            return values.ContainsKey(date);
        }
        public bool HasValues()
        {
            return TimeCoordinates.Any();
        }

        #region Apply functions

        public TimeSeries Sum(TimeSeries ts)
        {
            return Apply(
                (date, value) => ts.ContainsValueAt(date),
                (date, value) => value + ts[date]);
        }
        public TimeSeries Substract(double v)
        {
            return Values
                .Substract(v)
                .ToTimeSeries();
        }

        public IEnumerable<DateValue> Sum(double offset)
        {
            return Values
                .Sum(offset);
        }

        public TimeSeries Substract(TimeSeries ts)
        {
            return Sum(ts.MultiplyBy(-1));
        }
        public TimeSeries MultiplyBy(double factor)
        {
            return Apply((date, value) => value * factor);
        }
        public TimeSeries Merge(TimeSeries ts)
        {
            return TimeCoordinates
                .Union(ts.TimeCoordinates)
                .Select(date => new DateValue(date, ContainsValueAt(date)
                                                        ? this[date]
                                                        : ts[date]))
                .ToTimeSeries();
        }

        public TimeSeries AddMissingValuesWith(Func<DateTime, double> function)
        {
            Dictionary<DateTime, double> vals = HasValues()
                ? GetMinimumDate().GetDaysTo(GetMaximumDate())
                    .ToDictionary(day => day,
                                  day => ContainsValueAt(day)
                                      ? this[day]
                                      : function.Invoke(day))
                : new Dictionary<DateTime, double>();

            return new TimeSeries(vals);
        }
        //public Tseries AddMissingValuesWith(IDateValuesFiller filler)
        //{
        //    return HasValues()
        //        ? GetFirstDate().GetDatesTo(GetLastDate(), Span)
        //            .Where(day => !ContainsValueAt(day))
        //            .Select(day => filler.GetValueAt(day, this))
        //            .Union(Values)
        //            .ToTimeSeries()
        //        : new Tseries();
        //}

        public TimeSeries Apply(
            Func<DateTime, double, bool> selectFunction,
            Func<DateTime, double, DateTime> offsetFunction,
            Func<DateTime, double, double> valueFunction)
        {
            return Values
                .Where(dv => selectFunction.Invoke(dv.Date, dv.Value))
                .Select(dv => dv.Apply(offsetFunction, valueFunction))
                .Union(Values.Where(dv => !selectFunction.Invoke(dv.Date, dv.Value)))
                .ToTimeSeries();
        }

        public TimeSeries Apply(
            Func<DateTime, double, bool> selectFunction,
            Func<DateTime, double, DateTime> offsetFunction)
        {
            return Apply(
                selectFunction,
                offsetFunction,
                (date, value) => value);
        }
        public TimeSeries Apply(
            Func<DateTime, double, bool> selectFunction,
            Func<DateTime, double, double> valueFunction)
        {
            return Apply(
                selectFunction,
                (date, value) => date,
                valueFunction);
        }
        public TimeSeries Apply(
            Func<DateTime, double, DateTime> offsetFunction,
            Func<DateTime, double, double> valueFunction)
        {
            return Apply(
                (date, value) => true,
                offsetFunction,
                valueFunction);
        }

        public TimeSeries Apply(Func<DateTime, double, bool> selectFunction)
        {
            return Apply(
                selectFunction,
                (date, value) => date,
                (date, value) => value);
        }
        public TimeSeries RemoveDatesWhere(Func<DateTime, bool> selectFunction)
        {
            return Apply((date, value) => selectFunction.Invoke(date));
        }
        public TimeSeries RemoveFebruary29S()
        {
            Func<DateTime, bool> predicate = date => date.Month == 2 &&
                                                     date.Day == 29;
            return RemoveDatesWhere(predicate);
        }
        public TimeSeries RemoveValueAt(DateTime day)
        {
            return RemoveDatesWhere(date => date == day);
        }
        public TimeSeries RemoveValuesBetween(DateTime firstDate, DateTime lastDay)
        {
            Func<DateTime, bool> f = date => date >= firstDate &&
                                             date <= lastDay;
            return RemoveDatesWhere(f);
        }

        public TimeSeries Apply(Func<DateTime, double, DateTime> offsetFunction)
        {
            return Apply(
                (date, value) => true,
                offsetFunction,
                (date, value) => value);
        }
        public TimeSeries Apply(Func<DateTime, DateTime> offsetFunction)
        {
            return Apply((date, value) => offsetFunction.Invoke(date));
        }
        public TimeSeries OffsetBy(TimeSpan span)
        {
            Func<DateTime, DateTime> f = day => day.Add(span);
            return Apply(f);
        }

        public TimeSeries Apply(Func<DateTime, double, double> valueFunction)
        {
            return Apply(
                (date, value) => true,
                (date, value) => date,
                valueFunction);
        }
        public TimeSeries Apply(Func<DateTime, double> valueFunction)
        {
            return Apply((date, value) => valueFunction.Invoke(date));
        }
        public TimeSeries OffsetBy(double offset)
        {
            return OffsetBy(
                date => true,
                offset);
        }
        public TimeSeries OffsetBy(Func<DateTime, bool> selectionFunction, double offset)
        {
            return Apply(
                (date, value) => selectionFunction.Invoke(date),
                (date, value) => value + offset);
        }
        #endregion

        #region QueryFunctions

        public IEnumerable<DateValue> Where(Func<DateValue, bool> selectFunction)
        {
            return Values
                .Where(selectFunction.Invoke);
        }

        public double Average()
        {
            return TimeCoordinates
                .Average(date => this[date]);
        }
        #endregion
        public static IEnumerable<DateValue> FromCsvFile(
            string fileName,
            char separator = ';',
            CultureInfo numberCulture = null,
            string dateFormat = null,
            CultureInfo dateCulture = null)
        {
            string text = File.ReadAllText(fileName);
            return FromString(text, separator, numberCulture, dateFormat, dateCulture);
        }

        public static IEnumerable<DateValue> FromString(
            string text,
            char separator = ';',
            CultureInfo numberCulture = null,
            string dateFormat = null,
            CultureInfo dateCulture = null)
        {
            if (numberCulture == null)
                numberCulture = CultureInfo.InvariantCulture;
            if (string.IsNullOrEmpty(dateFormat))
                dateFormat = "yyyy/MM/dd";
            if (dateCulture == null)
                dateCulture = CultureInfo.InvariantCulture;
            foreach (string line in text.Split('\n').Where(line => !string.IsNullOrEmpty(line)))
            {
                string[] strings = line.Split(separator);
                if (strings.Length == 2)
                {
                    double value;
                    DateTime date;
                    bool isANumber = double.TryParse(strings[1], NumberStyles.Number, numberCulture, out value);
                    bool isADate = DateTime.TryParse(strings[0], dateCulture, DateTimeStyles.None, out date);
                    if (isADate && isANumber)
                        yield return new DateValue(date, value);
                }
            }
        }
        public static IEnumerable<DateValue> FromExcelFile(string fileName, string sheetName, string columnName)
        {
            using (ExcelPackage file = new ExcelPackage(new FileInfo(fileName)))
            {
                ExcelWorksheet ws = file.Workbook.Worksheets
                    .FirstOrDefault(worksheet => worksheet.Name == sheetName);

                if (ws != null)
                    using (ws)
                    {
                        int columnHandle = 1;
                        string columnLabel = ws.Cells[1, columnHandle].Value.ToString();
                        bool isTheColumn = columnLabel == columnName;
                        while (!isTheColumn &&
                               !string.IsNullOrEmpty(columnLabel))
                        {
                            columnHandle++;
                            columnLabel = ws.Cells[1, columnHandle].Value.ToString();
                            isTheColumn = columnLabel == columnName;

                        }
                        if (isTheColumn)
                            return FromExcelWorkSheet(ws, columnHandle, true);
                    }
            }
            return new List<DateValue>();
        }
        public static IEnumerable<DateValue> FromExcelFile(string fileName, string sheetName, int columnHandle, bool hasTitles = true)
        {
            using (ExcelPackage file = new ExcelPackage(new FileInfo(fileName)))
            {
                ExcelWorksheet ws = file.Workbook.Worksheets
                    .FirstOrDefault(worksheet => worksheet.Name == sheetName);

                if (ws != null)
                    using (ws)
                        return FromExcelWorkSheet(ws, columnHandle, hasTitles);
            }

            return new List<DateValue>();
        }

        private static List<DateValue> FromExcelWorkSheet(ExcelWorksheet ws, int columnHandle, bool hasTitles)
        {
            List<DateValue> output = new List<DateValue>();
            int rowHandle = hasTitles ? 2 : 1;
            ws.Cells.Reset();
            bool isCellEmpty = string.IsNullOrEmpty(ws.Cells[rowHandle, 1].Value?.ToString()) ||
                               string.IsNullOrEmpty(ws.Cells[rowHandle, columnHandle].Value?.ToString());
            while (!isCellEmpty)
            {
                DateValue dv = new DateValue(DateTime.FromOADate((double)ws.Cells[rowHandle, 1].Value),
                    (double)ws.Cells[rowHandle, columnHandle].Value);
                output.Add(dv);
                rowHandle++;
                isCellEmpty = string.IsNullOrEmpty(ws.Cells[rowHandle, 1].Value?.ToString());
            }

            return output;
        }
        public IEnumerable<DateTime> GetCommonDates(TimeSeries ts)
        {
            return GetCommonDates(new List<TimeSeries> { this, ts });
        }
        public DateTime GetMinimumDate()
        {
            return TimeCoordinates.Min();
        }
        public DateTime GetMaximumDate()
        {
            return TimeCoordinates.Max();
        }
        public IEnumerable<DateTime> GetDatesBetween(DateTime firstDate, DateTime lastDate)
        {
            return TimeCoordinates
                .Where(day => firstDate <= day &&
                              day <= lastDate);
        }
        public IEnumerable<DateValue> GetValuesBetween(DateTime firstDate, DateTime lastDate)
        {
            return Values
                .Where(value => value.Date >= firstDate &&
                                value.Date <= lastDate);

        }
        public object Clone()
        {
            return new TimeSeries(values);
        }

        public DateTime? GetClosestDay(DateTime day, bool lookInTheFuture)
        {
            DateTime? closestDate = null;
            if (HasValues())
            {
                DateTime relativeDate = lookInTheFuture
                    ? GetMaximumDate()
                    : GetMinimumDate();
                closestDate = day.GetDaysTo(relativeDate)
                    .FirstOrDefault(ContainsValueAt);

                closestDate = closestDate == DateTime.MinValue
                    ? null
                    : closestDate;
            }

            return closestDate;
        }
        public IEnumerable<DateTime> GetMissingDates()
        {
            return HasValues()
                ? GetMinimumDate().GetDaysTo(GetMaximumDate())
                    .Where(day => !ContainsValueAt(day))
                : new List<DateTime>();
        }

        public double GetSlopeAt(DateTime date)
        {
            double slope = 0;

            if (HasValues())
            {
                DateTime firstDate = GetMinimumDate();
                DateTime lastDate = GetMaximumDate();
                if (firstDate >= date)
                {
                    DateTime nextDate = GetDatesBetween(firstDate.AddDays(1), lastDate)
                        .FirstOrDefault();
                    if (nextDate != DateTime.MinValue)
                    {
                        double range = (nextDate - firstDate).TotalDays;
                        if (range > 0)
                            slope = (this[nextDate] - this[firstDate]) / range;
                    }
                }
                if (lastDate <= date)
                {
                    DateTime previousDate = lastDate.GetDaysTo(firstDate)
                        .FirstOrDefault(day => ContainsValueAt(day) &&
                                               day < lastDate);
                    if (previousDate != DateTime.MinValue)
                    {
                        double range = (lastDate - previousDate).TotalDays;
                        if (range > 0)
                            slope = (this[lastDate] - this[previousDate]) / range;
                    }
                }
                if (firstDate < date &&
                    lastDate > date)
                {
                    DateTime leftDay = TimeCoordinates
                        .Where(day => day <= date)
                        .MinBy(day => (date - day).TotalDays);
                    DateTime rightDay = TimeCoordinates
                        .Where(day => day >= date)
                        .MinBy(day => (day - date).TotalDays);
                    double range = (rightDay - leftDay).TotalDays;
                    if (range > 0)
                        slope = (this[rightDay] - this[leftDay]) / range;
                }
            }

            return slope;
        }
        public DateTime? GetNextDate(DateTime date)
        {
            DateTime? result = null;

            if (HasValues())
            {
                DateTime lastDate = GetMaximumDate();
                if (lastDate >= date.AddDays(1))
                    result = date.AddDays(1).GetDaysTo(lastDate)
                        .First(ContainsValueAt);
            }

            return result;
        }
        public DateTime? GetPreviousDate(DateTime date)
        {
            DateTime? result = null;

            if (HasValues())
            {
                DateTime firstDate = GetMinimumDate();
                if (firstDate <= date.AddDays(-1))
                    result = date.AddDays(-1).GetDaysTo(firstDate)
                        .First(ContainsValueAt);
            }

            return result;
        }

        #region PLOT

        public static void Plot(TimeSeries ts)
        {
            FunctionSeries series = new FunctionSeries
            {
                MarkerType = MarkerType.None,
                Color = OxyColor.FromRgb(255, 0, 0)
            };
            foreach (DateTime day in ts.TimeCoordinates.OrderBy(day => day))
            {
                double dayNumeric = DateTimeAxis.ToDouble(day);
                series.Points.Add(new DataPoint(dayNumeric, ts[day]));
            }

            PlotModel model = new PlotModel();
            model.Series.Add(series);
            model.Axes.Clear();
            model.Axes.Add(new DateTimeAxis());
            PlotView view = new PlotView
            {
                Model = model,
                Dock = DockStyle.Fill
            };

            Form form = new Form();
            form.Controls.Add(view);
            form.ShowDialog();
        }
        public static void Plot(params TimeSeriesPlotInfo[] parameters)
        {
            Plot(parameters.ToList());
        }
        public static void Plot(IEnumerable<TimeSeriesPlotInfo> parameters)
        {
            PlotView view = GetPlotView(parameters);

            Form form = new Form();
            form.Controls.Add(view);
            form.ShowDialog();
        }
        public static PlotView GetPlotView(IEnumerable<TimeSeriesPlotInfo> parameters)
        {
            PlotModel model = new PlotModel();

            parameters
                .OrderByDescending(pi => pi.Order)
                .ForEach(pi => model.Series.Add(GetDataPointSeries(pi)));

            model.Axes.Clear();
            model.Axes.Add(new DateTimeAxis());
            PlotView view = new PlotView
            {
                Model = model,
                Dock = DockStyle.Fill
            };
            return view;
        }

        private static DataPointSeries GetDataPointSeries(TimeSeriesPlotInfo pi)
        {
            if (pi.SeriesType == typeof(FunctionSeries))
                return GetFunctionSeries(pi);
            if (pi.SeriesType == typeof(LinearBarSeries))
                return GetLinearBarSeries(pi);

            throw new NotImplementedException("El tipo de gráfico no ha sido implementado");
        }
        private static DataPointSeries GetLinearBarSeries(TimeSeriesPlotInfo info)
        {
            LinearBarSeries series = new LinearBarSeries
            {
                FillColor = info.Color,
                Title = info.Title,
            };
            info.Series.Values
                .OrderBy(dv => dv.Date)
                .ForEach(dv =>
                {
                    double dayNumeric = DateTimeAxis.ToDouble(dv.Date);
                    series.Points.Add(new DataPoint(dayNumeric, dv.Value));
                });
            return series;
        }
        private static FunctionSeries GetFunctionSeries(TimeSeriesPlotInfo info)
        {
            FunctionSeries series = new FunctionSeries
            {
                MarkerType = info.Marker,
                Color = info.Color,
                Title = info.Title,
            };
            info.Series.Values
                .OrderBy(dv => dv.Date)
                .ForEach(dv =>
                {
                    double dayNumeric = DateTimeAxis.ToDouble(dv.Date);
                    series.Points.Add(new DataPoint(dayNumeric, dv.Value));
                });

            return series;
        }
        public static PlotView GetPlotView(params TimeSeriesPlotInfo[] parameters)
        {
            return GetPlotView(parameters.ToList());
        }


        #endregion

        public static IEnumerable<DateTime> GetCommonDates(List<TimeSeries> tss)
        {
            return tss.First().TimeCoordinates
                .Where(date => tss.All(ts => ts.ContainsValueAt(date)));
        }

        public TimeSeries Copy()
        {
            return new TimeSeries(values)
            {
                Name = Name
            };
        }

        public DateTime GetNextSpanDate(DateTime date, TimeSpan span)
        {
            // TODO: cada serie sabe como añadir el span.
            // si es una serie diaria puede tener la modalidad de omitir los 29s de febrero.
            return date.Add(span);
        }

        public bool Any()
        {
            return HasValues();
        }

        public TimeSeries GetSimpleMovingAverage(
            TimeSpan span)
        {
            TimeSeries result = new TimeSeries();

            if (HasValues())
            {
                List<DateTime> days = TimeCoordinates
                    .ToList();
                Values
                    .ForEach(dv =>
                    {
                        DateTime leftDate = dv.Date.Add(-span);
                        double average = days
                            .Where(day2 => day2 >= leftDate &&
                                           day2 <= dv.Date)
                            .Average(day2 => this[day2]);
                        result[dv.Date] = average;
                    });
            }

            return result;
        }
        public TimeSeries GetCenteredMovingAverage(
            TimeSpan span)
        {
            return GetCenteredMovingAverage(span.DivideBy(2), span.DivideBy(2));
        }
        public TimeSeries GetCenteredMovingAverage(
            TimeSpan leftSpan,
            TimeSpan rightSpan)
        {
            TimeSeries result = new TimeSeries();

            if (HasValues())
            {
                DateTime firstDate = GetMinimumDate();
                DateTime lastDate = GetMaximumDate();
                List<DateTime> days = TimeCoordinates
                    .ToList();
                TimeCoordinates
                    .Where(day => day.Add(-leftSpan) >= firstDate &&
                                  day.Add(rightSpan) <= lastDate)
                    .ForEach(day =>
                    {
                        DateTime leftDate = day.Add(-leftSpan);
                        DateTime rightDate = day.Add(rightSpan);
                        List<double> rangeValues = days
                            .Where(day2 => day2 >= leftDate &&
                                           day2 <= rightDate)
                            .Select(day2 => this[day2])
                            .ToList();
                        if (rangeValues.Any())
                            result[day] = rangeValues.Average();
                    });
            }

            return result;
        }
        public IEnumerable<DateValue> GetExponentialMovingAverage(
            TimeSpan span,
            TimeSpan step)
        {
            int n = (int)span.DivideBy(step);
            double weight = (double)2 / (n + 1);

            double previousEmaValue = 0;
            foreach (DateTime date in GetMinimumDate().GetDatesTo(GetMaximumDate(), step))
            {
                double value = weight * this[date] + previousEmaValue * (1 - weight);
                previousEmaValue = value;
                yield return new DateValue(date, value);
            }
        }
    }
}