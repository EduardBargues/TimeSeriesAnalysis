using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSeriesAnalysis;

namespace UnitTestProject
{
    [TestClass]
    public class TimeSeriesDecompositionTest
    {
        [TestMethod]
        public void ConstantStandardMethod()
        {
            DateTime firstDate = new DateTime(2000, 1, 1);
            DateTime lastDate = new DateTime(2000, 12, 31);
            TimeSeries ts = TimeSeries.CreateDailyConstantTimeSeries(10, firstDate, lastDate);
            DecompositionParameters input = new DecompositionParameters
            {
                SeasonalPeriod = new TimeSpan(days: 30, hours: 0, minutes: 0, seconds: 0),
                MovingAverageDays = new TimeSpan(days: 10, hours: 0, minutes: 0, seconds: 0),
                MaintainExtremeValues = true,
                Step = new TimeSpan(1, 0, 0, 0)
            };
            TimeSeriesDecomposition decomp = TimeSeriesDecomposition.DoDecomposition(ts, input);
            Assert.IsTrue(AreTimeSeriesAlmostEquals(decomp.Trend, ts, 1e-3));
            TimeSeries zeroTimeSeries = TimeSeries.CreateDailyConstantTimeSeries(0, firstDate, lastDate);
            Assert.IsTrue(AreTimeSeriesAlmostEquals(decomp.Seasonal, zeroTimeSeries, 1e-3));
            Assert.IsTrue(AreTimeSeriesAlmostEquals(decomp.Seasonal, zeroTimeSeries, 1e-3));
        }
        [TestMethod]
        public void LinearStandardMethod()
        {
            DateTime firstDate = new DateTime(2000, 1, 1);
            DateTime lastDate = new DateTime(2000, 12, 31);
            TimeSeries ts = TimeSeries.CreateDailyLinearTimeSeries(0, 10, firstDate, lastDate);
            DecompositionParameters input = new DecompositionParameters
            {
                SeasonalPeriod = new TimeSpan(days: 30, hours: 0, minutes: 0, seconds: 0),
                MovingAverageDays = new TimeSpan(days: 10, hours: 0, minutes: 0, seconds: 0),
                MaintainExtremeValues = true,
                Step = new TimeSpan(1, 0, 0, 0)
            };
            TimeSeriesDecomposition decomp = TimeSeriesDecomposition.DoDecomposition(ts, input);
            Assert.IsTrue(AreTimeSeriesAlmostEquals(decomp.Trend, ts, 1e-3));
            TimeSeries zeroTimeSeries = TimeSeries.CreateDailyConstantTimeSeries(0, firstDate, lastDate);
            Assert.IsTrue(AreTimeSeriesAlmostEquals(decomp.Seasonal, zeroTimeSeries, 1e-3));
            Assert.IsTrue(AreTimeSeriesAlmostEquals(decomp.Seasonal, zeroTimeSeries, 1e-3));
        }
        [TestMethod]
        public void SinusStandardMethod()
        {
            DateTime firstDate = new DateTime(2000, 1, 1);
            DateTime lastDate = new DateTime(2000, 12, 31);
            TimeSeries ts = TimeSeries.CreateDailySinusoidalTimeSeries(1, 2 * Math.PI / 365, 0, firstDate, lastDate);
            DecompositionParameters input = new DecompositionParameters
            {
                SeasonalPeriod = new TimeSpan(days: 30, hours: 0, minutes: 0, seconds: 0),
                MovingAverageDays = new TimeSpan(days: 30, hours: 0, minutes: 0, seconds: 0),
                MaintainExtremeValues = true,
                Step = new TimeSpan(1, 0, 0, 0)
            };
            TimeSeriesDecomposition decomp = TimeSeriesDecomposition.DoDecomposition(ts, input);
            Assert.IsTrue(AreTimeSeriesAlmostEquals(decomp.Trend, ts, 1e-3));
            TimeSeries zeroTimeSeries = TimeSeries.CreateDailyConstantTimeSeries(0, firstDate, lastDate);
            Assert.IsTrue(AreTimeSeriesAlmostEquals(decomp.Seasonal, zeroTimeSeries, 1e-3));
            Assert.IsTrue(AreTimeSeriesAlmostEquals(decomp.Seasonal, zeroTimeSeries, 1e-3));
        }
        [TestMethod]
        public void LinearPlusSinusStandardMethod()
        {
        }
        [TestMethod]
        public void LinearPlusRandomErrorStandardMethod()
        {
        }
        [TestMethod]
        public void LinearPlusSinusPlusRandomErrorStandardMethod()
        {
        }
        private bool AreTimeSeriesAlmostEquals(TimeSeries t1, TimeSeries t2, double tolerance)
        {
            return t1.TimeCoordinates
                .Where(t2.ContainsValueAt)
                .All(day => AreNumbersAlmostEquals(t1[day], t2[day], tolerance));
        }

        private bool AreNumbersAlmostEquals(double n1, double n2, double tolerance)
        {
            return Math.Abs(n1 - n2) <= tolerance;
        }
    }
}
