using System.Drawing;

namespace TeslaAnalysis
{
    public class CandleTimeSeriesPlotInfo
    {
        public CandleTimeSeries Series { get; set; }
        public Color IncreasingColor { get; set; }
        public Color DecreasingColor { get; set; }
        public string Title { get; set; }
        public Color BackgroundColor { get; set; }
        //public double CandleWidth { get; set; }
        public bool PositiveCandlesHollow { get; set; }

        public static CandleTimeSeriesPlotInfo Create(
            CandleTimeSeries series = null,
            Color? increasingColor = null,
            Color? decreasingColor = null,
            string title = null,
            Color? backgroundColor = null,
            bool positiveCandlesHollow = true)
        {
            CandleTimeSeriesPlotInfo info = new CandleTimeSeriesPlotInfo
            {
                Series = series,
                IncreasingColor = increasingColor
                                  ?? Color.Black,
                DecreasingColor = decreasingColor
                                  ?? Color.Red,
                BackgroundColor = backgroundColor
                    ?? Color.White,
                PositiveCandlesHollow = positiveCandlesHollow,
            };
            if (!string.IsNullOrEmpty(title))
                info.Title = title;
            else
            {
                string alternativeTitle = series?.Name;
                info.Title = string.IsNullOrEmpty(alternativeTitle)
                    ? "NO NAME"
                    : alternativeTitle;
            }

            return info;
        }
    }
}