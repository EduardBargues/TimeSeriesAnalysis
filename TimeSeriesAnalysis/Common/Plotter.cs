using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoreLinq;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace Common
{
    public static class Plotter
    {
        public static T GetDataPointSeries<T>(params (string, object)[] parameters)
        {
            return GetDataPointSeries<T>(parameters.ToList());
        }

        private static T GetDataPointSeries<T>(IEnumerable<(string, object)> parameters)
        {
            Type type = typeof(T);
            T series = default;
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor != null)
            {
                series = (T)constructor.Invoke(new object[] { });
                Dictionary<string, PropertyInfo> propertiesByName = series.GetType().GetProperties()
                    .Where(pi =>
                    {
                        MethodInfo methodInfo1 = pi.GetSetMethod();
                        return methodInfo1 != null &&
                               methodInfo1.IsPublic;
                    })
                    .ToDictionary(pi => pi.Name);
                parameters
                    .ForEach(input =>
                    {
                        PropertyInfo pi = propertiesByName.ContainsKey(input.Item1)
                            ? propertiesByName[input.Item1]
                            : null;
                        SetPropertyValue(series, pi, input);
                    });
            }

            return series;
        }

        private static void SetPropertyValue<T>(T series, PropertyInfo pi, (string, object) input)
        {
            if (pi != null &&
                series != null)
            {
                object value = input.Item2;
                pi.SetValue(series, value);
            }
        }

        public static void Plot(DataPointSeries series)
        {
            Form form = new Form();

            PlotView plotView = new PlotView { Model = new PlotModel() };
            plotView.Model.Series.Add(series);
            plotView.Dock = DockStyle.Fill;

            form.Controls.Add(plotView);
            form.ShowDialog();
        }

    }
}
