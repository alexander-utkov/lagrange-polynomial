using HandyControl.Controls;
using System.Windows;

namespace NumericalMethods
{
    public class DomainPropertyEditor : PropertyEditorBase
    {
        public DomainPropertyEditor()
        { }

        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            var element = new NumericUpDown
            {
                IsReadOnly = propertyItem.IsReadOnly,
                Maximum = double.MaxValue,
                Minimum = double.MinValue,
                Increment = 5,
            };

            return element;
        }

        public override DependencyProperty GetDependencyProperty() => NumericUpDown.ValueProperty;
    }
}
