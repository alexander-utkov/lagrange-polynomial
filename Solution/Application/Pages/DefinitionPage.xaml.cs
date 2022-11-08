using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NumericalMethods.Pages
{
    public partial class DefinitionPage : Page
    {
        public DefinitionPage()
        {
            InitializeComponent();
            InitializeShortcuts();

            Points = new ObservableCollection<Core.Point>() { new Core.Point() };
            definition.ItemsSource = Points;
        }

        public static string[] RequiredLocalizedResourceDictionaries = new string[]
        {
            "CommandDictionary.xaml",
        };

        public ObservableCollection<Core.Point> Points { get; set; }

        public static RoutedCommand EditContinueCommand = new RoutedCommand(
            "definition.edit.continue",
            typeof(DefinitionPage),
            new InputGestureCollection() { new KeyGesture(Key.Space) }
        );

        /*
        public static RoutedCommand EditFunctionCommand = new RoutedCommand(
            "definition.edit.function",
            typeof(DefinitionPage),
            new InputGestureCollection() { new KeyGesture(Key.Space, ModifierKeys.Shift) }
        );
        */

        public static RoutedCommand InterpolateCommand = new RoutedCommand(
            "definition.interpolate",
            typeof(DefinitionPage),
            new InputGestureCollection() { new KeyGesture(Key.S, ModifierKeys.Control) }
        );

        public static RoutedCommand RowRemoveCommand = new RoutedCommand(
            "definition.edit.remove.row",
            typeof(DefinitionPage),
            new InputGestureCollection() { new KeyGesture(Key.Delete, ModifierKeys.Shift) }
        );

        /// <summary>
        /// Содержит созданные страницей в <see cref="MainWindow"/> привязки команд к собственным обработчикам.
        /// </summary>
        private List<CommandBinding> m_command_bindings = new List<CommandBinding>();

        /// <summary>
        /// Содержит созданные страницей в <see cref="MainWindow"/> привязки жестов к собственным командам.
        /// </summary>
        private List<InputBinding> m_input_bindings = new List<InputBinding>();

        private void InitializeShortcuts()
        {
            shortcuts.Commands.Add(EditContinueCommand);
            //shortcuts.Commands.Add(EditFunctionCommand);
            shortcuts.Commands.Add(InterpolateCommand);
            shortcuts.Commands.Add(RowRemoveCommand);

            // Выводим стандартные команды пользователю

            List<string> gestures;

            gestures = new List<string>() { "Shift", "Enter" };
            shortcuts.AddShortcut("definition.navigation.up", gestures);

            gestures = new List<string>() { "Enter" };
            shortcuts.AddShortcut("definition.navigation.down", gestures);

            gestures = new List<string>() { "ЛКМ", "F2" };
            shortcuts.AddShortcut("definition.edit.cell", gestures, "/");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeCommands();
        }

        /// <summary>
        /// Инициализирует команды страницы и их обработчики.
        /// </summary>
        private void InitializeCommands()
        {
            InitializeCommand(EditContinueCommand, EditContinue);
            //InitializeCommand(EditFunctionCommand, EditFunction);
            InitializeCommand(InterpolateCommand, Interpolate);
            InitializeCommand(RowRemoveCommand, RowRemove);
        }

        /// <summary>
        /// Создает привязки в главном окне, включая привязку обработчика к команде, а также привязки жестов команды.
        /// </summary>
        /// <param name="command">Инициализируемая команда.</param>
        /// <param name="handler">Обработчик команды.</param>
        private void InitializeCommand(RoutedCommand command, ExecutedRoutedEventHandler handler)
        {
            // Создаем привязку команды к обработчику в главном окне
            var command_binding = new CommandBinding(command, handler);
            Application.Current.MainWindow.CommandBindings.Add(command_binding);
            m_command_bindings.Add(command_binding);

            // Создаем привязки жестов к команде в главном окне
            foreach (InputGesture gesture in command.InputGestures)
            {
                var input_binding = new InputBinding(command, gesture);
                Application.Current.MainWindow.InputBindings.Add(input_binding);
                m_input_bindings.Add(input_binding);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            UninitializeCommands();
        }

        /// <summary>
        /// Удаляет привязки из главного окна, созданные при вызовах <see cref="InitializeCommand"/>.
        /// </summary>
        private void UninitializeCommands()
        {
            // Удаляем привязки собственных команд из главного окна
            foreach (var binding in m_command_bindings)
            {
                Application.Current.MainWindow.CommandBindings.Remove(binding);
            }
            m_command_bindings.Clear();

            // Удаляем привязки собственных жестов из главного окна
            foreach (var binding in m_input_bindings)
            {
                Application.Current.MainWindow.InputBindings.Remove(binding);
            }
            m_input_bindings.Clear();
        }

        /// <summary>
        /// Начинает редактирование первой незаполненной ячейки.
        /// </summary>
        private void EditContinue(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (var point in Points)
            {
                if (point.IsDefinedX == false)
                {
                    definition.CurrentCell = new DataGridCellInfo(point, definition.Columns[0]);
                }
                else if (point.IsDefinedY == false)
                {
                    definition.CurrentCell = new DataGridCellInfo(point, definition.Columns[1]);
                }
                else continue;

                definition.BeginEdit();
            }
        }

        private void Interpolate(object sender, ExecutedRoutedEventArgs e)
        {
            interpolate_Click(sender, null);
        }

        private void RowRemove(object sender, ExecutedRoutedEventArgs e)
        {
            var point = definition.CurrentItem as Core.Point;
            var index = Points.IndexOf(point);
            var column = definition.CurrentCell.Column;
            
            Points.Remove(point);
            
            // Задаем фокусировку на иной элемент
            if (Points.Count == 0)
            {
                // Если удалить все элементы, то пользователь не сможет создать ни одного нового. Создаем точку:
                point = new Core.Point();
                column = definition.Columns[0];
                Points.Add(point);
            }
            else if (index > Points.Count - 1) index--;
            if (index < 0) index = 0;

            definition.CurrentCell = new DataGridCellInfo(Points[index], column);
        }

        private void definition_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (e.Column == definition.CurrentColumn) // Обрабатываем событие только для измененной ячейки
                {
                    // Проверяем новое значение
                    var element = e.EditingElement as TextBox;
                    bool is_valid = Core.Point.ValidateValue(element.Text);
                    if (is_valid == false)
                    {
                        e.Cancel = true;
                        definition.BeginEdit();
                        return;
                    }

                    // Производим навигацию до следующей ячейки (строки)
                    if (definition.CurrentColumn == definition.Columns[0])
                    {
                        // TODO: К следующей незаполненной.
                        var point = definition.CurrentItem;
                        definition.CurrentCell = new DataGridCellInfo(point, definition.Columns[1]);
                    }
                    else
                    {
                        var point = new Core.Point();
                        Points.Add(point);
                        definition.CurrentCell = new DataGridCellInfo(point, definition.Columns[0]);
                    }
                }
            }
        }

        private void interpolate_Click(object sender, RoutedEventArgs e)
        {
            List<double> data_x = new List<double>();
            List<double> data_y = new List<double>();
            foreach (Core.Point point in Points)
            {
                if (point.IsDefined == true)
                {
                    data_x.Add(point.X);
                    data_y.Add(point.Y);
                }
                else
                {
                    // TODO: Уведомить пользователя об исключении точки.
                }
            }
            if (data_x.Count > 0)
            {
                var interpolator = new Core.LagrangeInterpolator(data_x, data_y);
                NavigationService.Navigate(new SolutionPage(interpolator));
            }
            else
            {
                // TODO: Уведомить пользователя об необходимости ввода точек...
            }
        }
    }
}
