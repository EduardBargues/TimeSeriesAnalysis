using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using MoreLinq;
using OxyPlot;
using OxyPlot.Series;

namespace TimeSeriesAnalysis
{
    public static class Plotter
    {
        //public static PlotView GetPlotView(params Series[] series)
        //{
        //    PlotModel model = new PlotModel();
        //    series.ForEach(s => model.Series.Add(s));

        //    return new PlotView()
        //    {
        //        Model = model,
        //        Dock = DockStyle.Fill,
        //    };
        //}

        public static void Plot(params Series[] series)
        {
            //PlotView plotView = GetPlotView(series);
            Form form = new Form();
            //form.Controls.Add(plotView);
            form.ShowDialog();
        }
        public static DataPointSeries GetDataPointSeries(params object[] parameters)
        {
            return GetDataPointSeries(parameters.ToList());
        }
        private static DataPointSeries GetDataPointSeries(IEnumerable<object> parameters)
        {
            DataPointSeries series = null;

            List<object> inputs = parameters
                .ToList();

            List<Type> types = inputs
                .Where(parameter => parameter is Type &&
                                    IsSameOrSubclass(typeof(DataPointSeries), (Type)parameter))
                .Cast<Type>()
                .ToList();

            Type seriesType = types
                .FirstOrDefault();
            if (seriesType != null)
            {
                ConstructorInfo constructor = seriesType.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    series = (DataPointSeries)constructor.Invoke(new object[] { });
                    List<PropertyInfo> propertiesWithSetter = series.GetType().GetProperties()
                        .Where(pi => pi.GetSetMethod() != null)
                        .ToList();
                    Dictionary<string, PropertyInfo> propertiesByName = propertiesWithSetter
                        .ToDictionary(pi => pi.Name);
                    inputs
                        .Where(input => input is Spi)
                        .Cast<Spi>()
                        .ForEach(input =>
                        {
                            PropertyInfo pi = propertiesByName.ContainsKey(input.PropertyName)
                                ? propertiesByName[input.PropertyName]
                                : null;
                            SetPropertyValue(series, pi, input);
                        });
                }
            }

            return series;
        }

        private static void SetPropertyValue(DataPointSeries series, PropertyInfo pi, object input)
        {
            if (pi != null &&
                series != null)
            {
                object value = input is Spi
                    ? ((Spi)input).Value
                    : input;
                pi.SetValue(series, value);
            }
        }

        private static bool IsSameOrSubclass(
            Type potentialBase,
            Type potentialDescendant)
        {
            return potentialDescendant.IsSubclassOf(potentialBase)
                   || potentialDescendant == potentialBase;
        }
    }

    public class Spi
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }
        public Spi(string propName, object propValue)
        {
            PropertyName = propName;
            Value = propValue;
        }
    }
}
