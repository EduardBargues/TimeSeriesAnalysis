using System;
using System.Linq;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSeriesAnalysis;

namespace UnitTestProject
{
    [TestClass]
    public class TimeSeriesOperationsTest
    {
        private double tolerance = 1e-10;

        [TestMethod]
        public void Sum()
        {
            TimeSeries ts = TimeSeries.CreateDailyLinearTimeSeries(0, 1, new DateTime(2000, 1, 1), new DateTime(2000, 12, 31));
            TimeSeries sumTs = ts.Sum(ts);
            Assert.IsTrue(sumTs.Dates
                .All(day => Math.Abs(sumTs[day] - 2 * ts[day]) < tolerance));
        }
        [TestMethod]
        public void MultiplyBy()
        {
            TimeSeries ts = TimeSeries.CreateDailyLinearTimeSeries(0, 1, new DateTime(2000, 1, 1), new DateTime(2000, 12, 31));
            TimeSeries timesTs = ts.MultiplyBy(5);
            Assert.IsTrue(timesTs.Dates
                .All(day => Math.Abs(timesTs[day] - 5 * ts[day]) < tolerance));
        }
        [TestMethod]
        public void Substract()
        {
            TimeSeries ts = TimeSeries.CreateDailyLinearTimeSeries(0, 1, new DateTime(2000, 1, 1), new DateTime(2000, 12, 31));
            TimeSeries substractTs = ts.Substract(ts);
            Assert.IsTrue(substractTs.Dates
                .All(day => Math.Abs(substractTs[day]) < tolerance));
        }
        [TestMethod]
        public void MergeSeries()
        {
            DateTime firstDate = new DateTime(2000, 1, 1);
            DateTime lastDate = new DateTime(2010, 1, 1);
            TimeSeries ts = TimeSeries.CreateDailyLinearTimeSeries(0, 1, firstDate, new DateTime(2000, 12, 31));
            ts[lastDate] = 0;

            TimeSeries constantTs = TimeSeries.CreateDailyConstantTimeSeries(0, new DateTime(2001, 1, 1), lastDate);
            TimeSeries tsWithMissingValues = ts.Merge(constantTs);

            Assert.IsTrue(firstDate.GetDaysTo(lastDate)
                .All(day => tsWithMissingValues.ContainsValueAt(day)));
        }

        [TestMethod]
        public void Normalization()
        {
            DateTime firstDate = new DateTime(2000, 1, 1);
            DateTime lastDate = new DateTime(2000, 12, 31);
            TimeSeries ind1 = TimeSeries.CreateDailyLinearTimeSeries(0, 100, firstDate, lastDate);
            ind1.Name = "ind1";
            TimeSeries ind2 = TimeSeries.CreateDailySinusoidalTimeSeries(1, 2 * Math.PI / 365, 0, firstDate, lastDate);
            ind2.Name = "ind2";
            TimeSeries ts = ind1.Values
                .Sum(ind2.Values)
                .Normalize()
                .ToTimeSeries();
            Assert.IsTrue(ts.Values.Average() <= 1e-10);
            Assert.IsTrue(Math.Abs(ts.Values.StandardDeviation() - 1.0) < 1e-10);
        }
    }
}
