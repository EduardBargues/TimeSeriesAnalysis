using OxyPlot.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using TimeSeriesAnalysis;

namespace Interface
{
    public partial class Form1 : Form
    {
        private PlotView PlotView => plotPanel.Controls
            .Cast<PlotView>()
            .FirstOrDefault();
        private DateTime FirstDate => firstDatePicker.Value;
        private DateTime LastDate => lastDatePicker.Value;
        private bool Normalize => normalizeCheckBox.Checked;

        private string WorkingPath
        {
            get
            {
                return workingPathText.Text;
            }
            set { workingPathText.Text = value; }
        }

        private List<GridItemPlotInfo> Items => (List<GridItemPlotInfo>)gridTimeSeries.DataSource;

        private Dictionary<string, TimeSeries> timeSeries = new Dictionary<string, TimeSeries>();

        public Form1()
        {
            InitializeComponent();
            PlotView plotView = new PlotView();
            Controls.Add(plotView);
        }

        #region Events

        private void lastDatePicker_ValueChanged(object sender, EventArgs e)
        {
            SetPlot();
        }
        private void normalizeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetPlot();
        }
        private void firstDatePicker_ValueChanged(object sender, EventArgs e)
        {
            SetPlot();
        }
        private void workingPathText_TextChanged(object sender, EventArgs e)
        {
            timeSeries = GetTimeSeriesFromWorkingPath()
                .ToDictionary(s => s.Name, s => s);
            SetSeriesGridDataSource();
        }
        private void gridTimeSeries_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!updatingSeriesGrid)
                SetPlot();
        }
        private void selectWorkingPathButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                Description = "Choose a workping path:"
            };
            DialogResult dr = dialog.ShowDialog();
            if (dr == DialogResult.OK)
                WorkingPath = dialog.SelectedPath;
        }
        #endregion

        #region Methods

        private TimeSeries ReadFile(FileInfo file)
        {
            return TimeSeries.FromCsvFile(
                    fileName: file.FullName,
                    separator: ';',
                    numberCulture: CultureInfo.InvariantCulture,
                    dateFormat: "dd/MM/yyyy",
                    dateCulture: CultureInfo.InvariantCulture)
                .ToTimeSeries(file.Name);
        }
        private bool updatingSeriesGrid;
        private void SetSeriesGridDataSource()
        {
            updatingSeriesGrid = true;
            Random random = new Random();
            gridTimeSeries.DataSource = timeSeries.Keys
                .Select(seriesName => new GridItemPlotInfo()
                {
                    Name = seriesName,
                    Color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)),
                    Draw = false,
                })
                .ToList();
            updatingSeriesGrid = false;
        }
        private void SetPlot()
        {
            IEnumerable<TimeSeriesPlotInfo> tss = Items
                .Where(item => item.Draw)
                .Select(item =>
                {
                    TimeSeries ts = timeSeries[item.Name];
                    TimeSeries finalTs = Normalize
                        ? ts.Values
                            .Normalize()
                            .Where(v => v.Date >= FirstDate &&
                                        v.Date <= LastDate)
                            .ToTimeSeries(ts.Name)
                        : ts.Values
                            .Where(v => v.Date >= FirstDate &&
                                        v.Date <= LastDate)
                            .ToTimeSeries(ts.Name);

                    return TimeSeriesPlotInfo.Create(
                        finalTs,
                        item.Name,
                        OxyColor.FromRgb(item.Color.R, item.Color.G, item.Color.B));
                });
            PlotView plotView = TimeSeries.GetPlotView(tss);
            plotView.Dock = DockStyle.Fill;
            plotPanel.Controls.Clear();
            plotPanel.Controls.Add(plotView);
            plotPanel.Refresh();
        }
        private List<TimeSeries> GetTimeSeriesFromWorkingPath()
        {
            List<TimeSeries> output = new List<TimeSeries>();
            if (!string.IsNullOrEmpty(WorkingPath))
            {
                DirectoryInfo directory = new DirectoryInfo(WorkingPath);
                if (directory.Exists)
                    output = directory.GetFiles("*.csv", SearchOption.AllDirectories)
                        .Select(ReadFile)
                        .Where(ts => ts != null)
                        .ToList();
            }
            return output;
        }
        #endregion
    }

    
}
