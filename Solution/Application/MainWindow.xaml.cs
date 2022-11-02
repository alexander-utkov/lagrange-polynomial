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
        /// Упрощает формулу <see cref="input"/> и обновляет ее отображение в <see cref="formula"/>.
        /// </summary>
        /// <param name="sender">Отправитель события.</param>
        /// <param name="e">Данные события.</param>
        private void simplify_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                formula.Formula = input.Text.Simplify().Latexise();
            }
            catch
            {
                formula.Formula = "\\color{red}{Whoops!}";
            }
        }
    }
}
