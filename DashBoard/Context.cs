using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Common;
using MoreLinq;
using TeslaAnalysis;

namespace DashBoard
{
    public class Context
    {
        private string databaseConnectionString = @"Server=localhost\SQLEXPRESS;Database=FinancialFreedom";
        private string userName = "sa";
        private string password = "112358134711";

        private readonly Dictionary<DateTime, List<Trade>> tradesByDay = new Dictionary<DateTime, List<Trade>>();
        private CandleTimeSeries historyCandleTimeSeries;

        public CandleTimeSeries HistoryCandleTimeSeries => historyCandleTimeSeries ?? (historyCandleTimeSeries = GetHistoryCandleTimeSeriesFromDatabase());

        private static Context instance;
        public static Context Instance => instance
            ?? (instance = new Context());

        private CandleTimeSeries GetHistoryCandleTimeSeriesFromDatabase()
        {
            CandleTimeSeries result = new CandleTimeSeries();

            using (DatabaseConnector connector = new DatabaseConnector())
            {
                connector.Connect(databaseConnectionString, userName, password);
                IEnumerable<(string, object)[]> candlesInfo = connector.ExecuteReaderCommand("select * from BitCoinDailyCandles");
                result = candlesInfo
                    .Select(GetCandleFromDatabaseInfo)
                    .ToCandleTimeSeries();
                connector.Disconnect();
            }

            return result;
        }
        private Candle GetCandleFromDatabaseInfo(IEnumerable<(string, object)> info)
        {
            Dictionary<string, object> propsByName = info
                .ToDictionary(i => i.Item1, i => i.Item2);
            Candle candle = new Candle
            {
                Start = (DateTime)propsByName[nameof(Candle.Start)],
                Duration = TimeSpan.FromTicks((long)propsByName[nameof(Candle.Duration)]),
                Open = Convert.ToDouble(propsByName[nameof(Candle.Open)]),
                Close = Convert.ToDouble(propsByName[nameof(Candle.Close)]),
                Min = Convert.ToDouble(propsByName[nameof(Candle.Min)]),
                Max = Convert.ToDouble(propsByName[nameof(Candle.Max)]),
                BuyVolume = Convert.ToDouble(propsByName[nameof(Candle.BuyVolume)]),
                SellVolume = Convert.ToDouble(propsByName[nameof(Candle.SellVolume)]),
            };
            return candle;
        }

        public IEnumerable<Trade> GetDailyTrades(DateTime date)
        {
            DateTime day = date.Date;
            if (!tradesByDay.ContainsKey(day))
            {
                List<Trade> trades = GetDailyTimeSeriesFromDatabase(day);
                tradesByDay.Add(day, trades);
            }
            return tradesByDay[day];
        }

        public IEnumerable<Trade> GetTradesBetween(DateTime date1, DateTime date2)
        {
            return date1.GetDaysTo(date2)
                .Distinct()
                .SelectMany(GetDailyTrades);
        }
        private List<Trade> GetDailyTimeSeriesFromDatabase(DateTime day)
        {
            List<Trade> trades;
            using (DatabaseConnector connector = new DatabaseConnector())
            {
                connector.Connect(databaseConnectionString, userName, password);
                IEnumerable<(string, object)[]> tradeInfos = connector.ExecuteReaderCommand($"select * from BitCoinTrades where " +
                                                                                            $"year(Instant) = {day.Year} and " +
                                                                                            $"month(Instant) = {day.Month} and " +
                                                                                            $"day(Instant) = {day.Day}");
                trades = tradeInfos
                    .Select(GetTradeFromDatabaseInfo)
                    .ToList();

                connector.Disconnect();
            }

            return trades;
        }

        private Trade GetTradeFromDatabaseInfo((string, object)[] info)
        {
            Dictionary<string, object> propertiesByName = info
                .ToDictionary(i => i.Item1, i => i.Item2);

            Trade trade = new Trade()
            {
                Instant = (DateTime)propertiesByName[nameof(Trade.Instant)],
                Volume = Convert.ToDouble(propertiesByName[nameof(Trade.Volume)]),
                Price = Convert.ToDouble(propertiesByName[nameof(Trade.Price)]),
            };
            string type = (string)propertiesByName[nameof(Trade.Type)];
            trade.Type = string.IsNullOrEmpty(type) ||
                         type == "SELL"
                ? TradeType.Sell
                : TradeType.Buy;

            return trade;
        }
    }
}
