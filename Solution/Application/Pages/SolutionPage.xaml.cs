using AngouriMath;
using NumericalMethods.Core;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NumericalMethods.Pages
{
    public partial class SolutionPage : Page
    {
        public SolutionPage(Core.IInterpolator interpolator, Entity function = null)
        {
            InitializeComponent();

            m_interpolator = interpolator;
            m_function = function;

            m_plot_info = new PlotInfo(m_interpolator.DataX.Min() - 1, m_interpolator.DataX.Max() + 1);

            polynomial.Formula = "P(x)=" + interpolator.Polynomial.Latexise();

            plot.Plot.Style(
                figureBackground: Color.Transparent,
                dataBackground: Color.Transparent,
                grid: Color.DarkGray
            );
            PlotHelper.OverrideContextMenu(plot);
        }

        private IInterpolator m_interpolator;
        private Entity m_function;
        private PlotInfo m_plot_info;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown += Page_KeyDown;

            FigurePlot();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown -= Page_KeyDown;
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                details_Click(sender, null);
            }
        }

        private void FigurePlot()
        {
            List<double> data_x = new List<double>();
            List<double> data_y = new List<double>();
            for (double x = m_plot_info.A; x < m_plot_info.B; x += m_plot_info.Step)
            {
                data_x.Add(x);
                data_y.Add(m_interpolator.Interpolate(x));
            }
            plot.Plot.AddScatter(data_x.ToArray(), data_y.ToArray(), color: Color.Magenta);
            plot.Refresh();
        }

        private void details_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new WalkthroughPage(m_interpolator, m_function));
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void define_plot_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PlotPage(m_interpolator, m_function));
        }

        private void FormulaCopyNormal_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(m_interpolator.Polynomial.ToString());
        }

        private void FormulaCopyLatex_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(m_interpolator.Polynomial.Latexise());
        }
    }
}
