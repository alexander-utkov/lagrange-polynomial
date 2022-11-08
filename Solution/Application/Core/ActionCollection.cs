using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace NumericalMethods.Core
{
    /// <summary>
    /// Именованный список действий.
    /// </summary>
    public class ActionCollection : List<Action>
    {
        /// <summary>
        /// Создает экземпляр с возможностью загрузки наименования из ресурсов.
        /// </summary>
        /// <param name="key">Наименование списка или его ключ в ресурсах.</param>
        public ActionCollection(string key)
        {
            m_key = key;
        }

        protected string m_key;

        /// <summary>
        /// Наименование списка.
        /// </summary>
        public string Name => Application.Current.TryFindResource(m_key) as string;
    }
}
