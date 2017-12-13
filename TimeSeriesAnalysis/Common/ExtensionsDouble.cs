using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class ExtensionsDouble
    {
        public static double DivideBy(this double number, double number2)
        {
            if (number <= Utils.Tolerance && 
                number2 <= Utils.Tolerance)
                return 1;

            return number2 <= Utils.Tolerance ? double.MaxValue : number / number2;
        }
    }
}
