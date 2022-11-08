using NumericalMethods.Core;
using System.Windows;
using System.Windows.Controls;
using WpfMath.Controls;

namespace NumericalMethods.Controls
{
    /// <summary>
    /// Элемент управления для отображения <see cref="Action"/> или <see cref="ActionCollection"/>.
    /// </summary>
    public partial class ActionsView : UserControl
    {
        /// <summary>
        /// Создает экземпляр с единственным действием <see cref="Action"/>.
        /// </summary>
        /// <param name="action">Отображаемое действие.</param>
        public ActionsView(Action action)
        {
            InitializeComponent();
            AddActionToLayout(action);
        }

        /// <summary>
        /// Создает экземпляр со множеством действий <see cref="Action"/>.
        /// </summary>
        /// <param name="actions">Коллекция отображаемых действий.</param>
        public ActionsView(ActionCollection actions)
        {
            InitializeComponent();
            foreach (Action action in actions)
            {
                AddActionToLayout(action);
            }
        }

        private static Thickness m_margin = new Thickness(0, 0, 0, 12);

        /// <summary>
        /// Добавляет действие на макет.
        /// </summary>
        /// <param name="action">Добавляемое действие</param>
        private void AddActionToLayout(Action action)
        {
            if (string.IsNullOrEmpty(action.Description) == false)
            {
                UIElement description = new TextBlock()
                {
                    Margin = m_margin,
                    Text = action.Description
                };
                panel.Children.Add(description);
            }

            if (string.IsNullOrEmpty(action.Content) == false)
            {
                UIElement content = new FormulaControl()
                {
                    Margin = m_margin,
                    Formula = action.Content
                };
                panel.Children.Add(content);
            }
        }
    }
}
