using AngouriMath;
using AngouriMath.Extensions;
using System;
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

        public static RoutedCommand EditFunctionCommand = new RoutedCommand(
            "definition.edit.function",
            typeof(DefinitionPage),
            new InputGestureCollection() { new KeyGesture(Key.Space, ModifierKeys.Shift) }
        );

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

        /// <summary>
        /// Исходная функция f(x). Может быть null.
        /// </summary>
        private Entity m_function_entity = null;

        /// <summary>
        /// Скомпилированная исходная функция f(x) для выполнения вычислений. Может быть null.
        /// </summary>
        private Func<double, double> m_function_compiled = null;

        /// <summary>
        /// Объект для синхронизации доступа к <see cref="m_function_compiled"/>. Не может быть null.
        /// </summary>
        /// <remarks>
        /// Значение не имеет значения. Объект используется в выражении lock.
        /// </remarks>
        private object m_function_lock = 'z';

        private void InitializeShortcuts()
        {
            shortcuts.Commands.Add(EditContinueCommand);
            shortcuts.Commands.Add(EditFunctionCommand);
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
            InitializeCommand(EditFunctionCommand, EditFunction);
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
            EditNextPoint(false);
            definition.BeginEdit();
        }

        private void EditFunction(object sender, ExecutedRoutedEventArgs e)
        {
            function.Focus();
        }

        private void Interpolate(object sender, ExecutedRoutedEventArgs e)
        {
            interpolate_Click(sender, null);
        }

        private void RowRemove(object sender, ExecutedRoutedEventArgs e)
        {
            var point = definition.CurrentItem as Core.Point;
            var index = Points.IndexOf(point);
            var column = definition.CurrentColumn;

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
                    var column = definition.Columns.IndexOf(e.Column);
                    if (column == 1 && m_function_compiled != null)
                    {
                        InvalidateFunction();
                    }

                    var element = e.EditingElement as TextBox;
                    var validation = Core.Point.TryParseValue(element.Text);
                    var is_valid = validation.Item1;
                    var value = validation.Item2;

                    if (is_valid == false)
                    {
                        e.Cancel = true;
                        // TODO: Уведомить пользователя о недействительности значения.
                        return;
                    }

                    // Избегаем повторения абсцисс
                    if (column == 0)
                    {
                        foreach (Core.Point point in Points)
                        {
                            if (point.IsDefinedX == true && point.X == value)
                            {
                                e.Cancel = true;
                                // TODO: Уведомить пользователя.
                                return;
                            }
                        }
                    }

                    var preview_cell_value = new PreviewCellValue()
                    {
                        Point = e.Row.Item as Core.Point,
                        Column = column,
                        Value = value
                    };
                    EditNextPoint(true, preview_cell_value);
                }
            }
        }

        /// <summary>
        /// Переходит на следующую ячейку на данной строке или ниже.
        /// </summary>
        /// <remarks>
        /// Заполненные ячейки пропускаются, а переход по ним производится слева направо и сверху вниз. Если определена
        /// исходная функция, то переход на ординаты не происходит. Вместо этого вызывается функция для их вычисления.
        /// </remarks>
        /// <param name="is_continue">Если true, то проверка начнется с текущей точки; иначе, с первой.</param>
        /// <param name="preview_cell_value">Предварительный просмотр значения ячейки при событиях изменения.</param>
        private void EditNextPoint(bool is_continue = true, PreviewCellValue? preview_cell_value = null)
        {
            if (definition.CurrentItem == null)
            {
                definition.CurrentItem = Points[0];
            }

            lock (m_function_lock)
            {
                var start_index = definition.Items.IndexOf(definition.CurrentItem);
                for (int index = start_index; index < definition.Items.Count; index++)
                {
                    var point = definition.Items[index] as Core.Point;

                    var has_preview = preview_cell_value.HasValue ? preview_cell_value.Value.Point == point : false;
                    var has_abscissa = point.IsDefinedX || (has_preview && preview_cell_value.Value.Column == 0);
                    var has_ordinate = point.IsDefinedY || (has_preview && preview_cell_value.Value.Column == 1);

                    if (has_abscissa == false)
                    {
                        BeginEditCell(point, 0);
                    }
                    else if (has_ordinate == false)
                    {
                        if (m_function_compiled == null)
                        {
                            BeginEditCell(point, 1);
                        }
                        else
                        {
                            double value = point.X;
                            if (has_preview && preview_cell_value.Value.Column == 0)
                            {
                                value = preview_cell_value.Value.Value;
                            }

                            bool success = TryEvalOrdinate(point, value);
                            if (success == false)
                            {
                                BeginEditCell(point, 1);
                            }
                            else break;
                        }
                    }
                    else continue;
                    return;
                }
            }

            var new_point = new Core.Point();
            Points.Add(new_point);
            BeginEditCell(new_point, 0);
        }

        private void BeginEditCell(Core.Point point, int column)
        {
            definition.CurrentCell = new DataGridCellInfo(point, definition.Columns[column]);
        }

        private bool TryEvalOrdinate(Core.Point point, double x)
        {
            lock (m_function_lock)
            {
                if (m_function_compiled == null)
                {
                    return false;
                }

                point.Y = m_function_compiled(x);
            }
            return true;
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
                NavigationService.Navigate(new SolutionPage(interpolator, m_function_entity));
            }
            else
            {
                // TODO: Уведомить пользователя об необходимости ввода точек...
            }
        }

        private void InvalidateFunction()
        {
            lock (m_function_lock)
            {
                m_function_compiled = null;
                m_function_entity = null;
                function.Text = "";
            }
            // TODO: Уведомить пользователя.
        }

        private void function_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                lock (m_function_lock)
                {
                    m_function_entity = function.Text.ToEntity();
                    m_function_compiled = m_function_entity.Compile<double, double>("x");
                }
                ReEvalOrdinates();
            }
            catch
            {
                InvalidateFunction();
            }
        }

        private void ReEvalOrdinates()
        {
            foreach (Core.Point point in definition.Items)
            {
                if (point.IsDefinedX)
                {
                    TryEvalOrdinate(point, point.X);
                }
            }
        }

        private void function_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                definition.Focus();
                EditNextPoint(false);
                definition.BeginEdit();
            }
        }
    }
}
