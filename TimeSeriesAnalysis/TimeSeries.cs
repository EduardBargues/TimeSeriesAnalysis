using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CommonUtils;
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
        private readonly Dictionary<DateTime, double> valuesByDate = new Dictionary<DateTime, double>();
        private readonly DateValue[] dateValues;
        private readonly Dictionary<DateTime, int> indicesByDate = new Dictionary<DateTime, int>();

        public IEnumerable<DateTime> Dates => dateValues
            .Select(dv => dv.Date);
        public IEnumerable<DateValue> Values => dateValues;
        public string Name { get; set; }

        // CONSTRUCTORS
        public TimeSeries()
        {

        }
        public TimeSeries(Dictionary<DateTime, double> valsByDate)
        {
            List<KeyValuePair<DateTime, double>> kvps = valsByDate
                .OrderBy(kvp => kvp.Key)
                .ToList();
            int count = kvps.Count;
            dateValues = new DateValue[count];
            kvps
                .ForEach((kvp, index) =>
                {
                    DateValue dv = new DateValue(kvp.Key, kvp.Value);
                    dateValues[index] = dv;
                    valuesByDate.Add(kvp.Key, kvp.Value);
                    indicesByDate.Add(dv.Date, index);
                });
        }
        public TimeSeries(IEnumerable<DateValue> values)
        {
            DateValue[] orderedValues = values
                .OrderBy(dv => dv.Date)
                .ToArray();

            int count = orderedValues.Length;

            dateValues = new DateValue[count];
            orderedValues
                .ForEach((dateValue, index) =>
                {
                    dateValues[index] = dateValue;
                    valuesByDate.Add(dateValue.Date, dateValue.Value);
                    indicesByDate.Add(dateValue.Date, index);
                });
        }

        public static TimeSeries Create(Dictionary<DateTime, double> vals)
        {
            TimeSeries result = new TimeSeries(vals);
            return result;
        }
        public static TimeSeries CreateDailyConstantTimeSeries(double constantValue
            , DateTime firstDate
            , DateTime lastDate)
        {
            return CreateDailyTimeSeries(day => constantValue, firstDate, lastDate);
        }
        public static TimeSeries CreateDailyLinearTimeSeries(double firstValue
            , double lastValue
            , DateTime firstDay
            , DateTime lastDay)
        {
            double range = (lastDay - firstDay).TotalDays;
            double increment = lastValue - firstValue;
            double Function(DateTime day) => firstValue + increment * (day - firstDay).TotalDays / range;
            return CreateDailyTimeSeries(Function, firstDay, lastDay);
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
        public static TimeSeries CreateDailySinusoidalTimeSeries(double a
            , double alpha
            , double betha
            , DateTime firstDay
            , DateTime lastDay)
        {
            double Function(DateTime day) => a * Math.Sin(alpha * (day - firstDay).TotalDays + betha);
            return CreateDailyTimeSeries(Function, firstDay, lastDay);
        }
        public static TimeSeries CreateDailyUniformlyRandomSeries(double average
            , double amplitude
            , DateTime firstDate
            , DateTime lastDate)
        {
            Random random = new Random();
            return CreateDailyTimeSeries(
                function: date => average + (random.NextDouble() - 0.5) * amplitude / 2,
                firstDate: firstDate,
                lastDate: lastDate);
        }
        public static TimeSeries CreateDailyNormallyRandomSeries(double average
            , double deviation
            , DateTime firstDate
            , DateTime lastDate)
        {
            Random random = new Random();
            return CreateDailyTimeSeries(
                function: date => random.NextGaussian(average, deviation),
                firstDate: firstDate,
                lastDate: lastDate);
        }
        public static TimeSeries CreateDailyTimeSeries(Func<DateTime, double> function
            , DateTime firstDate
            , DateTime lastDate)
        {
            Dictionary<DateTime, double> vals = firstDate.GetDaysTo(lastDate)
                .ToDictionary(day => day, function);
            return new TimeSeries(vals);
        }

        // GET VALUES
        public int Count()
        {
            return dateValues.Length;
        }
        public double GetValue(DateTime date)
        {
            return valuesByDate[date];
        }
        public double GetValue(int index)
        {
            return dateValues[index].Value;
        }
        public DateTime GetDate(int index)
        {
            return dateValues[index].Date;
        }
        public double this[DateTime date]
        {
            get => GetValue(date);
            set
            {
                valuesByDate[date] = value;
                int index = indicesByDate[date];
                dateValues[index].Value = value;
            }
        }
        public bool ContainsValueAt(DateTime date)
        {
            return valuesByDate.ContainsKey(date);
        }
        public bool HasValues()
        {
            return dateValues.Any();
        }
        public int GetIndex(DateTime date)
        {
            return indicesByDate[date];
        }
        public double GetDerivative(DateTime date
            , DifferentiationMode mode = DifferentiationMode.Centered
            , TimeScale scale = TimeScale.Days)
        {
            int index = GetIndex(date);
            int leftIndex, rightIndex;
            switch (mode)
            {
                case DifferentiationMode.Left:
                    leftIndex = index - 1;
                    rightIndex = index;
                    break;
                case DifferentiationMode.Centered:
                    leftIndex = index - 1;
                    rightIndex=index + 1;
                    break;
                case DifferentiationMode.Right:
                    leftIndex = index;
                    rightIndex = index + 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            double leftValue = GetValue(leftIndex);
            double rightValue = GetValue(rightIndex);
            DateTime leftDate = GetDate(leftIndex);
            DateTime rightDate = GetDate(rightIndex);
            TimeSpan span = rightDate - leftDate;
            double temporalSpan = GetTemporalSpan(span, scale);

            return (rightValue - leftValue).DivideBy(temporalSpan);
        }
        public double GetDerivative(int index
            , DifferentiationMode mode = DifferentiationMode.Centered
            , TimeScale scale = TimeScale.Days)
        {
            return GetDerivative(GetDate(index), mode, scale);
        }
        private double GetTemporalSpan(TimeSpan span, TimeScale scale)
        {
            switch (scale)
            {
                case TimeScale.Days:
                    return span.TotalDays;
                case TimeScale.Hours:
                    return span.TotalHours;
                case TimeScale.Minutes:
                    return span.TotalMinutes;
                case TimeScale.Seconds:
                    return span.TotalSeconds;
                case TimeScale.Milliseconds:
                    return span.TotalMilliseconds;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scale), scale, null);
            }
        }
        
        #region Apply functions

        public TimeSeries Sum(TimeSeries ts)
        {
            Dictionary<DateTime, double> dictionary = ts.Dates
                .ToDictionary(date => date, date => ts[date] + this[date]);
            return Create(dictionary);
        }
        public TimeSeries Substract(double v)
        {
            Dictionary<DateTime, double> dictionary = Dates
                .ToDictionary(date => date,
                              date => this[date] - v);
            return Create(dictionary);
        }
        public TimeSeries Sum(double offset)
        {
            Dictionary<DateTime, double> dictionary = Dates
                .ToDictionary(date => date,
                    date => this[date] - offset);
            return Create(dictionary);
        }
        public TimeSeries Substract(TimeSeries ts)
        {
            Dictionary<DateTime, double> dictionary = ts.Dates
                .ToDictionary(date => date, date => this[date] - ts[date]);
            return Create(dictionary);
        }
        public TimeSeries MultiplyBy(double factor)
        {
            Dictionary<DateTime, double> dictionary = Dates
                .ToDictionary(date => date,
                              date => factor * this[date]);
            return Create(dictionary);
        }
        public TimeSeries Merge(TimeSeries ts, TimeSeriesMergeMode mergeAction = TimeSeriesMergeMode.MantainOriginal)
        {
            Dictionary<DateTime, double> dictionary = this.Dates
                .Union(ts.Dates)
                .ToDictionary(date => date, date =>
                {
                    switch (mergeAction)
                    {
                        case TimeSeriesMergeMode.Override:
                            return ts[date];
                        case TimeSeriesMergeMode.Sum:
                            return (this.ContainsValueAt(date) ? this[date] : 0) +
                                   (ts.ContainsValueAt(date) ? ts[date] : 0);
                        case TimeSeriesMergeMode.MantainOriginal:
                            return this.ContainsValueAt(date) ? this[date] : ts[date];
                        default:
                            throw new ArgumentOutOfRangeException(nameof(mergeAction), mergeAction, null);
                    }
                });
            return Create(dictionary);
        }

        public TimeSeries Apply(Func<DateTime, double, bool> selectFunction = null
            , Func<DateTime, double, DateTime> dateFunction = null
            , Func<DateTime, double, double> valueFunction = null)
        {
            bool Select(DateValue dv)
            {
                return selectFunction?.Invoke(dv.Date, dv.Value) ?? true;
            }

            return Values
                .Where(Select)
                .Select(dv => dv.Apply(dateFunction, valueFunction))
                .Union(Values
                            .Where(dv => !Select(dv)))
                .ToTimeSeries();
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
        public TimeSeries OffsetBy(TimeSpan span)
        {
            Func<DateTime, double, DateTime> f = (day, value) => day.Add(span);
            return Apply(dateFunction: f);
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
                selectFunction: (date, value) => selectionFunction(date),
                valueFunction: (date, value) => value + offset);
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
            return Dates
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
            return Dates.Min();
        }
        public DateTime GetMaximumDate()
        {
            return Dates.Max();
        }
        public IEnumerable<DateTime> GetDatesBetween(DateTime firstDate, DateTime lastDate)
        {
            return Dates
                .Where(day => firstDate <= day &&
                              day <= lastDate);
        }
        public IEnumerable<DateValue> GetValuesBetween(DateTime firstDate, DateTime lastDate)
        {
            return Values
                .Where(value => value.Date >= firstDate &&
                                value.Date <= lastDate);

        }

        #region PLOT

        public static void Plot(TimeSeries ts)
        {
            FunctionSeries series = new FunctionSeries
            {
                MarkerType = MarkerType.None,
                Color = OxyColor.FromRgb(255, 0, 0)
            };
            foreach (DateTime day in ts.Dates.OrderBy(day => day))
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
        public static PlotView GetPlotView(IEnumerable<TimeSeriesPlotInfo> parameters, Action<object, AxisChangedEventArgs> onAxisChangedMethod = null)
        {
            PlotModel model = new PlotModel();

            parameters
                .OrderByDescending(pi => pi.Order)
                .ForEach(pi => GetDataPointSeries(pi).ForEach(dps => model.Series.Add(dps)));

            model.Axes.Clear();
            DateTimeAxis axis = new DateTimeAxis();
            if (onAxisChangedMethod != null)
                axis.AxisChanged += onAxisChangedMethod.Invoke;

            model.Axes.Add(axis);
            PlotView view = new PlotView
            {
                Model = model,
                Dock = DockStyle.Fill
            };
            return view;
        }

        private static IEnumerable<DataPointSeries> GetDataPointSeries(TimeSeriesPlotInfo pi)
        {
            if (pi.SeriesType == typeof(FunctionSeries))
                return GetFunctionSeries(pi);
            //if (pi.SeriesType == typeof(LinearBarSeries))
            //    return GetLinearBarSeries(pi);

            throw new NotImplementedException("El tipo de gráfico no ha sido implementado");
        }
        private static DataPointSeries GetLinearBarSeries(TimeSeriesPlotInfo info)
        {
            LinearBarSeries series = new LinearBarSeries
            {
                FillColor = OxyColor.FromArgb(info.Color.A, info.Color.R, info.Color.G, info.Color.B),
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
        private static List<FunctionSeries> GetFunctionSeries(TimeSeriesPlotInfo info)
        {
            List<FunctionSeries> series = new List<FunctionSeries>();
            List<DateValue> values = info.Series.Values
                .OrderBy(dv => dv.Date)
                .ToList();
            Color previousColor = values.Any()
                ? info.ColorFunction.Invoke(values.First())
                : Color.Blue;
            FunctionSeries currentSeries = new FunctionSeries
            {
                MarkerType = info.Marker,
                Title = info.LegendTitleFunction.Invoke(previousColor),
                RenderInLegend = true,
                Color = OxyColor.FromArgb(previousColor.A, previousColor.R, previousColor.G, previousColor.B)
            };
            HashSet<Color> plottedColors = new HashSet<Color>();
            values
                .ForEach(dv =>
                {
                    double dayNumeric = DateTimeAxis.ToDouble(dv.Date);
                    DataPoint point = new DataPoint(dayNumeric, dv.Value);
                    Color color = info.ColorFunction.Invoke(dv);
                    if (color != previousColor)
                    {
                        if (currentSeries.Points.Count >= 2)
                        {
                            plottedColors.Add(previousColor);
                            series.Add(currentSeries);
                        }
                        currentSeries = new FunctionSeries
                        {
                            MarkerType = info.Marker,
                            Title = info.LegendTitleFunction.Invoke(color),
                            Color = OxyColor.FromArgb(color.A, color.R, color.G, color.B),
                            RenderInLegend = !plottedColors.Contains(color),
                        };
                    }
                    currentSeries.Points.Add(point);
                    previousColor = color;
                });
            if (currentSeries.Points.Count >= 2)
                series.Add(currentSeries);

            return series;
        }
        public static PlotView GetPlotView(params TimeSeriesPlotInfo[] parameters)
        {
            return GetPlotView(parameters.ToList());
        }


        #endregion

        public static IEnumerable<DateTime> GetCommonDates(List<TimeSeries> tss)
        {
            return tss.First().Dates
                .Where(date => tss.All(ts => ts.ContainsValueAt(date)));
        }

        public DateTime GetNextSpanDate(DateTime date, TimeSpan span)
        {
            // TODO: cada serie sabe como añadir el periods.
            // si es una serie diaria puede tener la modalidad de omitir los 29s de febrero.
            return date.Add(span);
        }

        public bool Any()
        {
            return HasValues();
        }

        #region Moving Averages

        public IEnumerable<DateValue> GetSimpleMovingAverage(int periods)
        {
            return from dv in dateValues
                   let average = indicesByDate[dv.Date].GetIntegersTo(Math.Max(0, indicesByDate[dv.Date] - periods))
                                    .Average(index => dateValues[index].Value)
                   select new DateValue(dv.Date, average);
        }
        public IEnumerable<DateValue> GetCenteredMovingAverage(int periods)
        {
            return GetCenteredMovingAverage(periods / 2, periods / 2);
        }
        public IEnumerable<DateValue> GetCenteredMovingAverage(int leftPeriods, int rightPeriods)
        {
            int maxIndex = dateValues.Length - 1;
            return from dv in dateValues
                   let leftIndex = Math.Max(0, indicesByDate[dv.Date] - leftPeriods)
                   let rightIndex = Math.Min(maxIndex, indicesByDate[dv.Date] + rightPeriods)
                   let average = leftIndex.GetIntegersTo(rightIndex)
                                    .Average(index => dateValues[index].Value)
                   select new DateValue(dv.Date, average);
        }
        public IEnumerable<DateValue> GetExponentialMovingAverage(int periods)
        {
            return from dv in dateValues
                   let valuesToUse = indicesByDate[dv.Date].GetIntegersTo(Math.Max(0, indicesByDate[dv.Date] - periods))
                                        .Select(index => dateValues[index])
                                        .ToArray()
                   let ema = valuesToUse
                                .WeightedAverage(function: (dateValue, index) => dateValue.Value,
                                                 weightFunction: (dateValue, index) => valuesToUse.Length - index)
                   select new DateValue(dv.Date, ema);
        }

        #endregion
    }
}