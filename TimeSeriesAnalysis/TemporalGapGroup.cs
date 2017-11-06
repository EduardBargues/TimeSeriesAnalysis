using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MoreLinq;

namespace TimeSeriesAnalysis
{
    public class TemporalGapGroup
    {
        public List<TemporalGapTuple> Pairs { get; set; } = new List<TemporalGapTuple>();

        public DataTable GetPredictionTable(string targetName, TimeSpan predictionSpan)
        {
            DataTable table = new DataTable();
            List<TemporalGapTuple> pairs = Pairs
                .Where(pair => pair.Target.Name == targetName &&
                               pair.Indicator.Name != targetName &&
                               pair.TemporalGap >= predictionSpan)
                .ToList();

            List<DataColumn> columnNames = GetColumns(pairs, targetName);
            columnNames
                .ForEach(column => table.Columns.Add(column));

            TimeSeries target = pairs
                .First(pair => pair.Target.Name == targetName)
                .Target;
            target.TimeCoordinates
                .ForEach(day =>
                {
                    DataRow row = table.NewRow();
                    FillRow(row, predictionSpan, day, target, pairs);
                    table.Rows.Add(row);
                });

            return table;
        }

        private void FillRow(DataRow row, TimeSpan predictionSpan, DateTime day, TimeSeries target, List<TemporalGapTuple> pairs)
        {
            DateTime targetNextDate = day.Add(predictionSpan);
            if (target.ContainsValueAt(targetNextDate))
            {
                double incrementTarget = target[targetNextDate] - target[day];
                row[target.Name] = incrementTarget;
            }
            pairs
                .ForEach(pair => FillRowWithIndicatorData(row, predictionSpan, day, pair));
        }

        private void FillRowWithIndicatorData(DataRow row, TimeSpan predictionSpan, DateTime day, TemporalGapTuple pair)
        {
            TimeSeries indicator = pair.Indicator;
            DateTime indicatorDay = day.Add(-pair.TemporalGap);
            DateTime indicatorNextDay = indicatorDay.Add(predictionSpan);
            if (indicator.ContainsValueAt(indicatorDay) &&
                indicator.ContainsValueAt(indicatorNextDay))
            {
                double incrementIndicator = indicator[indicatorNextDay] - indicator[indicatorDay];
                row[indicator.Name] = incrementIndicator;
            }
        }

        private List<DataColumn> GetColumns(List<TemporalGapTuple> pairs, string targetName)
        {
            List<DataColumn> columnNames = new List<DataColumn>
            {
                new DataColumn(targetName,typeof(double))
            };
            columnNames.AddRange(pairs
                .Where(pair => pair.Target.Name == targetName &&
                               pair.Indicator.Name != targetName)
                .Select(pair => new DataColumn(pair.Indicator.Name, typeof(double))));
            return columnNames;
        }
    }
}
