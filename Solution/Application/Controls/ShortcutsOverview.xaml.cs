using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

// REVIEW, DOCS, TODO

namespace NumericalMethods.Controls
{
    /// <summary>
    /// Элемент управления для отображения множества именованных сочетаний (альтернатив) жестов.
    /// </summary>
    public partial class Shortcuts : UserControl
    {
        public Shortcuts()
        {
            InitializeComponent();

            Commands = new ObservableCollection<RoutedCommand>();
            Commands.CollectionChanged += Commands_CollectionChanged;
        }

        /// <summary>
        /// Получает коллекцию команд, которые отображаются элементом управления. Сочетание именуется строковым ресурсом
        /// по ключу <see cref="RoutedCommand.Name"/> или же им самим.
        /// </summary>
        public ObservableCollection<RoutedCommand> Commands { get; }

        private void Commands_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (RoutedCommand command in e.NewItems)
                {
                    AddShortcut(command);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (RoutedCommand command in e.OldItems)
                {
                    RemoveShortcut(command);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (RoutedCommand command in e.OldItems)
                {
                    RemoveShortcut(command);
                }
                foreach (RoutedCommand command in e.NewItems)
                {
                    AddShortcut(command);
                }
                // TODO: Сохранить порядок команд как в Commands.
            }
            else if (e.Action == NotifyCollectionChangedAction.Move)
            {
                // TODO: Изменить порядок команд как в Commands. (?)
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                grid.Children.Clear();
                grid.RowDefinitions.Clear();
                // FIXME: Исключить удаление сочетаний, добавленных отдельно от Commands.
            }
        }

        private void AddShortcut(RoutedCommand command)
        {
            var gestures = GetGestures(command.InputGestures);
            AddShortcut(command.Name, gestures);
        }

        private IReadOnlyCollection<string> GetGestures(InputGestureCollection collection)
        {
            List<string> gestures = new List<string>();
            foreach (var gesture in collection)
            {
                if (gesture.GetType() == typeof(KeyGesture))
                {
                    var data = (KeyGesture)gesture;

                    if (((int)data.Modifiers & (int)ModifierKeys.Windows) > 0) gestures.Add("Win");
                    if (((int)data.Modifiers & (int)ModifierKeys.Control) > 0) gestures.Add("Ctrl");
                    if (((int)data.Modifiers & (int)ModifierKeys.Shift) > 0) gestures.Add("Shift");
                    if (((int)data.Modifiers & (int)ModifierKeys.Alt) > 0) gestures.Add("Alt");

                    gestures.Add(data.Key.ToString());
                }
                // TODO: Поддержка MouseGesture.
                else
                {
                    var unexpected = gesture.GetType().Name;
                    throw new System.NotImplementedException($"Command with not supported Gesture type '{unexpected}'.");
                }
            }
            return gestures.AsReadOnly();
        }

        public void AddShortcut(string description_key, IReadOnlyCollection<string> gestures, string separator="+")
        {
            string description = description_key;
            if (Resources.Contains(description_key))
            {
                description = (string)Resources[description_key];
            }

            // Добавляем в конец новую строку
            var definition = new RowDefinition()
            {
                Height = new GridLength(32)
            };
            grid.RowDefinitions.Add(definition);

            // Выводим описание команды слева
            UIElement description_element = new TextBlock()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Text = description
            };
            Grid.SetColumn(description_element, 0);
            Grid.SetRow(description_element, grid.RowDefinitions.Count - 1);
            grid.Children.Add(description_element);

            // Выводим сочетание клавиш (кнопок) справа
            Shortcut shortcut = new Shortcut(gestures, separator)
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            Grid.SetColumn(shortcut, 2);
            Grid.SetRow(shortcut, grid.RowDefinitions.Count - 1);
            grid.Children.Add(shortcut);
        }

        private void RemoveShortcut(RoutedCommand command)
        {
            throw new System.NotImplementedException("TODO");
        }
    }
}
