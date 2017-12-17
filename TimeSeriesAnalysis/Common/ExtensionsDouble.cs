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
            return number2 <= Utils.Tolerance 
                ? double.NaN 
                : number / number2;
        }
    }
}
