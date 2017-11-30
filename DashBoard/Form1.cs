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
using TeslaAnalysis;
using CandleTimeSeries = TeslaAnalysis.CandleTimeSeries;

namespace DashBoard
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }

        private Dictionary<DateTime, List<Trade>> tradesByDay;
        private void Form_Load(object sender, EventArgs e)
        {
            string path = @"D:\Dropbox\Llibertat Financera\Analisis\0002-BitCoin\BitCoinTradeData\BitCoinHistoricalTradeDate.csv";
            tradesByDay = LoadFile(path)
                .GroupBy(trade => trade.Instant.Date)
                .ToDictionary(grouping => grouping.Key,
                              grouping => grouping.ToList());
            CandleTimeSeries dailySeries = DefineDailyCandleTimeSeries();
            historyCtl1.LoadSeries(dailySeries);
        }

        private CandleTimeSeries DefineDailyCandleTimeSeries()
        {
            var dailyCandleTimeSeries = new CandleTimeSeries();
            foreach (DateTime date in tradesByDay.Keys)
            {
                Candle candle = tradesByDay[date].ToCandle();
                candle.StartDate = date;
                candle.Duration = TimeSpan.FromDays(1);
                dailyCandleTimeSeries[date] = candle;
            }
            return dailyCandleTimeSeries;
        }

        private IEnumerable<Trade> LoadFile(string path)
        {
            string[] lines = File.ReadAllLines(path);
            for (int index = 1; index < lines.Length; index++)
                yield return Trade.Parse(lines[index], CultureInfo.CurrentCulture);
        }
    }
}
