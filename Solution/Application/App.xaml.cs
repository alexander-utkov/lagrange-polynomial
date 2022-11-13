using HandyControl.Tools;
using NumericalMethods.Core;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

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

        /// <summary>
        /// Обновляет <see cref="Culture"/> в соответствии с предпочитаемой локализацией пользовательского интерфейса.
        /// </summary>
        private void ChangeCulture()
        {
            try
            {
                Culture = new CultureInfo(Preferences.Language);
                ConfigHelper.Instance.SetLang(Preferences.Language);
            }
            catch
            {
                Culture = new CultureInfo("ru");
            }
            Thread.CurrentThread.CurrentCulture = Culture;
            Thread.CurrentThread.CurrentUICulture = Culture;
        }

        /// <summary>
        /// Получает словарь ресурсов из файла.
        /// </summary>
        /// <remarks>
        /// Относительный путь задается по отношению к каталогу локализированных ресурсов «Assets/Values/{LC}/» при
        /// <paramref name="localized"/> равном true (по умолчанию), а при false - по отношению к каталогу проекта.
        /// </remarks>
        /// <param name="path">Относительный путь к словарю.</param>
        /// <param name="localized">Является ли словарь локализированным.</param>
        /// <returns>Возвращает загруженный словарь.</returns>
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
