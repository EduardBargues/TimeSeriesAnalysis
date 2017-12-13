using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeslaAnalysis
{
    public abstract class Indicator : IIndicator
    {
        private readonly Func<CandleTimeSeries, DateTime, double> f;

        protected Indicator()
        {
            
        }
        protected Indicator(Func<CandleTimeSeries, DateTime, double> function)
        {
            this.f = function;
        }

        public double GetValueAt(CandleTimeSeries series, DateTime instant)
        {
            return f.Invoke(series, instant);
        }
    }
}
