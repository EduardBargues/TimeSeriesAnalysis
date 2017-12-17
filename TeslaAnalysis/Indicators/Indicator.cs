using System;

namespace TeslaAnalysis.Indicators
{
    public abstract class Indicator : IIndicator
    {
        private readonly Func<DateTime, double> f;

        protected Indicator()
        {

        }
        protected Indicator(Func<DateTime, double> function)
        {
            this.f = function;
        }

        public string Name { get; set; }
        public string ShortName { get; set; }

        public double GetValueAt(DateTime date)
        {
            return f(date);
        }
        public double this[DateTime date] => GetValueAt(date);
    }
}
