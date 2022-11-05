using System.Collections.Generic;
using System.Windows;

namespace NumericalMethods.Core
{
    public class ActionCollection : List<Action>
    {
        public ActionCollection(string key)
        {
            object name = Application.Current.TryFindResource(key);
            Name = name == null ? key : name as string;
        }

        public readonly string Name;
    }
}
