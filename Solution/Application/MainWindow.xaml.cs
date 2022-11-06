using System.Windows.Input;
using System.Windows;
using NumericalMethods.Pages;
using System.Windows.Navigation;

namespace NumericalMethods
{
    public partial class MainWindow : HandyControl.Controls.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && page.NavigationService.CanGoBack)
            {
                page.NavigationService.GoBack();
            }
        }
    }
}
