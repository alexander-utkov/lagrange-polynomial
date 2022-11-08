using NumericalMethods.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace NumericalMethods.Pages
{
    /// <summary>
    /// Страница приложения.
    /// </summary>
    /// <remarks>
    /// Реализует общие потребности страниц приложения, включая динамическую загрузку локализованного словаря ресурсов,
    /// а также всех локализованных, требуемых страницей посредством определения статического поля:
    /// 
    /// <code>
    /// public static string[] RequiredLocalizedResourceDictionaries = new string[]
    /// {
    ///     "CookbookDictionary.xaml", // «Assets/Values/{LC}/CookbookDictionary.xaml»
    /// };
    /// </code>
    /// 
    /// При изменении предпочитаемого языка пользовательского интерфейса, все локализованные словари перезагрузятся.
    /// </remarks>
    public class Page : System.Windows.Controls.Page
    {
        public Page()
        {
            InitializeResources();

            App.Preferences.PropertyChanged += Preferences_PropertyChanged;
        }

        protected App App => App.Current as App;

        /// <summary>
        /// Хранит динамически загруженные словари ресурсов.
        /// </summary>
        private List<ResourceDictionary> m_loaded = new List<ResourceDictionary>();

        /// <summary>
        /// (Пере-) загружает локализованные словари ресурсов.
        /// </summary>
        protected void InitializeResources()
        {
            // Загруженные словари ресурсов более недействительны, однако, во избежание проблем от отсутствия словарей,
            // не удаляем их до окончания загрузки новых: данные будут использоваться, и будут в приоритете по слиянию.
            var invalidated = m_loaded.ToArray();
            
            // Убираем недействительные сейчас, чтобы не смешивать их с загружаемыми
            m_loaded.Clear();

            // Загружаем локализованный словарь ресурсов, соответствующий наименованию производного класса
            LoadResourceDictionary($"Pages/{GetName()}.xaml");
            
            // Загружаем локализованные словари ресурсов, требуемые производным классом
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

            // Выгружаем недействительные и начинаем использовать загруженные
            foreach (var resource in invalidated)
            {
                App.Resources.MergedDictionaries.Remove(resource);
            }
        }

        /// <summary>
        /// Получает наименование производного класса без окончания «Page».
        /// </summary>
        /// <returns>Возвращает наименование.</returns>
        private string GetName()
        {
            var result = GetType().Name;
            if (result.EndsWith("Page"))
            {
                return result.Substring(0, result.Length - 4);
            }
            return result;
        }

        /// <summary>
        /// Загружает локализованный словарь ресурсов.
        /// </summary>
        /// <param name="path">Расположение словаря относительно каталога «Assets/Values/{LC}/».</param>
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
