using AngouriMath;
using Extender;
using HandyControl.Controls;
using NumericalMethods.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace NumericalMethods.Pages
{
    public partial class PlotPage : Page
    {
        public PlotPage(IInterpolator interpolator, Entity function = null)
        {
            InitializeComponent();

            m_interpolator = interpolator;
            m_function = function;

            if (m_function != null)
            {
                try
                {
                    m_function_compiled = m_function.Compile<double, double>("x");
                }
                catch
                { }
            }

            plot.Plot.Style(
                figureBackground: Color.Transparent,
                dataBackground: Color.Transparent,
                grid: Color.DarkGray
            );
            PlotHelper.OverrideContextMenu(plot);
        }

        private IInterpolator m_interpolator;
        private Entity m_function;
        private Type m_properties_model_type;
        private object m_properties_model;

        /// <summary>
        /// Скомпилированная исходная функция f(x) для выполнения вычислений. Может быть null.
        /// </summary>
        private Func<double, double> m_function_compiled = null;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializePropertiesModel(new PlotInfo(m_interpolator.DataX.Min() - 1, m_interpolator.DataX.Max() + 1));
            RefreshPlot();
        }

        /// <summary>
        /// Создает модель свойств для <see cref="props"/>.
        /// </summary>
        /// <remarks>
        /// Экземпляр модели, как и ее тип, создается во время выполнения (рефлексия), чтобы исключить свойства исходной
        /// функции при ее отсутствии.
        /// </remarks>
        /// <param name="defaults">Значения по умолчанию.</param>
        private void InitializePropertiesModel(PlotInfo defaults)
        {
            TypeExtender extender = new TypeExtender("PropertiesModel", typeof(System.Object));
            Dictionary<string, object> values = new Dictionary<string, object>();

            Action<string, string, string, string, Type> add_property = (string category, string name,
                string defaults_name, string key, Type editor) =>
            {
                string display_name = Application.Current.TryFindResource(key + ".name") as string;
                string description = Application.Current.TryFindResource(key) as string;
                extender.AddProperty(name, typeof(double), new List<Tuple<Type, object[]>>()
                {
                    new Tuple<Type, object[]>(typeof(CategoryAttribute), new object[] { category }),
                    new Tuple<Type, object[]>(typeof(DisplayNameAttribute), new object[] { display_name }),
                    new Tuple<Type, object[]>(typeof(DescriptionAttribute), new object[] { description }),
                    new Tuple<Type, object[]>(typeof(EditorAttribute), new object[]
                    {
                        editor,
                        typeof(PropertyEditorBase)
                    })
                });

                // Запоминмаем значение по умолчанию данного свойства
                values[name] = typeof(PlotInfo).GetProperty(defaults_name).GetValue(defaults);
            };

            string interpolant_plot = Application.Current.TryFindResource("category.plot.interpolant") as string;
            add_property(interpolant_plot, "InterpolantA", "A", "function.domain.start", typeof(DomainPropertyEditor));
            add_property(interpolant_plot, "InterpolantB", "B", "function.domain.end", typeof(DomainPropertyEditor));
            add_property(interpolant_plot, "InterpolantStep", "Step", "plot.step", typeof(StepPropertyEditor));

            if (m_function_compiled != null)
            {
                string source_plot = Application.Current.TryFindResource("category.plot.source") as string;
                add_property(source_plot, "SourceA", "A", "function.domain.start", typeof(DomainPropertyEditor));
                add_property(source_plot, "SourceB", "B", "function.domain.end", typeof(DomainPropertyEditor));
                add_property(source_plot, "SourceStep", "Step", "plot.step", typeof(StepPropertyEditor));
            }

            m_properties_model_type = extender.FetchType();
            m_properties_model = Activator.CreateInstance(m_properties_model_type);

            // Устанавливаем значения по умолчанию для созданного экземпляра
            foreach (PropertyInfo property in m_properties_model_type.GetProperties())
            {
                property.SetValue(m_properties_model, values[property.Name]);
            }

            props.SelectedObject = m_properties_model;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void replot_Click(object sender, RoutedEventArgs e)
        {
            RefreshPlot();
        }

        private void RefreshPlot()
        {
            plot.Plot.Clear();

            if (m_function_compiled == null)
            {
                plot.Plot.AddScatter(
                    m_interpolator.DataX.ToArray(),
                    m_interpolator.DataY.ToArray(),
                    lineWidth: 0,
                    markerSize: 10,
                    color: Color.Cyan,
                    label: FindResource("legend.node") as string
                );
            }
            else
            {
                BuildPlot("Source", m_function_compiled, Color.Orange, "f(x)");
            }
            BuildPlot("Interpolant", m_interpolator.Interpolate, Color.Magenta, "P(x)");

            plot.Plot.Legend(enable: true);
            plot.Refresh();
        }

        private void BuildPlot(string basename, Func<double, double> func, Color color, string label)
        {
            var a = GetValue<double>(basename + "A");
            var b = GetValue<double>(basename + "B");
            var step = Math.Abs(GetValue<double>(basename + "Step"));

            if (a > b)
            {
                double c = a;
                a = b;
                b = c;
            }

            List<double> data_x = new List<double>();
            List<double> data_y = new List<double>();
            for (double x = a; x <= b; x += step)
            {
                data_x.Add(x);
                data_y.Add(func(x));
            }
            plot.Plot.AddScatter(data_x.ToArray(), data_y.ToArray(), color: color, label: label);
        }

        private T GetValue<T>(string property)
        {
            return (T)m_properties_model_type.GetProperty(property).GetValue(m_properties_model);
        }
    }
}
