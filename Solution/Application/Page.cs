using NumericalMethods.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace NumericalMethods.Pages
{
    public class Page : System.Windows.Controls.Page
    {
        public Page()
        {
            InitializeResources();

            App.Preferences.PropertyChanged += Preferences_PropertyChanged;
        }

        protected App App => App.Current as App;

        private List<ResourceDictionary> m_loaded = new List<ResourceDictionary>();

        protected void InitializeResources()
        {
            var invalidated = m_loaded.ToArray();
            m_loaded.Clear();

            LoadResourceDictionary($"Pages/{GetName()}.xaml");
            
            var name = "RequiredLocalizedResourceDictionaries";
            var flags = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public;
            var property = GetType().GetField(name, flags);
            var required = property?.GetValue(this);
            if (required != null)
            {
                foreach (string path in required as IEnumerable<string>)
                {
                    LoadResourceDictionary(path);
                }
            }

            foreach (var resource in invalidated)
            {
                App.Resources.MergedDictionaries.Remove(resource);
            }
        }

        private string GetName()
        {
            var result = GetType().Name;
            if (result.EndsWith("Page"))
            {
                return result.Substring(0, result.Length - 4);
            }
            return result;
        }

        protected void LoadResourceDictionary(string path)
        {
            var dictionary = App.GetResourceDictionary(path);
            App.Resources.MergedDictionaries.Add(dictionary);
            m_loaded.Add(dictionary);
        }

        private void Preferences_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Preferences.Language))
            {
                InitializeResources();
            }
        }
    }
}
