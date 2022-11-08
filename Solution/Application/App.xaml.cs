using NumericalMethods.Core;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace NumericalMethods
{
    public partial class App : Application
    {
        public App()
        {
            Preferences = Preferences.Instance;
            Preferences.PropertyChanged += Preferences_PropertyChanged;

            ChangeCulture();
        }

        public Preferences Preferences { get; }
        public CultureInfo Culture { get; private set; }

        private void Preferences_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Preferences.Language))
            {
                ChangeCulture();
            }
        }

        private void ChangeCulture()
        {
            try
            {
                Culture = new CultureInfo(Preferences.Language);
            }
            catch
            {
                Culture = new CultureInfo("ru");
            }
        }

        public ResourceDictionary GetResourceDictionary(string path, bool localized = true)
        {
            if (localized == true)
            {
                path = $"/NumericalMethods;Component/Assets/Values/{Preferences.Language}/{path}";
            }

            var uri = new Uri(path, UriKind.Relative);
            var result = LoadComponent(uri) as ResourceDictionary;
            result.Source = uri;
            return result;
        }
    }
}
