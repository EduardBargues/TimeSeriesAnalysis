using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSeriesAnalysis;
using System.Data;
using MachineLearning;
using MoreLinq;

namespace UnitTestProject
{
    [TestClass]
    public class TimeSeriesPredictionTest
    {
        private double tolerance = 1e-10;
        [TestMethod]
        public void CorrelationSame()
        {
            TimeSeries lts = TimeSeries.CreateDailyLinearTimeSeries(0, 1, new DateTime(2000, 1, 1), new DateTime(2000, 12, 31));
            double correlation = TimeSeriesCorrelation.GetCorrelationBetween(lts, lts);
            Assert.IsTrue(Math.Abs(correlation - 1) < tolerance);
        }
        [TestMethod]
        public void CorrelationMultiply()
        {
            TimeSeries lts = TimeSeries.CreateDailySinusoidalTimeSeries(1, 1, 1, new DateTime(2000, 1, 1), new DateTime(2000, 12, 31));
            TimeSeries lts2 = lts.MultiplyBy(2);
            double correlation = TimeSeriesCorrelation.GetCorrelationBetween(lts, lts2);
            Assert.IsTrue(Math.Abs(correlation - 1) < tolerance);
        }
        [TestMethod]
        public void CorrelationSinus()
        {
            TimeSeries sts = TimeSeries.CreateDailySinusoidalTimeSeries(1, 2, 0, new DateTime(2000, 1, 1), new DateTime(2000, 12, 31));
            CorrelationAnalysisParameters parameters = new CorrelationAnalysisParameters()
            {
                Span = new TimeSpan(1, 0, 0, 0),
                NumberOfSpansInthePast = 10,
                NumberOfSpansIntheFuture = 10,
            };
            List<TemporalGapTuple> analysis = TimeSeriesCorrelation.DoCorrelationAnalysis(sts, sts, parameters)
                .ToList();
            TemporalGapTuple maxCorrelationAnalysis = analysis
                .MaxBy(an => an.Correlation);

            Assert.IsTrue(Math.Abs(maxCorrelationAnalysis.Correlation - 1) < tolerance);
            Assert.IsTrue(maxCorrelationAnalysis.Indicator == maxCorrelationAnalysis.Target &&
                          maxCorrelationAnalysis.TemporalGap.Ticks == 0);
            Assert.IsTrue(analysis.All(an => an.Correlation <= maxCorrelationAnalysis.Correlation));
        }
        [TestMethod]
        public void LinearPredictor1Indicator()
        {
            TimeSeries indicator = TimeSeries.CreateDailySinusoidalTimeSeries(1, 2, 0, new DateTime(2000, 1, 1), new DateTime(2000, 12, 31));
            indicator.Name = "INDICATOR";
            TimeSeries target = indicator.OffsetBy(new TimeSpan(10, 0, 0, 0));
            target.Name = "TARGET";
            CorrelationAnalysisParameters parameters = new CorrelationAnalysisParameters()
            {
                Span = new TimeSpan(1, 0, 0, 0),
                NumberOfSpansInthePast = 10,
                NumberOfSpansIntheFuture = -10,
            };
            IEnumerable<TemporalGapTuple> analysis = TimeSeriesCorrelation.DoCorrelationAnalysis(indicator, target, parameters);
            TemporalGapGroup g = new TemporalGapGroup()
            {
                Pairs = analysis
                    .Where(a => Math.Abs(a.Correlation - 1) < tolerance)
                    .ToList(),
            };
            TimeSpan predictionSpan = new TimeSpan(5, 0, 0, 0);
            DataTable table = g.GetPredictionTable(target.Name, predictionSpan);
            LinearPredictor linearPredictor = new LinearPredictor();
            linearPredictor.Learn(
                table: table,
                trainingLabel: indicator.Name,
                targetLabel: target.Name);

            TimeSpan correlationSpan = new TimeSpan(10, 0, 0, 0);
            Assert.IsTrue(target.Dates
                .Where(day => target.ContainsValueAt(day.Add(predictionSpan)) &&
                              indicator.ContainsValueAt(day.Add(-correlationSpan)) &&
                              indicator.ContainsValueAt(day.Add(-correlationSpan).Add(predictionSpan)))
                .All(day =>
                {
                    double incrementIndicator = indicator[day.Add(-correlationSpan).Add(predictionSpan)] - indicator[day.Add(-correlationSpan)];
                    DataRow row = table.NewRow();
                    row[indicator.Name] = incrementIndicator;
                    return Math.Abs((double)linearPredictor.Predict(row) - incrementIndicator) < tolerance;
                }));
        }
        [TestMethod]
        public void LinearPredictor2Indicators()
        {
            DateTime firstDate = new DateTime(2000, 1, 1);
            DateTime lastDate = new DateTime(2000, 12, 31);
            TimeSeries ind1 = TimeSeries.CreateDailyLinearTimeSeries(0, 100, firstDate, lastDate);
            ind1.Name = "ind1";
            TimeSeries ind2 = TimeSeries.CreateDailySinusoidalTimeSeries(1, 2 * Math.PI / 365, 0, firstDate, lastDate);
            ind2.Name = "ind2";
            List<TimeSeries> indicators = new List<TimeSeries>() { ind1, ind2 };
            TimeSpan predictionSpan = new TimeSpan(5, 0, 0, 0);
            TimeSpan correlationSpan = new TimeSpan(10, 0, 0, 0);
            TimeSeries target = ind1
                .Sum(ind2.MultiplyBy(2))
                .OffsetBy(correlationSpan);
            target.Name = "target";
            TemporalGapGroup g = new TemporalGapGroup()
            {
                Pairs = new List<TemporalGapTuple>()
                {
                    new TemporalGapTuple()
                    {
                        Indicator = ind1,
                        Target = target,
                        TemporalGap = correlationSpan,
                    },
                    new TemporalGapTuple()
                    {
                        Indicator = ind2,
                        Target = target,
                        TemporalGap = correlationSpan,
                    },
                }
            };
            DataTable table = g.GetPredictionTable(target.Name, predictionSpan);
            LinearPredictor linearPredictor = new LinearPredictor() { UseIntercept = false };
            linearPredictor.Learn(
                table: table,
                trainingLabels: new List<string>() { ind1.Name, ind2.Name },
                targetLabel: target.Name);
            
            Assert.IsTrue(target.Dates
                .Where(day => target.ContainsValueAt(day.Add(predictionSpan)) &&
                              indicators.All(ind => ind.ContainsValueAt(day.Add(-correlationSpan)) &&
                                                    ind.ContainsValueAt(day.Add(-correlationSpan).Add(predictionSpan))))
                .All(day =>
                {
                    DataRow row = table.NewRow();
                    indicators
                        .ForEach(ind => row[ind.Name] = ind[day.Add(-correlationSpan).Add(predictionSpan)] - ind[day.Add(-correlationSpan)]);
                    double predictedTargetIncrement = (double)linearPredictor.Predict(row);
                    double realTargetIncrement = target[day.Add(predictionSpan)] - target[day];
                    return Math.Abs(predictedTargetIncrement - realTargetIncrement) < tolerance;
                }));
        }
    }
}
