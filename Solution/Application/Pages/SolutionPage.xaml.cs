using NumericalMethods.Core;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace NumericalMethods.Pages
{
    public partial class SolutionPage : Page
    {
        public SolutionPage(Core.IInterpolator interpolator)
        {
            InitializeComponent();

            m_interpolator = interpolator;
            m_plot_info = new PlotInfo(m_interpolator.DataX.Min() - 1, m_interpolator.DataX.Max() + 1);

            polynomial.Formula = "P(x)=" + interpolator.Polynomial.Latexise();

            FigurePlot();
        }

        private IInterpolator m_interpolator;
        private PlotInfo m_plot_info;

        private void FigurePlot()
        {
            List<double> data_x = new List<double>();
            List<double> data_y = new List<double>();
            for (double x = m_plot_info.From; x < m_plot_info.To; x += m_plot_info.Step)
            {
                data_x.Add(x);
                data_y.Add(m_interpolator.Interpolate(x));
            }
            plot.Plot.AddScatter(data_x.ToArray(), data_y.ToArray());
            plot.Refresh();
        }

        private void details_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new WalkthroughPage(m_interpolator));
        }

        private void back_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void define_plot_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }
    }
}
