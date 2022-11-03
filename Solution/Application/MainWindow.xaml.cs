using System.Windows;

namespace NumericalMethods
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void interpolate_Click(object sender, RoutedEventArgs e)
        {
            // NOTE: Правильный ответ: P(x) = 7x - 2
            var data_x = new double[] { 0, 2, -2 };
            var data_y = new double[] { -2, 12, -16 };

            // NOTE: Правильный ответ: P(x) = 4.834848x^3 - 1.477474x
            // Источник упростил числа: дробь 6119104/1265625 точнее.
            // var data_x = new double[] { -1.5, -0.75, 0, 0.75, 1.5 };
            // var data_y = new double[] { -14.1014, -0.931596, 0, 0.931596, 14.1014 };

            // TODO: Выполнять асинхронно (заблокировать пользовательский интерфейс, но исключить его зависание).
            LagrangeInterpolator interpolator = new LagrangeInterpolator(data_x, data_y);
            solution.Formula = interpolator.Solution;

            // TOOD: Построить график функции и многое другое.
        }
    }
}
