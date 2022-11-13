using ScottPlot;
using System.Windows;
using System.Windows.Controls;

namespace NumericalMethods
{
    public static class PlotHelper
    {
        public static void OverrideContextMenu(WpfPlot plot)
        {
            plot.RightClicked -= plot.DefaultRightClickEvent;
            plot.RightClicked += (sender, e) =>
            {
                var menu = new ContextMenu();

                var AutoAxisMenuItem = new MenuItem()
                {
                    Header = Application.Current.FindResource("plot.fit") as string
                };
                AutoAxisMenuItem.Click += (s, ev) =>
                {
                    plot.Plot.AxisAuto();   
                    plot.Refresh();
                };
                menu.Items.Add(AutoAxisMenuItem);

                menu.IsOpen = true;
            };
        }
    }
}
