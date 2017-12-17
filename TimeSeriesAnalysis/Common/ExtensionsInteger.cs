using System;
using System.Collections.Generic;

namespace Common
{
    public static class ExtensionsInteger
    {
        public static int GetNextOddNumber(this int number)
        {
            return number % 2 == 0
                ? number + 1
                : number;
        }

        public static IEnumerable<int> GetIntegersTo(this int start, int end)
        {
            int increment = Math.Sign(end - start);
            yield return start;
            int current = start;
            while (current != end)
            {
                current += increment;
                yield return current;
            }
        }
    }
}
