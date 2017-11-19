using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSeriesAnalysis;

namespace UnitTestProjectNuget
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            TimeSeries series = TimeSeries.CreateDailySinusoidalTimeSeries(1, 1, 1, DateTime.MaxValue, DateTime.MaxValue);
            TimeSeriesPredictorAverage average = new TimeSeriesPredictorAverage();
        }
    }
}
