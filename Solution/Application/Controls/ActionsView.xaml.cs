using NumericalMethods.Core;
using System.Windows;
using System.Windows.Controls;
using WpfMath.Controls;

namespace NumericalMethods.Controls
{
    public partial class ActionsView : UserControl
    {
        public ActionsView(Action action)
        {
            InitializeComponent();
            AddActionToLayout(action);
        }

        private static Thickness m_margin = new Thickness(0, 0, 0, 12);

        public ActionsView(ActionCollection actions)
        {
            InitializeComponent();
            foreach (Action action in actions)
            {
                AddActionToLayout(action);
            }
        }

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
