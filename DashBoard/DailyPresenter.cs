using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeslaAnalysis;
using TimeSeriesAnalysis;

namespace DashBoard
{
    public interface IDailyView
    {
        IStatisticsView GetStatisticsView();
        DateTime? SelectedDay { get; }
        event Action RefreshRequest;
        bool ComputeCandlesByDuration { get; }
        bool ComputeCandlesByTicks { get; }
        bool ComputeCandlesByVolume { get; }
        TimeSpan CandlesDuration { get; }
        int CandlesTicks { get; }
        double CandlesVolume { get; }
        void LoadData(CandleTimeSeries series, IEnumerable<(TimeSeries, Color)> indicators);
        void LoadDays(IEnumerable<DateTime> days);
        bool DaysLoaded { get; }
    }

    public class DailyPresenter
    {
        private readonly IDailyView view;
        private readonly StatisticsPresenter statisticsPresenter;

        public DailyPresenter(IDailyView view)
        {
            this.view = view;
            statisticsPresenter = new StatisticsPresenter(view.GetStatisticsView());
            this.view.RefreshRequest += View_RefreshRequest;
        }

        public void LoadDays()
        {
            if (!view.DaysLoaded)
            {
                IEnumerable<DateTime> days = Context.Instance.HistoryCandleTimeSeries.Candles
                    .Select(candle => candle.Start.Date)
                    .Distinct();
                view.LoadDays(days);
            }
        }

        private void View_RefreshRequest()
        {
            LoadData();
        }

        public void LoadData()
        {
            if (view.SelectedDay != null)
            {
                DateTime day = view.SelectedDay.Value.Date;
                CandleTimeSeries series = new CandleTimeSeries();
                List<Trade> trades = Context.Instance.GetDailyTrades(day)
                    .ToList();
                if (view.ComputeCandlesByDuration)
                {
                    TimeSpan candleDuration = view.CandlesDuration;
                    series = GetCandleTimeSeriesByDuration(trades, candleDuration, day);
                }
                if (view.ComputeCandlesByTicks)
                {
                    // TODO
                }
                if (view.ComputeCandlesByVolume)
                {
                    // TODO
                }
                statisticsPresenter.LoadData(series);
                view.LoadData(series, new List<(TimeSeries, Color)>());
                //LoadSeriesIndicators(day);
            }
        }

        //private void LoadSeriesIndicators(DateTime day)
        //{
        //    IEnumerable<Trade> trades = Context.Instance.GetTradesBetween(day.AddDays(-1), day);

        //    view.LoadData(null, null);
        //}

        private CandleTimeSeries GetCandleTimeSeriesByDuration(IEnumerable<Trade> trades, TimeSpan candleDuration, DateTime firstCandleStart)
        {
            return trades
                .ToCandlesByDuration(candleDuration, firstCandleStart)
                .ToCandleTimeSeries($"{firstCandleStart:yyyy/MM/dd} - By duration");
        }
    }
}
