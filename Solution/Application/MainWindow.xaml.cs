using AngouriMath;
using AngouriMath.Extensions;
using ScottPlot;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using static AngouriMath.Entity;

namespace NumericalMethods
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обновляет отображение формулы <see cref="input"/> в <see cref="formula"/>.
        /// 
        /// Проблемы
        /// ========
        /// FIXME: Необходимо отображение нелексируемых токенов в formula.
        ///
        /// Лексер Angouri при '1+' ожидает правый операнд, а при конце строки дает исключение. Необходимо отделить
        /// последний нелексируемый оператор и отобразить его в напрямую в `formula`. Данная проблема встречается и
        /// в незавершенных функциях '1+cos(2-', что особенно усложняет их написание.
        /// </summary>
        /// <param name="sender"><see cref="TextBox"/>, отправивший событие.</param>
        private void input_TextChanged(object sender, TextChangedEventArgs e)
        {
            var expression = ((TextBox)sender).Text;
            try
            {
                formula.Formula = expression.Latexise();
            }
            catch
            {
                formula.Formula = "\\color{red}{" + expression + "}";
            }
        }

        /// <summary>
        /// Упрощает формулу в <see cref="input"/>, а также обновляет ее отображение в <see cref="formula"/>.
        /// </summary>
        private void simplify_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var expression = input.Text.Simplify();
                formula.Formula = expression.Latexise();
                input.Text = expression.ToString();
            }
            catch
            {
                formula.Formula = "\\color{red}{Whoops!}";
            }
        }

        /// <summary>
        /// Записывает в <see cref="input"/> ее производную, а также отображает ее в <see cref="formula"/>.
        /// 
        /// Проблемы
        /// ========
        /// FIXME: Необходимы временные ограничения на выполнение операции, а также асинхронное выполнение.
        /// 
        /// Нахождение производной третьего порядка функции `x^2 + ln(cos(x) + 3) + 4x` занимает очень много времени.
        /// </summary>
        private void derivative_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var expression = input.Text.Differentiate("x");
                formula.Formula = expression.Latexise();
                input.Text = expression.ToString();
            }
            catch
            {
                formula.Formula = "\\color{red}{Whoops!}";
            }
        }

        /// <summary>
        /// Обновляет переменные на <see cref="calculate"/> при их изменении в <see cref="vars"/>.
        /// </summary>
        private void vars_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var variables = new List<string>();
                foreach (string variable in vars.Text.Split(','))
                {
                    variables.Add(variable.Split('=')[0].Trim());
                }
                calculate.Content = "f(" + string.Join(",", variables) + ")";
            }
            catch
            {
                // Пользователь в процессе ввода... TODO: выделить неправильный синтаксис.
            }
        }

        /// <summary>
        /// Вычисляет значение функции с переменными, заданными в <see cref="vars"/>.
        /// </summary>
        private void calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Заменяем переменные на заданные числа; запоминаем переменные и числа для вывода их внутри `f(..)`
                var expression = MathS.FromString(input.Text);
                var variables = new List<string>();
                var values = new List<string>();
                foreach (string variable in vars.Text.Split(','))
                {
                    var data = variable.Split('=');
                    var name = data[0].Trim();
                    var value = float.Parse(data[1].Trim()); // FIXME: Учитывать локализацию (точка или запятая)?

                    // FIXME: Переменная может отсутствовать в формуле.
                    expression = expression.Substitute(name, value);
                    
                    variables.Add(name);
                    values.Add(data[1].Trim());
                }

                // Вычисляем значение функции;
                // FIXME: Будет исключение, если определены не все переменные.
                var result = expression.EvalNumerical();

                // Выводим функцию с переменными, с подставленными значениями, а также сам результат
                var common = "f(" + string.Join(",", variables) + ")=" + input.Text.Latexise();
                var numerical = "f(" + string.Join(",", values) + ")=" + expression.Latexise();
                formula.Formula = common + ";\\\\" + numerical + "=" + result.Latexise();

                // NOTE: Новая линия в LaTeX - две обратные косые черты.
            }
            catch
            {
                formula.Formula = "\\color{red}{Whoops!}";
            }
        }

        /// <summary>
        /// Строит график <see cref="input"/> в <see cref="plot"/> в области определения <see cref="range"/>.
        /// </summary>
        private void chart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var expression = MathS.FromString(input.Text);

                var data_x = new List<double>();
                var data_y = new List<double>();

                var data = range.Text.Replace(".", ",").Split(';'); // FIXME
                double from = double.Parse(data[0].Trim());
                double to = double.Parse(data[1].Trim());
                double step = double.Parse(data[2].Trim());
                for (double x = from; x <= to; x += step)
                {
                    data_x.Add(x);
                    data_y.Add((double)expression.Substitute("x", x).EvalNumerical()); // FIXME: Throws on NaN.
                }

                plot.Plot.Clear();
                plot.Plot.AddScatter(data_x.ToArray(), data_y.ToArray());
                plot.Refresh();
            }
            catch
            {
                formula.Formula = "\\color{red}{Whoops!}";
            }
        }
    }
}
