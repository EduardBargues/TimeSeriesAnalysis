using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot.Series;

namespace DashBoard
{
    public partial class StatisticsCtl : UserControl, IStatisticsView
    {
        public StatisticsCtl()
        {
            InitializeComponent();
        }

        public void LoadData(DataTable data, BarSeries series)
        {
            grid.DataSource = data;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            grid.Refresh();
        }
    }
}
