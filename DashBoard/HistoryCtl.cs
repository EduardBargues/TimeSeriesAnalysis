using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot.WindowsForms;
using TeslaAnalysis;

namespace DashBoard
{
    public partial class HistoryCtl : UserControl
    {
        public HistoryCtl()
        {
            InitializeComponent();
        }

        private CandleTimeSeries series;
        private DateTime minDate => dateTimePickerFrom.Value;
        private DateTime maxDate => dateTimePickerTo.Value;

        public void LoadSeries(CandleTimeSeries candleTimeSeries)
        {
            this.series = candleTimeSeries;

            DateTime? min = candleTimeSeries.GetMinDate();
            if (min != null)
                dateTimePickerFrom.Value = min.Value;
            DateTime? max = candleTimeSeries.GetMaxDate();
            if (max != null)
                dateTimePickerTo.Value = max.Value;
            RefreshPlot();
        }

        private void RefreshPlot()
        {
            CandleTimeSeries candleTimeSeries = series.Candles
                .Where(candle => candle.StartDate >= minDate && candle.StartDate <= maxDate)
                .ToCandleTimeSeries("SERIES");
            PlotView plotView = CandleTimeSeries.GetPlotView(CandleTimeSeriesPlotInfo.Create(series:candleTimeSeries));
            panel.Controls.Clear();
            panel.Controls.Add(plotView);
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshPlot();
        }
    }
}
