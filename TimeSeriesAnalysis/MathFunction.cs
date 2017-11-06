using System;

namespace TimeSeriesAnalysis
{
    public static class MathFunction
    {
        public static double GetTricube(double u)
        {
            return GetWeightFunction(u, 3);
        }
        public static double GetBisquare(double u)
        {
            return GetWeightFunction(u, 2);
        }
        public static double GetWeightFunction(double u, double exponent)
        {
            return 0 <= u &&
                   u < 1
                ? Math.Pow(1 - Math.Pow(u, exponent), exponent)
                : 0;
        }
    }
}
