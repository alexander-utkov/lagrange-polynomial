using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace NumericalMethods.Core
{
    /// <summary>
    /// Точка двумерной декартовой системы координат.
    /// </summary>
    /// <remarks>
    /// Назначение класса заключается в инициализации точки через текстовый ввод в пользовательском интерфейсе. При этом
    /// создается событие <see cref="PropertyChanged"/>. Его обработчик способен проверить состояние инициализации через
    /// свойство <see cref="IsDefined"/>.
    /// </remarks>
    public class Point : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        /// <summary>
        /// Создает точку без инициализации ее абсциссы и ординаты.
        /// </summary>
        public Point()
        { }

        /// <summary>
        /// Создает определенную точку.
        /// </summary>
        /// <param name="x">Значение на оси абсцисс.</param>
        /// <param name="y">Значение на оси ординат.</param>
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Создает точку из входных параметров, представленных в строковом виде.
        /// </summary>
        /// <remarks>
        /// Неинтерпретируемые записи значения ни вызывают исключения, ни инициализируют значения.
        /// </remarks>
        /// <param name="x">Значение на оси абсцисс.</param>
        /// <param name="y">Значение на оси ординат.</param>
        public Point(string x, string y)
        {
            SourceX = x;
            SourceY = y;
        }

        /// <summary>
        /// Событие, вызываемое при изменении значения на оси абсцисс или на оси ординат.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Событие, вызываемое при изменении свойства <see cref="HasErrors"/>.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected double m_x = double.NegativeInfinity;
        protected double m_y = double.NegativeInfinity;

        protected string m_x_source = "";
        protected string m_y_source = "";

        protected bool m_x_source_raw_invalid = false;
        protected bool m_y_source_raw_invalid = false;

        /// <summary>
        /// Получает или задает значение на оси абсцисс.
        /// </summary>
        /// <remarks>
        /// Значение синхронизировано с <see cref="SourceX"/>. При переопределении вызывается событие
        /// <see cref="PropertyChanged"/>.
        /// </remarks>
        public double X
        {
            get => m_x;
            set
            {
                m_x = value;
                m_x_source = value == double.NegativeInfinity ? "" : value.ToString("g", App.Culture);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(X)));
            }
        }

        /// <summary>
        /// Получает или задает значение на оси ординат.
        /// </summary>
        /// <remarks>
        /// Значение синхронизировано с <see cref="SourceY"/>. При переопределении вызывается событие
        /// <see cref="PropertyChanged"/>.
        /// </remarks>
        public double Y
        {
            get => m_y;
            set
            {
                m_y = value;
                m_y_source = value == double.NegativeInfinity ? "" : value.ToString("g", App.Culture);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Y)));
            }
        }

        /// <summary>
        /// Получает или задает значение на оси абсцисс (<see cref="X"/>).
        /// </summary>
        /// <remarks>
        /// При наличии ошибок в записи, исключение не вызывается и значение не меняется. Значение должно быть в формате
        /// <see cref="App.Culture"/>; разрешены разделители тысячных (1.000,5 / 1,000.5), экспоненциальная запись и прочее.
        /// </remarks>
        public string SourceX
        {
            get => m_x_source;
            set
            {
                var result = double.NegativeInfinity;
                var success = double.TryParse(value, NumberStyles.Any, App.Culture, out result);
                if (success == true)
                {
                    X = result;
                }
                else
                {
                    m_x_source = X == double.NegativeInfinity ? "" : X.ToString("g", App.Culture);
                }
                m_x_source_raw_invalid = !success;
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("SourceX"));
            }
        }

        /// <summary>
        /// Получает или задает значение на оси абсцисс (<see cref="Y"/>).
        /// </summary>
        /// <remarks>
        /// При наличии ошибок в записи, исключение не вызывается и значение не меняется. Значение должно быть в формате
        /// <see cref="App.Culture"/>; разрешены разделители тысячных (1.000,5 / 1,000.5), экспоненциальная запись и прочее.
        /// </remarks>
        public string SourceY
        {
            get => m_y_source;
            set
            {
                var result = double.NegativeInfinity;
                var success = double.TryParse(value, NumberStyles.Any, App.Culture, out result);
                if (success == true)
                {
                    Y = result;
                }
                else
                {
                    m_y_source = Y == double.NegativeInfinity ? "" : Y.ToString("g", App.Culture);
                }
                m_y_source_raw_invalid = !success;
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("SourceY"));
            }
        }

        /// <summary>
        /// Получает наличие ошибок при определении свойств <see cref="SourceX"/> и <see cref="SourceY"/>.
        /// </summary>
        public bool HasErrors => (m_x_source_raw_invalid || m_y_source_raw_invalid);

        /// <summary>
        /// Проверяет определенность значений точки на оси абсцисс.
        /// </summary>
        public bool IsDefinedX => ValidateValue(m_x);

        /// <summary>
        /// Проверяет определенность значений точки на оси ординат.
        /// </summary>
        public bool IsDefinedY => ValidateValue(m_y);

        /// <summary>
        /// Проверяет определенность значений точки на осях абсцисс и ординат. Значение false означает, что точка еще не
        /// инициализирована.
        /// </summary>
        public bool IsDefined => IsDefinedX && IsDefinedY;

        /// <summary>
        /// Проверяет допустимость значения для <see cref="X"/> и <see cref="Y"/>. Если задать <paramref name="value"/>,
        /// когда метод возвращает false, то соответствующее свойство будет считаться неопределенным.
        /// </summary>
        /// <param name="value">Проверяемое значение</param>
        /// <returns>Возвращает true, если значение допустимо; иначе, false.</returns>
        static public bool ValidateValue(double value)
        {
            return !(double.IsNaN(value) || double.IsPositiveInfinity(value) || double.IsNegativeInfinity(value));
        }

        /// <summary>
        /// Проверяет допустимость строкового представления значения для <see cref="SourceX"/> и <see cref="SourceY"/>.
        /// </summary>
        /// <param name="value">Проверяемое значение</param>
        /// <returns>Возвращает true, если значение допустимо; иначе, false.</returns>
        static public bool ValidateValue(string value)
        {
            var result = double.NegativeInfinity;
            bool success = double.TryParse(value, NumberStyles.Any, App.Culture, out result);
            return success && ValidateValue(result);
        }

        /// <summary>
        /// Получает список ошибок определения свойства.
        /// </summary>
        /// <param name="property">Свойство SourceX, SourceY или null для обоих.</param>
        /// <returns>Возвращает список ошибок.</returns>
        public IEnumerable GetErrors(string property)
        {
            List<string> errors = new List<string>();
            if ((property == null || property == "SourceX") && IsDefinedX == false)
            {
                errors.Add("недопустимое значение");
            }
            if ((property == null || property == "SourceY") && IsDefinedY == false)
            {
                errors.Add("недопустимое значение");
            }
            return errors;
        }
    }
}
