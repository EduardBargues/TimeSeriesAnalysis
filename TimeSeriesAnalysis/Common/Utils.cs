using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Utils
    {
        public static double Max(params double[] numbers)
        {
            return numbers.Max();
        }

        public static double Min(params double[] numbers)
        {
            return numbers.Min();
        }
        public static double Tolerance = 1e-15;
    }
}
