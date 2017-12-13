using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeslaAnalysis
{
    public interface IIndicator
    {
        double GetValueAt(CandleTimeSeries series, DateTime instant);
    }
}
