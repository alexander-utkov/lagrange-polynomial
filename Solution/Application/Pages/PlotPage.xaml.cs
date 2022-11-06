using Extender;
using HandyControl.Controls;
using NumericalMethods.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace NumericalMethods.Pages
{
    public partial class PlotPage : Page
    {
        public PlotPage(IInterpolator interpolator)
        {
            InitializeComponent();

            m_interpolator = interpolator;
        }

        private IInterpolator m_interpolator;
        private Type m_properties_model_type;
        private object m_properties_model;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializePropertiesModel(new PlotInfo(m_interpolator.DataX.Min() - 1, m_interpolator.DataX.Max() + 1));

            props.SelectedObject = m_properties_model;

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

            Action<string, string, string, string, string, Type> add_property = (string category, string name,
                string display_name, string defaults_name, string description_key, Type editor) =>
            {
                string description = Application.Current.FindResource(description_key) as string;
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

            string interpolant_plot = "Интерполирующая функция";
            add_property(interpolant_plot, "InterpolantA", "a", "A", "fn.domain.a", typeof(DomainPropertyEditor));
            add_property(interpolant_plot, "InterpolantB", "b", "B", "fn.domain.b", typeof(DomainPropertyEditor));
            add_property(interpolant_plot, "InterpolantStep", "Шаг", "Step", "plot.step", typeof(StepPropertyEditor));

            /*
            string source_plot = "Исходная функция";
            add_property(source_plot, "SourceA", "a", "fn.domain.a", typeof(DomainPropertyEditor));
            add_property(source_plot, "SourceB", "b", "fn.domain.b", typeof(DomainPropertyEditor));
            add_property(source_plot, "SourceStep", "Шаг", "plot.step", typeof(StepPropertyEditor));
            */

            m_properties_model_type = extender.FetchType();
            m_properties_model = Activator.CreateInstance(m_properties_model_type);

            // Устанавливаем значения по умолчанию для созданного экземпляра
            foreach (PropertyInfo property in m_properties_model_type.GetProperties())
            {
                property.SetValue(m_properties_model, values[property.Name]);
            }
        }

        private void back_Click(object sender, System.Windows.RoutedEventArgs e)
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

            BuildPlot("Interpolant", m_interpolator.Interpolate);

            plot.Refresh();
        }

        private void BuildPlot(string basename, Func<double, double> func)
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
            plot.Plot.AddScatter(data_x.ToArray(), data_y.ToArray());
        }

        private T GetValue<T>(string property)
        {
            return (T)m_properties_model_type.GetProperty(property).GetValue(m_properties_model);
        }
    }
}
