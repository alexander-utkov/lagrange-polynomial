using System.Collections.Generic;
using System.Windows;

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
            object name = Application.Current.TryFindResource(key);
            Name = name == null ? key : name as string;
        }

        /// <summary>
        /// Наименование списка.
        /// </summary>
        public readonly string Name;
    }
}
