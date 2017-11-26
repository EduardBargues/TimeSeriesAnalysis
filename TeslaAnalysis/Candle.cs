using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeslaAnalysis
{
    public class Candle
    {
        public DateTime StartDate { get; set; }
        public TimeSpan Duration { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double BuyVolume { get; set; }
        public double SellVolume { get; set; }
        public double Volume => BuyVolume + SellVolume;
    }
}
