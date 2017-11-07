using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using MoreLinq;

namespace TimeSeriesAnalysis
{
    public class TimeSeriesDecomposition
    {
        public TimeSeries Tseries => Trend
            .Sum(Seasonal)
            .Sum(Remainder);
        public TimeSeries Trend { get; set; } = new TimeSeries();
        public TimeSeries Seasonal { get; set; } = new TimeSeries();
        public TimeSeries Remainder { get; set; } = new TimeSeries();

        #region StandardDecomposition

        public static TimeSeriesDecomposition DoDecomposition(
            TimeSeries ts,
            DecompositionParameters parameters)
        {
            TimeSeriesDecomposition result = new TimeSeriesDecomposition();
            if (ts.Any())
            {
                result.Trend = ts.GetCenteredMovingAverage(
                    parameters.MovingAverageDays,
                    parameters.Step)
                    .ToTimeSeries();
                TimeSeries seasonalComponent = GetSeasonalComponent(
                    ts,
                    parameters.SeasonalPeriod,
                    parameters.Step);
                result.Seasonal = seasonalComponent.Substract(ts.Average());
                result.Remainder = ts
                    .Substract(result.Trend)
                    .Substract(result.Seasonal);
            }

            return result;
        }


        private static TimeSeries GetSeasonalComponent(
            TimeSeries tseries,
            TimeSpan seasonalPeriod,
            TimeSpan step)
        {
            TimeSeries result = new TimeSeries();

            GetSeasonalCycleSubseries(
                    tseries,
                    seasonalPeriod,
                    step)
                .ForEach(series =>
                {
                    double average = series.Dates
                        .Average(day => series[day]);
                    TimeSeries timeSeries = series.Values
                        .Select(dv => dv.SetValue(average))
                        .ToTimeSeries();
                    result = result.Merge(timeSeries);
                });

            return result;
        }
        #endregion

        #region LoessDecomposition

        public static TimeSeriesDecomposition DoLoessDecomposition(LoessDecompositionParameters parameters)
        {
            TimeSeriesDecomposition result = new TimeSeriesDecomposition();
            TimeSeries series = parameters.Series;
            if (series.Any())
            {
                result.Trend = SmoothTimeSeriesWithLoess(
                        series.Values,
                        series.Dates,
                        parameters.NumberOfNeighbors,
                        parameters.RobustnessIterations,
                        parameters.LocalPolynomialDegree,
                        series.Dates.ToDictionary(date => date, date => 1.0))
                    .ToTimeSeries();
                result.Seasonal = GetSeasonalComponent(
                        series,
                        parameters.SeasonalPeriod,
                        parameters.Step)
                    .Substract(series.Average());
                result.Remainder = series
                    .Substract(result.Trend)
                    .Substract(result.Seasonal);
            }

            return result;
        }
        private static IEnumerable<DateValue> SmoothTimeSeriesWithLoess(
            IEnumerable<DateValue> series,
            IEnumerable<DateTime> daysToEvaluateSmoothedTimeSeries,
            int numberOfNeighbors,
            int robustnessIterations,
            int localPolynomialDegree,
            Dictionary<DateTime, double> robustnessWeights)
        {
            Dictionary<double, double> seriesByXCoordinates = series
                .ToNumberDictionary();
            Dictionary<double, double> robustnessWeightsNumeric = robustnessWeights.Keys
                .ToDictionary(day => day.ToOADate(),
                    day => robustnessWeights[day]);
            LoessParameters loessParameters = new LoessParameters
            {
                Series = seriesByXCoordinates,
                XsToEvaluate = daysToEvaluateSmoothedTimeSeries
                    .Select(day => day.ToOADate())
                    .ToArray(),
                LocalPolynomialDegree = localPolynomialDegree,
                NumberOfNeighbors = numberOfNeighbors,
                RobustnessIterations = robustnessIterations,
                RobustnessWeights = robustnessWeightsNumeric
            };
            Dictionary<double, double> smoothedDict = Loess.Smooth(loessParameters);
            return smoothedDict.Keys
                .Select(date => new DateValue(DateTime.FromOADate(date), smoothedDict[date]));
        }

        #endregion

        #region STLDecomposition

        //public static TimeSeriesDecomposition DoStlDecomposition(
        //    Tseries ts,
        //    StlDecompositionParameters parameters)
        //{
        //    TimeSeriesDecomposition result = null;
        //    Tseries tseries = ts.Copy();
        //    if (tseries.HasValues())
        //    {
        //        DateTime firstDate = tseries.GetFirstDate();
        //        DateTime lastDate = tseries.GetLastDate();

        //        result = new TimeSeriesDecomposition
        //        {
        //            Trend = Tseries.CreateConstantTimeSeries(constantValue: 0, firstDate: firstDate, lastDate: lastDate),
        //        };
        //        result.Trend.RemoveFebruary29S();

        //        Dictionary<DateTime, double> robustnessWeights = result.Trend.TimeCoordinates
        //            .ToDictionary(keySelector: day => day,
        //                elementSelector: day => 1.0);
        //        for (int passIndex = 0; passIndex < parameters.OuterLoopPasses; passIndex++)
        //        {
        //            result = GetIncrementDecomposition(
        //                tseries: tseries,
        //                tsd: result,
        //                robustnessWeights: robustnessWeights,
        //                parameters: parameters);
        //            double weightThreshold = GetWeightThreshold(ts: result.Remainder);
        //            robustnessWeights = GetTimeWeights(ts: tseries, threshold: weightThreshold);
        //        }
        //    }
        //    return result;
        //}
        //private static TimeSeriesDecomposition GetIncrementDecomposition(
        //    Tseries tseries,
        //    TimeSeriesDecomposition tsd,
        //    Dictionary<DateTime, double> robustnessWeights,
        //    StlDecompositionParameters parameters)
        //{
        //    TimeSeriesDecomposition result = new TimeSeriesDecomposition();

        //    // (1) DETRENDING
        //    Tseries detrendedSeries = tseries.Substract(ts: tsd.Trend);
        //    // (2) CYCLE-SUBSERIES SMOOTHING
        //    Tseries smoothedSeasonalSeries = DoCycleSubseriesSmoothing(
        //        tseries: detrendedSeries,
        //        seasonalCycleObservations: parameters.SeasonalCycleObservations,
        //        seasonalComponentSmoothingParameter: parameters.SeasonalComponentSmoothingParameter,
        //        robustnessWeights: robustnessWeights);
        //    Tseries.Plot(ts: smoothedSeasonalSeries);

        //    // (3) LOW PASS FILTERING OF SMOOTHED CYCLE-SUBSERIES
        //    Tseries lowPassFilteredSeries = DoLowPassFiltering(
        //        tseries: smoothedSeasonalSeries,
        //        seasonalCycleObservations: parameters.SeasonalCycleObservations,
        //        lowPassFilterSmoothingParameter: parameters.LowPassFilterSmoothingParameter,
        //        robustnessWeights: robustnessWeights);
        //    // (4) DETRENDING OF SMOOTHED CYCLE-SUBSERIES
        //    result.Seasonal = smoothedSeasonalSeries.Substract(ts: lowPassFilteredSeries);
        //    // (5) DESEASONALIZING
        //    Tseries deseasonalizedSeries = tseries.Substract(ts: result.Seasonal);
        //    // (6) TREND SMOOTHING
        //    result.Trend = SmoothTimeSeriesWithLoess(
        //        series: deseasonalizedSeries,
        //        daysToEvaluateSmoothedTimeSeries: deseasonalizedSeries.TimeCoordinates.ToArray(),
        //        numberOfNeighbors: parameters.TrendComponentSmoothingParameter,
        //        robustnessIterations: 1,
        //        localPolynomialDegree: 1,
        //        robustnessWeights: robustnessWeights);
        //    result.Remainder = tseries
        //        .Substract(ts: result.Trend)
        //        .Substract(ts: result.Seasonal);

        //    return result;
        //}
        //private static Tseries DoLowPassFiltering(
        //    Tseries tseries,
        //    int seasonalCycleObservations,
        //    int lowPassFilterSmoothingParameter,
        //    Dictionary<DateTime, double> robustnessWeights)
        //{
        //    Tseries result = GetCenteredMovingAvegare(
        //        tseries: tseries,
        //        span: seasonalCycleObservations,
        //        maintainExtremeDays: false);
        //    result = GetCenteredMovingAvegare(
        //        tseries: result,
        //        span: seasonalCycleObservations,
        //        maintainExtremeDays: false);
        //    result = GetCenteredMovingAvegare(
        //        tseries: result,
        //        span: 3,
        //        maintainExtremeDays: false);
        //    result = SmoothTimeSeriesWithLoess(
        //        series: result,
        //        daysToEvaluateSmoothedTimeSeries: result.TimeCoordinates.ToArray(),
        //        numberOfNeighbors: lowPassFilterSmoothingParameter,
        //        robustnessIterations: 1,
        //        localPolynomialDegree: 1,
        //        robustnessWeights: robustnessWeights);
        //    return result;
        //}

        //private static Tseries DoCycleSubseriesSmoothing(
        //    Tseries tseries,
        //    int seasonalCycleObservations,
        //    int seasonalComponentSmoothingParameter,
        //    Dictionary<DateTime, double> robustnessWeights)
        //{
        //    Tseries cycleSubSerie = new Tseries();

        //    GetSeasonalCycleSubseries(
        //            tseries: tseries,
        //            seasonalPeriod: seasonalCycleObservations)
        //        .ForEach(action: series =>
        //        {
        //            DateTime firstDate = series.GetFirstDate();
        //            DateTime firstDateBefore = firstDate.AddDaysWithoutLeapYear(days: -seasonalCycleObservations);
        //            series[date: firstDateBefore] = series[date: firstDate];
        //            robustnessWeights.Add(key: firstDateBefore, value: robustnessWeights[key: firstDate]);

        //            DateTime lastDate = series.GetLastDate();
        //            DateTime lastDateAfter = lastDate.AddDaysWithoutLeapYear(days: seasonalCycleObservations);
        //            series[date: lastDateAfter] = series[date: lastDate];
        //            robustnessWeights.Add(key: lastDateAfter, value: robustnessWeights[key: lastDate]);

        //            Tseries smoothedSeries = SmoothTimeSeriesWithLoess(
        //                series: series,
        //                daysToEvaluateSmoothedTimeSeries: series.TimeCoordinates.ToArray(),
        //                numberOfNeighbors: seasonalComponentSmoothingParameter,
        //                robustnessIterations: 1,
        //                localPolynomialDegree: 1,
        //                robustnessWeights: robustnessWeights);
        //            cycleSubSerie = cycleSubSerie.AddMissingValuesFrom(ts: smoothedSeries);
        //        });

        //    return cycleSubSerie;
        //}


        //private static Dictionary<DateTime, double> GetTimeWeights(Tseries ts, double threshold)
        //{
        //    double actualThreshold = threshold > 0
        //        ? threshold
        //        : 1;

        //    Dictionary<DateTime, double> weights = ts.TimeCoordinates
        //        .ToDictionary(keySelector: date => date,
        //            elementSelector: date => MathFunction.GetBisquare(u: ts[date: date] / actualThreshold));

        //    return weights;
        //}
        //private static double GetWeightThreshold(Tseries ts)
        //{
        //    double[] doubles = ts.TimeCoordinates
        //        .Select(selector: day => ts[date: day])
        //        .ToArray();
        //    return 6 * Math.Abs(value: Measures.Median(values: doubles));
        //}


        #endregion

        #region Common

        public static IEnumerable<TimeSeries> GetSeasonalCycleSubseries(
            TimeSeries tseries,
            TimeSpan seasonalPeriod,
            TimeSpan step)
        {
            Dictionary<DateTime, int> groups = new Dictionary<DateTime, int>();

            if (tseries.HasValues())
            {
                DateTime firstDate = tseries.GetMinimumDate();
                DateTime lastDate = tseries.GetMaximumDate();

                int groupLabel = 1;
                TimeSpan correctedSeasonalPeriod = seasonalPeriod - step;
                DateTime firstDateInterval = firstDate;
                DateTime firstDatePlus1SeasonalPeriod = tseries.GetNextSpanDate(firstDateInterval, correctedSeasonalPeriod);
                DateTime lastDateInterval = ExtensionsDateTime.GetMinDate(firstDatePlus1SeasonalPeriod, lastDate);
                while (firstDateInterval < lastDateInterval)
                {
                    ExtensionsDateTime.GetDatesTo(firstDateInterval, lastDateInterval, step)
                        .Where(tseries.ContainsValueAt)
                        .ForEach(day => groups.Add(day, groupLabel++));

                    firstDateInterval = lastDateInterval.Add(step);
                    firstDatePlus1SeasonalPeriod = tseries.GetNextSpanDate(firstDateInterval, correctedSeasonalPeriod);
                    lastDateInterval = ExtensionsDateTime.GetMinDate(firstDatePlus1SeasonalPeriod, lastDate);
                    groupLabel = 1;
                }
            }

            return groups.Keys
                .GroupBy(day => groups[day])
                .Select(days => new TimeSeries(days.ToDictionary(day => day, day => tseries[day])));
        }
        #endregion
    }
}
