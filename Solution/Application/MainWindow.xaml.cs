using NumericalMethods.Core;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace NumericalMethods
{
    public partial class MainWindow : HandyControl.Controls.Window
    {
        public MainWindow()
        {
            InitializeResources();
            InitializeComponent();

            App.Preferences.PropertyChanged += Preferences_PropertyChanged;
        }

        protected App App => App.Current as App;

        private void InitializeResources()
        {
            Resources = new ResourceDictionary();
            Resources.MergedDictionaries.Add(App.GetResourceDictionary("WindowDictionary.xaml"));

            // FIXME: LagrangeInterpolator рано запрашивает ресурс, словарь которого еще не загружен, и запоминает null.
            Resources.MergedDictionaries.Add(App.GetResourceDictionary("ActionDictionary.xaml"));
        }

        private void Preferences_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Preferences.Language))
            {
                InitializeResources();
            }
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
