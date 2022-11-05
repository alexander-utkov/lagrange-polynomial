using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NumericalMethods.Controls
{
    public partial class Shortcut : UserControl
    {
        public Shortcut()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                // По загрузке вызываем setter'ы свойств зависимостей, ибо XAML отправляет их значения непосредственно в
                // соответствующие DependencyProperty, тем самым пропуская создание блока жеста.
                Gesture = Gesture;
            };
        }

        public Shortcut(IList<string> gestures, string separator = "+")
        {
            InitializeComponent();
            SetGestures(gestures, separator);
        }
        
        public string Gesture
        {
            get => (string)GetValue(GestureProperty);
            set
            {
                Clear();
                AddBlock(value);
                SetValue(GestureProperty, value);
            }
        }

        public static readonly DependencyProperty GestureProperty = DependencyProperty.Register(
            "Gesture", typeof(string), typeof(Shortcut), new PropertyMetadata("")
        );

        private const int padding = 3;
        private static Thickness m_block_child_margin = new Thickness(padding);
        private static Thickness m_plus_margin = new Thickness(6, padding, 6, padding);
        private static CornerRadius m_block_corner_radius = new CornerRadius(6);
        private static Brush m_brush_back = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        private static Brush m_brush_fore = new SolidColorBrush(Color.FromRgb(33, 33, 33));

        public void SetGestures(IList<string> gestures, string separator = "+")
        {
            Clear();

            bool is_first = true;
            foreach (string gesture in gestures)
            {
                if (is_first == false)
                {
                    UIElement plus = new TextBlock()
                    {
                        Margin = m_plus_margin,
                        Text = separator
                    };
                    panel.Children.Add(plus);
                }

                AddBlock(gesture);
                is_first = false;
            }
        }

        protected void Clear()
        {
            panel.Children.Clear();
        }

        protected void AddBlock(string gesture)
        {
            UIElement block = new Border()
            {
                Background = m_brush_back,
                CornerRadius = m_block_corner_radius,
                Child = new TextBlock()
                {
                    Margin = m_block_child_margin,
                    Foreground = m_brush_fore,
                    Text = gesture
                }
            };
            panel.Children.Add(block);
        }
    }
}
