using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using TeslaAnalysis;
using TimeSeriesAnalysis;

namespace DashBoard
{
    public interface IHistoryView
    {
        void LoadData(CandleTimeSeries series, IEnumerable<(TimeSeries, Color)> indicators);
        event Action LoadDataRequest;
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        TimeSpan IndicatorPeriod { get; }
        TimeSpan SmoothingPeriod { get; }
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
            DateTime diStartDay = view.StartDate.Add(view.IndicatorPeriod);
            DirectionalIndicator diPlus = DirectionalIndicator.Create(view.IndicatorPeriod, plus: true);
            TimeSeries diPlusSeries = series.Candles
                .Where(candle => candle.Start > diStartDay)
                .Select(candle => new DateValue(candle.Start, diPlus.GetValueAt(series, candle.Start)))
                .ToTimeSeries("DI+");
            yield return (diPlusSeries, Color.Blue);

            DirectionalIndicator diMinus = DirectionalIndicator.Create(view.IndicatorPeriod, plus: false);
            TimeSeries diMinusSeries = series.Candles
                .Where(candle => candle.Start > diStartDay)
                .Select(candle => new DateValue(candle.Start, diMinus.GetValueAt(series, candle.Start)))
                .ToTimeSeries("DI-");
            yield return (diMinusSeries, Color.Red);

            DirectionalMovementIndex dx = DirectionalMovementIndex.Create(view.IndicatorPeriod);
            TimeSeries dxSeries = series.Candles
                .Where(candle => candle.Start > diStartDay)
                .Select(candle => new DateValue(candle.Start, dx.GetValueAt(series, candle.Start)))
                .ToTimeSeries("DX");
            yield return (dxSeries, Color.DarkGray);

            AverageDirectionalMovementIndex adx = AverageDirectionalMovementIndex.Create(
                view.IndicatorPeriod,
                view.SmoothingPeriod,
                TimeSpan.FromDays(1),
                series,
                exponentialMovingAverage: false);
            TimeSeries adxSeries = series.Candles
                .Where(candle => candle.Start > diStartDay)
                .Select(candle => new DateValue(candle.Start, adx.GetValueAt(series, candle.Start)))
                .ToTimeSeries("ADX");
            yield return (adxSeries, Color.DarkSlateGray);
        }

        private CandleTimeSeries GetCandleTimeSeries()
        {
            return Context.Instance.HistoryCandleTimeSeries.Candles
                .Where(candle => candle.Start >= view.StartDate && candle.Start <= view.EndDate)
                .ToCandleTimeSeries(name: "BitCoin");
        }
    }
}
