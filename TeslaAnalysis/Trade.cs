using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeslaAnalysis
{
    public enum TradeType
    {
        Sell,
        Buy
    }
    public class Trade
    {
        public DateTime Date { get; set; }
        public double Price { get; set; }
        public double Volume { get; set; }
        public TradeType Type { get; set; }

        public override string ToString()
        {
            return $"{Date:yyyy/MM/dd HH:mm:ss.fff}; {Volume}; {Price}; {Type}";
        }
    }
}
