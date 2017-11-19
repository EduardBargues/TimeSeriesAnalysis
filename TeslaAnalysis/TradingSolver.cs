using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeslaAnalysis
{
    public static class TradingSolver
    {
        public static bool IsDojiPattern(Candle candle, double tolerance)
        {
            return Math.Abs(candle.Open - candle.Close) <= tolerance;
        }

        public static bool IsAbandonedBabyPattern(
            Candle leftCandle,
            Candle middleCandle,
            Candle rightCandle)
        {
            bool isAbandonedBabyPattern = false;
            bool orderedCandles = leftCandle.StartDate < middleCandle.StartDate &&
                                  middleCandle.StartDate < rightCandle.StartDate;
            if (orderedCandles)
            {
                bool pattern = leftCandle.Low >= middleCandle.High &&
                               rightCandle.Low >= middleCandle.High;
                bool invertedPattern = leftCandle.High <= middleCandle.Low &&
                                       rightCandle.High <= middleCandle.Low;
                isAbandonedBabyPattern = pattern || 
                                         invertedPattern;
            }
            return isAbandonedBabyPattern;
        }
    }
}
