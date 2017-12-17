using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using OxyPlot;

namespace Common
{
    public static class ExtensionsIEnumerable
    {
        public static double Deviation(this IEnumerable<double> list)
        {
            return Math.Sqrt(list.Variance());
        }
        public static double Deviation<T>(this IEnumerable<T> list, Func<T, double> f)
        {
            return Math.Sqrt(list.Variance(f));
        }

        public static double Variance<T>(this IEnumerable<T> list, Func<T, double> f)
        {
            List<double> doubles = list
                .Select(f.Invoke)
                .ToList();
            double average = doubles.Average();
            double variance = doubles
                .Sum(d => Math.Pow(d - average, 2));
            return variance;
        }
        public static double Variance(this IEnumerable<double> list)
        {
            List<double> doubles = list.ToList();
            double average = doubles.Average();
            double variance = doubles
                .Sum(d => Math.Pow(d - average, 2));
            return variance;
        }

        public static double Average<T>(this IEnumerable<T> list, Func<T, int, double> f)
        {
            return list.WeightedAverage(f, (element, index) => 1);
        }
        public static double WeightedAverage<T>(this IEnumerable<T> list
            , Func<T, int, double> function
            , Func<T, int, double> weightFunction)
        {
            double sum = 0;
            double sumWeights = 0;
            list
                .ForEach((element, index) =>
                {
                    sum += function(element, index) * weightFunction(element, index);
                    sumWeights += weightFunction(element, index);
                });

            return sum.DivideBy(sumWeights);
        }
    }
}
