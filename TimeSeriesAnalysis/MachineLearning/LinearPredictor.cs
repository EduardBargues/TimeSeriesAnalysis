using System.Collections.Generic;
using System.Data;
using System.Linq;
using Accord.Statistics.Models.Regression.Linear;

namespace MachineLearning
{
    public class LinearPredictor : IPredictor
    {
        private MultipleLinearRegression mlr;
        private List<string> inputLabels;
        public bool UseIntercept { get; set; }
        
        public void Learn(DataTable table, IEnumerable<string> trainingLabels, string targetLabel)
        {
            inputLabels = trainingLabels
                .ToList();

            double[] outputs = GetOutputs(table, targetLabel)
                .ToArray();
            double[][] inputs = GetInputs(table, inputLabels)
                .ToArray();

            OrdinaryLeastSquares ols = new OrdinaryLeastSquares
            {
                UseIntercept = UseIntercept
            };

            mlr = ols.Learn(inputs, outputs);
        }
        public void Learn(DataTable table, string trainingLabel, string targetLabel)
        {
            Learn(table, new List<string>() { trainingLabel }, targetLabel);
        }
        public object Predict(DataRow row)
        {
            double[] input = GetInput(inputLabels, row)
                .ToArray();
            double prediction = mlr.Transform(input);

            return prediction;
        }


        private IEnumerable<double[]> GetInputs(DataTable table, List<string> trainingLabels)
        {
            return table.Rows
                .Cast<DataRow>()
                .Where(row => row.ItemArray.All(item => item.GetType() != typeof(System.DBNull)))
                .Select(row => GetInput(trainingLabels, row)
                    .ToArray());
        }
        private IEnumerable<double> GetInput(List<string> trainingLabels, DataRow row)
        {
            return trainingLabels
                .Select(label => (double)row[label]);
        }
        private IEnumerable<double> GetOutputs(DataTable table, string outputLabel)
        {
            return table.Rows
                .Cast<DataRow>()
                .Where(row => row.ItemArray.All(item => item.GetType() != typeof(System.DBNull)))
                .Select(row => (double)row[outputLabel]);
        }
    }
}
