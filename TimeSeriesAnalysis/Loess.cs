using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MathNet.Numerics;
using MoreLinq;

namespace TimeSeriesAnalysis
{
    public static class Loess
    {
        public static Dictionary<double, double> Smooth(LoessParameters parameters)
        {
            Dictionary<double, double> result = parameters.Series
                .ToDictionary(kvp => kvp.Key,
                              kvp => kvp.Value);

            OptimizationInfo optimizationInfo = new OptimizationInfo();
            optimizationInfo.SetInfo(
                xs: parameters.XsToEvaluate.ToArray(),
                numberOfNeighbors: parameters.NumberOfNeighbors,
                robustnessWeights: parameters.RobustnessWeights);

            for (int iteration = 0; iteration < parameters.RobustnessIterations; iteration++)
                result = DoSmooth(
                    xy: result,
                    localPolynomialDegree: parameters.LocalPolynomialDegree,
                    info: optimizationInfo);

            return result;
        }

        private static Dictionary<double, double> DoSmooth(
            Dictionary<double, double> xy,
            int localPolynomialDegree,
            OptimizationInfo info)
        {
            Dictionary<double, double> smoothFunction = new Dictionary<double, double>();

            foreach (double xi in xy.Keys)
            {
                Dictionary<double, double> weights = info.GetWeights(xi);
                Dictionary<double, double> rangeXy = info.GetSubRange(xi)
                    .ToDictionary(x => x, x => xy[x]);

                double[] weightedFitPolynomial = FitWeightedPolynomial(rangeXy, weights, localPolynomialDegree);
                double px = Evaluate.Polynomial(xi, weightedFitPolynomial);
                smoothFunction.Add(xi, px);
            }

            return smoothFunction;
        }

        // HELPER METHODS
        private static double[] FitWeightedPolynomial(
            Dictionary<double, double> xy,
            Dictionary<double, double> weights,
            int polynomialDegree)
        {
            double[] xs = xy.Keys
                .OrderBy(x => x)
                .ToArray();
            double[] ys = xs
                .Select(x => xy[x])
                .ToArray();
            double[] ws = xs
                .Select(x => weights[x])
                .ToArray();
            try
            {
                double[] polynomial = Fit.PolynomialWeighted(
                    x: xs,
                    y: ys,
                    w: ws,
                    order: polynomialDegree);
                return polynomial;
            }
            catch (Exception e)
            {
                string filename = @"C:\Users\EBJ\Desktop\FitPolynomialWeighted.csv";
                File.Delete(filename);
                using (TextWriter tw = File.AppendText(filename))
                {
                    tw.WriteLine("xs;ys;ws");
                    xs
                        .ForEach((x, index) =>
                        {
                            tw.WriteLine($"{x};" +
                                         $"{ys[index].ToString(CultureInfo.InvariantCulture)};" +
                                         $"{ws[index].ToString(CultureInfo.InvariantCulture)}");
                        });
                    tw.Flush();
                    tw.Close();
                }
                throw;
            }
        }
    }

    public class LoessParameters
    {
        public Dictionary<double, double> Series { get; set; }
        public IEnumerable<double> XsToEvaluate { get; set; }
        public int NumberOfNeighbors { get; set; }
        public int RobustnessIterations { get; set; }
        public int LocalPolynomialDegree { get; set; }
        public Dictionary<double, double> RobustnessWeights { get; set; }
    }

    public class OptimizationInfo
    {
        private Dictionary<double, double> largestDistances = new Dictionary<double, double>();
        private Dictionary<double, double[]> subRanges = new Dictionary<double, double[]>();
        private Dictionary<double, Dictionary<double, double>> weights = new Dictionary<double, Dictionary<double, double>>();

        public double GetLargestDistance(double x)
        {
            return largestDistances[x];
        }
        public double[] GetSubRange(double x)
        {
            return subRanges[x];
        }
        public double GetWeight(
            double xReference,
            double xToComputeWeight)
        {
            return weights[xReference][xToComputeWeight];
        }

        public double GetWeightSum(double xReference)
        {
            return weights[xReference].Keys
                .Sum(x => GetWeight(xReference, x));
        }

        public void SetInfo(
            double[] xs,
            int numberOfNeighbors,
            Dictionary<double, double> robustnessWeights)
        {
            List<double> orderedXs = xs
                .OrderBy(x => x)
                .ToList();

            // LARGEST DISNTANCES
            largestDistances = orderedXs
                .ToDictionary(x => x,
                              x =>
                    {
                        double distanceToFirstElement = Math.Abs(x - orderedXs.First());
                        double distanceToLastElement = Math.Abs(x - orderedXs.Last());
                        return Math.Max(distanceToFirstElement, distanceToLastElement);
                    });
            // SUBRANGES
            subRanges = new Dictionary<double, double[]>();
            foreach (double x in xs)
            {
                double[] closestNeighbors = xs
                    .OrderBy(neighbor => Math.Abs(neighbor - x))
                    .Take(numberOfNeighbors + 1)
                    .ToArray();
                subRanges.Add(x, closestNeighbors);
            }
            // WEIGHTS
            weights = new Dictionary<double, Dictionary<double, double>>();
            int numberOfElements = xs.Length;
            foreach (double x1 in orderedXs)
            {
                double lambdaQ = numberOfNeighbors > numberOfElements
                    ? largestDistances[x1] * numberOfNeighbors / numberOfElements
                    : largestDistances[x1];
                foreach (double x2 in orderedXs)
                {
                    double normalCoordinate = Math.Abs(x2 - x1) / lambdaQ;
                    double weight = MathFunction.GetTricube(normalCoordinate);
                    if (robustnessWeights != null)
                        weight = weight * robustnessWeights[x2];
                    AddWeight(x1, x2, weight);
                }
            }
        }

        private void AddWeight(double x1, double x2, double weight)
        {
            if (!weights.ContainsKey(x1))
                weights.Add(x1, new Dictionary<double, double>());
            weights[x1].Add(x2, weight);
        }

        public Dictionary<double, double> GetWeights(double xi)
        {
            return weights[xi];
        }
    }
}
