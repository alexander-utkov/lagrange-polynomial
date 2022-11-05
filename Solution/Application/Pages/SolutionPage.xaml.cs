using AngouriMath.Extensions;
using ScottPlot;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Markup;

namespace NumericalMethods.Pages
{
    public partial class SolutionPage : Page
    {
        public SolutionPage(Core.IInterpolator interpolator)
        {
            InitializeComponent();
            
            polynom.Formula = interpolator.Polynom.Latexise();

            FigurePlot(interpolator);
        }

        private void FigurePlot(Core.IInterpolator interpolator)
        {
            List<double> data_x = new List<double>();
            List<double> data_y = new List<double>();
            for (double x = interpolator.Plot.From; x < interpolator.Plot.To; x += interpolator.Plot.Step)
            {
                data_x.Add(x);
                data_y.Add(interpolator.GetValue(x));
            }
            plot.Plot.AddScatter(data_x.ToArray(), data_y.ToArray());
            plot.Refresh();
        }

        private void back_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
