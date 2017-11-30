using System;
using System.Globalization;
using MoreLinq;

namespace TeslaAnalysis
{
    public enum TradeType
    {
        Sell,
        Buy
    }
    public class Trade
    {
        public DateTime Instant { get; set; }
        public double Price { get; set; }
        public double Volume { get; set; }
        public TradeType Type { get; set; }

        public string ToText(CultureInfo culture)
        {
            string textVolume = Volume.ToString(culture);
            string textPrice = Price.ToString(culture);

            return $"{Instant:yyyy/MM/dd HH:mm:ss.fff}; {textVolume}; {textPrice}; {Type}";
        }

        public static Trade Parse(
            string text,
            CultureInfo culture)
        {
            Trade trade = new Trade();
            text
                .Split(';')
                .ForEach((word, index) =>
                {
                    if (index == 0)
                        trade.Instant = DateTime.ParseExact(word, "yyyy/MM/dd HH:mm:ss.fff", culture);
                    if (index == 1)
                        trade.Volume = double.Parse(word, culture);
                    if (index == 2)
                        trade.Price = double.Parse(word, culture);
                    if (index == 3 &&
                        Enum.TryParse(word, true, out TradeType type))
                        trade.Type = type;
                });
            return trade;
        }
    }
}
