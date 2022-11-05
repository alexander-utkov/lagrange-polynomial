using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace NumericalMethods.Pages
{
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown += Page_KeyDown;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown -= Page_KeyDown;
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            start_Click(sender, null);
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DefinitionPage());
        }
    }
}
