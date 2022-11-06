using HandyControl.Controls;
using System.Windows;

namespace NumericalMethods
{
    public class StepPropertyEditor : PropertyEditorBase
    {
        public StepPropertyEditor()
        { }

        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            var element = new NumericUpDown
            {
                IsReadOnly = propertyItem.IsReadOnly,
                Maximum = double.MaxValue,
                Minimum = 0.005,
                Increment = 0.005,
            };

            return element;
        }

        public override DependencyProperty GetDependencyProperty() => NumericUpDown.ValueProperty;
    }
}
