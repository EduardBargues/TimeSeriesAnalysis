using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using Common;
using OxyPlot;
using OxyPlot.Series;
using TeslaAnalysis;

namespace DashBoard
{
    public interface IStatisticsView
    {
        void LoadData(DataTable data, BarSeries histogram);
        
    }

    internal class StatisticsPresenter
    {
        private IStatisticsView view;

        public StatisticsPresenter(IStatisticsView view)
        {
            this.view = view;
        }

        private CandleTimeSeries series;
        public void LoadData(CandleTimeSeries candleSeries)
        {
            this.series = candleSeries;
            DataTable table = GetStatistics(candleSeries);
            BarSeries histogram = GetHistogram(candleSeries);
            this.view.LoadData(table, histogram);
        }

        private BarSeries GetHistogram(CandleTimeSeries candleSeries)
        {
            BarSeries bars = Plotter.GetDataPointSeries<BarSeries>((nameof(BarSeries.Title), "PRUEVA"));
            // TODO
            return bars;
        }

        private static DataTable GetStatistics(CandleTimeSeries series)
        {
            const string columnNameDescription = "Description";
            const string columnNameCandlesUp = "Candles UP";
            const string columnNameCandlesDown = "Candles DOWN";
            const string format = "N2";
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn(columnNameDescription, typeof(string)));
            table.Columns.Add(new DataColumn(columnNameCandlesUp, typeof(string)));
            table.Columns.Add(new DataColumn(columnNameCandlesDown, typeof(string)));

            DataRow row = table.NewRow();
            row[columnNameDescription] = "#";
            row[columnNameCandlesUp] = series.Candles.Count(candle => candle.GoesUp);
            row[columnNameCandlesDown] = series.Candles.Count(candle => candle.GoesDown);
            table.Rows.Add(row);

            DataRow row2 = table.NewRow();
            row2[columnNameDescription] = "Body Average";
            row2[columnNameCandlesUp] =
                series.Candles.Where(candle => candle.GoesUp).Average(candle => candle.Body).ToString(format);
            row2[columnNameCandlesDown] =
                series.Candles.Where(candle => candle.GoesDown).Average(candle => candle.Body).ToString(format);
            table.Rows.Add(row2);

            DataRow row3 = table.NewRow();
            row3[columnNameDescription] = "Body St. Dev.";
            row3[columnNameCandlesUp] =
                series.Candles.Where(candle => candle.GoesUp).Deviation(candle => candle.Body).ToString(format);
            row3[columnNameCandlesDown] = series.Candles.Where(candle => candle.GoesDown).Deviation(candle => candle.Body)
                .ToString(format);
            table.Rows.Add(row3);

            DataRow row4 = table.NewRow();
            row4[columnNameDescription] = "Range Average";
            row4[columnNameCandlesUp] =
                series.Candles.Where(candle => candle.GoesUp).Average(candle => candle.Range).ToString(format);
            row4[columnNameCandlesDown] = series.Candles.Where(candle => candle.GoesDown).Average(candle => candle.Range)
                .ToString(format);
            table.Rows.Add(row4);

            DataRow row5 = table.NewRow();
            row5[columnNameDescription] = "Range St. Dev.";
            row5[columnNameCandlesUp] = series.Candles.Where(candle => candle.GoesUp).Deviation(candle => candle.Range)
                .ToString(format);
            row5[columnNameCandlesDown] = series.Candles.Where(candle => candle.GoesDown).Deviation(candle => candle.Range)
                .ToString(format);
            table.Rows.Add(row5);
            return table;
        }
    }
}