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
using TimeSeriesAnalysis;

namespace DashBoard
{
    public partial class HistoryCtl : UserControl, IHistoryView
    {
        public event Action LoadDataRequest;

        public HistoryCtl()
        {
            InitializeComponent();
            dateTimePickerFrom.Value = new DateTime(2017, 1, 1);
            buttonRefresh.Click += (a, b) => LoadDataRequest?.Invoke();
            numericUpDownDxPeriod.Value = 14;
            numericUpDownAdxSmoothingPeriod.Value = 14;
        }

        public DateTime StartDate => dateTimePickerFrom.Value;
        public DateTime EndDate => dateTimePickerTo.Value;
        public TimeSpan IndicatorPeriod => TimeSpan.FromDays((int)numericUpDownDxPeriod.Value);
        public TimeSpan SmoothingPeriod => TimeSpan.FromDays((int)numericUpDownAdxSmoothingPeriod.Value);

        public void LoadData(CandleTimeSeries series, IEnumerable<(TimeSeries, Color)> indicators)
        {
            seriesIndicatorCtl.LoadData(series, indicators);
        }
    }
}
