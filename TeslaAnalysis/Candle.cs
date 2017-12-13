using System;

namespace TeslaAnalysis
{
    public class Candle
    {
        public DateTime Start { get; set; }
        public TimeSpan Duration { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double BuyVolume { get; set; }
        public double SellVolume { get; set; }
        public double Volume => BuyVolume + SellVolume;
        public bool GoesUp => Open <= Close;
        public bool GoesDown => !GoesUp;
        public double Body => Math.Abs(Open - Close);
        public double Range => Max - Min;
    }
}
