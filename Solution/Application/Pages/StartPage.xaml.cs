using HandyControl.Controls;
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

            lang.SelectedValue = App.Preferences.Language;
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
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                start_Click(sender, null);
            }
            else if (e.Key == Key.F1)
            {
                Growl.Info(FindResource("navigation") as string);
            }
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DefinitionPage());
        }

        private void lang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var element = e.AddedItems[0] as ComboBoxItem;
            App.Preferences.Language = element.Content as string;
            App.Preferences.CommitChanges();
        }
    }
}
