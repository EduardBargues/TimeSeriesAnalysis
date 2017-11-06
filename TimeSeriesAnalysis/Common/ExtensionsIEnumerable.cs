using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public static class ExtensionsIEnumerable
    {
        public static double Deviation(this IEnumerable<double> list)
        {
            return Math.Sqrt(list.Variance());
        }

        public static double Variance(this IEnumerable<double> list)
        {
            List<double> doubles = list.ToList();
            double average = doubles.Average();
            double variance = doubles
                .Sum(d => Math.Pow(d - average, 2));
            return variance;
        }
    }
}
