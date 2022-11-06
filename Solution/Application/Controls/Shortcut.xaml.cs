using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NumericalMethods.Controls
{
    /// <summary>
    /// Элемент управления для отображения сочетаний (альтернатив) жестов.
    /// </summary>
    public partial class Shortcut : UserControl
    {
        /// <summary>
        /// Создает экземпляр с возможностью отложенного определения единственного жеста <see cref="Gesture"/> или жеста
        /// из составных блоков <see cref="Gestures"/>, разделенных <see cref="Separator"/>.
        /// </summary>
        public Shortcut()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Создает экземпляр с немедленным определением единственного жеста.
        /// </summary>
        /// <param name="gesture">Строковое представление жеста.</param>
        public Shortcut(string gesture)
        {
            InitializeComponent();

            // NOTE: Избегайте setter'ов, чтобы не вызывать RebuildLayout до события Loaded.

            SetValue(GestureProperty, gesture);
        }

        /// <summary>
        /// Создает экземпляр с немедленным определением сочетаний (альтернатив) жестов.
        /// </summary>
        /// <param name="gestures">Строковые представления жестов.</param>
        /// <param name="separator">Разделитель жестов <paramref name="gestures"/>.</param>
        public Shortcut(IReadOnlyCollection<string> gestures, string separator = "+")
        {
            InitializeComponent();

            // NOTE: Избегайте setter'ов, чтобы не вызывать RebuildLayout до события Loaded.

            SetValue(SeparatorProperty, separator);
            SetValue(GesturesProperty, gestures);
        }

        /// <summary>
        /// Получает или задает разделитель жестов <see cref="Gestures"/>. Может использоваться для определения жестов в
        /// качестве сочетания (Ctrl + S) или, например, альтернатив (F2 / ЛКМ).
        /// </summary>
        public string Separator
        {
            get => (string)GetValue(SeparatorProperty);
            set
            {
                SetValue(SeparatorProperty, value);

                RebuildLayout();
            }
        }

        public static readonly DependencyProperty SeparatorProperty = DependencyProperty.Register(
            "Separator", typeof(string), typeof(Shortcut), new PropertyMetadata("+")
        );

        /// <summary>
        /// Получает первый жест <see cref="Gestures"/> или задает единственный жест для отображения.
        /// </summary>
        public string Gesture
        {
            get => (string)GetValue(GestureProperty);
            set
            {
                SetValue(GestureProperty, value);
                SetValue(GesturesProperty, new List<string>() { value }.AsReadOnly());

                RebuildLayout();
            }
        }

        public static readonly DependencyProperty GestureProperty = DependencyProperty.Register(
            "Gesture", typeof(string), typeof(Shortcut), new PropertyMetadata("")
        );

        /// <summary>
        /// Получает или задает жесты для отображения.
        /// </summary>
        public IReadOnlyCollection<string> Gestures
        {
            get => (IReadOnlyCollection<string>)GetValue(GesturesProperty);
            set
            {
                SetValue(GestureProperty, value.ElementAtOrDefault(0));
                SetValue(GesturesProperty, value);

                RebuildLayout();
            }
        }

        public static readonly DependencyProperty GesturesProperty = DependencyProperty.Register(
            "Gestures", typeof(IReadOnlyCollection<string>), typeof(Shortcut)
        );

        /// <summary>
        /// Отступ между разделителем и жестом.
        /// </summary>
        private const int m_padding = 3;

        private static Thickness m_block_child_margin = new Thickness(m_padding);
        private static Thickness m_separator_margin = new Thickness(6, m_padding, 6, m_padding);
        private static CornerRadius m_block_corner_radius = new CornerRadius(6);
        private static Brush m_brush_back = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        private static Brush m_brush_fore = new SolidColorBrush(Color.FromRgb(33, 33, 33));

        private void Shortcut_Loaded(object sender, RoutedEventArgs e)
        {
            // Если определено свойство Gesture в XAML, то отправляем его в свойство Gestures:
            var gesture = GetValue(GestureProperty) as string;
            if (string.IsNullOrEmpty(gesture) == false)
            {
                SetValue(GesturesProperty, new List<string>() { gesture }.AsReadOnly());
            }
            // Если определено свойство Gestures в XAML, то все в порядке.

            RebuildLayout();
        }

        protected void RebuildLayout()
        {
            ClearLayout();

            if (Gestures != null)
            {
                bool is_first = true;
                foreach (string gesture in Gestures)
                {
                    if (is_first == false)
                    {
                        UIElement separator = new TextBlock()
                        {
                            Margin = m_separator_margin,
                            Text = Separator
                        };
                        panel.Children.Add(separator);
                    }

                    AddGestureToLayout(gesture);
                    is_first = false;
                }
            }
        }

        protected void ClearLayout()
        {
            panel.Children.Clear();
        }

        protected void AddGestureToLayout(string gesture)
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
