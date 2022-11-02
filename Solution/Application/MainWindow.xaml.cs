using AngouriMath.Extensions;
using System.Windows;
using System.Windows.Controls;

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
        /// <param name="e">Данные события.</param>
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
        /// <param name="sender">Отправитель события.</param>
        /// <param name="e">Данные события.</param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
    }
}
