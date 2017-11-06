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
    }
}
