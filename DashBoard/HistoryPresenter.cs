using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using TeslaAnalysis;
using TeslaAnalysis.Indicators;
using TimeSeriesAnalysis;

namespace DashBoard
{
    public interface IHistoryView
    {
        void LoadData(CandleTimeSeries series, IEnumerable<(TimeSeries, Color)> indicators);
        event Action LoadDataRequest;
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        int IndicatorPeriod { get; }
        int SmoothingPeriod { get; }
    }

    public class HistoryPresenter
    {
        private readonly IHistoryView view;

        public HistoryPresenter(IHistoryView view)
        {
            this.view = view;
            this.view.LoadDataRequest += View_LoadDataRequest;
        }

        private void View_LoadDataRequest()
        {
            CandleTimeSeries data = GetCandleTimeSeries();
            IEnumerable<(TimeSeries, Color)> indicators = GetIndicators();
            view.LoadData(data, indicators);
        }

        private IEnumerable<(TimeSeries, Color)> GetIndicators()
        {
            CandleTimeSeries series = Context.Instance.HistoryCandleTimeSeries.Candles
                .ToCandleTimeSeries();

            //TrueRange tr = TrueRange.Create(series);
            //TimeSeries trSeries = series.Candles
            //    .Where((candle, index) => index > 0)
            //    .Select(candle => new DateValue(candle.Start, tr.GetValueAt(candle.Start)))
            //    .ToTimeSeries("TR");
            //yield return (trSeries, Color.Blue);

            //AverageTrueRange atr = AverageTrueRange.Create(series, view.SmoothingPeriod);
            //TimeSeries atrSeries = series.Candles
            //    .Where((candle, index) => index > 0)
            //    .Select(candle => new DateValue(candle.Start, atr.GetValueAt(candle.Start)))
            //    .ToTimeSeries("ATR");
            //yield return (atrSeries, Color.Red);

            DirectionalIndicatorPlus diPlus = DirectionalIndicatorPlus.Create(series, view.IndicatorPeriod, view.SmoothingPeriod);
            TimeSeries diPlusSeries = series.Candles
                .Where((candle, index) => index > 0)
                .Select(candle => new DateValue(candle.Start, diPlus[candle.Start]))
                .ToTimeSeries("DI+");

            yield return (diPlusSeries, Color.Blue);

            DirectionalIndicatorMinus diMinus = DirectionalIndicatorMinus.Create(series, view.IndicatorPeriod, view.SmoothingPeriod);
            TimeSeries diMinusSeries = series.Candles
                .Where((candle, index) => index > 0)
                .Select(candle => new DateValue(candle.Start, diMinus[candle.Start]))
                .ToTimeSeries("DI-");
            yield return (diMinusSeries, Color.Red);

            DirectionalMovementIndex dx = DirectionalMovementIndex.Create(series, view.IndicatorPeriod, view.IndicatorPeriod);
            TimeSeries dxSeries = series.Candles
                .Where((candle, index) => index > 0)
                .Select(candle => new DateValue(candle.Start, dx[candle.Start]))
                .ToTimeSeries("DX");
            yield return (dxSeries, Color.DarkSlateGray);

            AverageDirectionalMovementIndex adx = AverageDirectionalMovementIndex.Create(series, view.IndicatorPeriod, view.SmoothingPeriod);
            TimeSeries adxSeries = series.Candles
                .Where((candle, index) => index > 0)
                .Select(candle => new DateValue(candle.Start, adx[candle.Start]))
                .ToTimeSeries("ADX");
            yield return (adxSeries, Color.DarkGray);
        }

        private CandleTimeSeries GetCandleTimeSeries()
        {
            return Context.Instance.HistoryCandleTimeSeries.Candles
                .Where(candle => candle.Start >= view.StartDate && candle.Start <= view.EndDate)
                .ToCandleTimeSeries(name: "BitCoin");
        }
    }
}
