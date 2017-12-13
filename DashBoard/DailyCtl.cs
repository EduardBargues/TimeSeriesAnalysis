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
    public partial class DailyCtl : UserControl, IDailyView
    {
        public DateTime? SelectedDay => gridDays.CurrentRow?.DataBoundItem as DateTime?;
        public event Action RefreshRequest;
        public event Action LoadDaysRequest;
        public bool DaysLoaded { get; set; }

        public bool ComputeCandlesByDuration => candlesByDurationRadioButton.Checked;
        public bool ComputeCandlesByTicks => candlesByTicksRadioButton.Checked;
        public bool ComputeCandlesByVolume => candlesByVolumeRadioButton.Checked;

        public TimeSpan CandlesDuration => TimeSpan.FromMinutes((int)candlesDurationUpDown.Value);
        public int CandlesTicks => (int)candlesTicksUpDown.Value;
        public double CandlesVolume => Convert.ToDouble(candlesVolumeUpDown.Value);

        public DailyCtl()
        {
            InitializeComponent();
            candlesByDurationRadioButton.Checked = true;
            candlesDurationUpDown.Value = 30;
            refreshButton.Click += (a, b) => RefreshRequest?.Invoke();
        }

        public IStatisticsView GetStatisticsView()
        {
            return statisticsCtl;
        }

        public void LoadDays(IEnumerable<DateTime> days)
        {
            DaysLoaded = true;

            gridDays.DataSource = days
                .OrderBy(day => day)
                .ToList();
            gridDays.Columns.Clear();
            gridDays.Columns.Add(nameof(DateTime.Date), "DAY");
            gridDays.Columns[0].DataPropertyName = nameof(DateTime.Date);
            gridDays.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridDays.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            gridDays.Refresh();
        }

        public void LoadData(CandleTimeSeries series, IEnumerable<(TimeSeries, Color)> indicators)
        {
            seriesIndicatorCtl.LoadData(series, indicators);
        }
    }
}
