using System;
using System.Collections.Generic;
using System.Linq;
using AngouriMath;
using AngouriMath.Core;
using AngouriMath.Extensions;

namespace NumericalMethods.Core
{
    /// <summary>
    /// Модель интерполяционного многочлена Лагранжа.
    /// 
    /// TODO: Вынести в проект ядра приложения и сделать проект модульного тестирования.
    /// </summary>
    public class LagrangeInterpolator : IInterpolator
    {
        /// <summary>Создает экземпляр без возможности изменения точек.</summary>
        /// <param name="data_x">Список абсцисс точек.</param>
        /// <param name="data_y">Список ординат точек.</param>
        /// <exception cref="ArgumentException">Входные списки несоразмерны.</exception>
        public LagrangeInterpolator(IList<double> data_x, IList<double> data_y)
        {
            if (data_x.Count != data_y.Count)
            {
                // TODO: Вынести сообщение в словарь ресурсов.
                throw new ArgumentException("Параметры data_x и data_y не могут иметь разное количество элементов.");
            }

            string formula, expression;
            string separator = ";\\\\";

            // Записываем в решение формулу интерполяционного многочлена Лагранжа
            formula = "P_n(x)\\approx\\sum_{i=0}^{n}{\\frac{y_i\\varphi(x)}{{\\varphi}'(x_i)(x-x_i)}}";
            Solution = formula + separator;

            // Определяем максимальное включительное значение индекса из формулы выше
            var n = data_x.Count - 1;

            // Записываем в решение формулу вспомогательной функции (phi) и ее саму
            formula = expression = "";
            for (int index = 0; index <= n; index++)
            {
                string i = index.ToString();
                formula = formula + $"(x-x_{i})";

                string value = data_x[index].ToString().Replace(",", "."); // FIXME 1*: Локализация (ниже).
                expression = expression + $"(x-{value})";
            }
            Solution = Solution + "\\varphi(x)=" + formula + separator;

            // FIXME 1: AngouriMath выдает исключение (предположительно) при попытке упрощения "x--1,5" из-за запятой.
            Entity aux = expression.Simplify();
            Solution = Solution + "\\varphi(x)=" + aux.Latexise() + separator;

            // Определяем и записываем в решение производную вспомогательной функции
            Entity aux_der = aux.Differentiate("x").Simplify();
            Solution = Solution + "{\\varphi}'(x)=" + aux_der.Latexise() + separator;

            // Вычисляем и записываем в решение значения функции aux_der в точках data_x
            List<double> data_aux_der = new List<double>();
            for (int index = 0; index <= n; index++)
            {
                var x = data_x[index];
                var result = aux_der.Substitute("x", x).EvalNumerical();
                data_aux_der.Add((double)result);
                Solution = Solution + "{\\varphi}'(x_" + index.ToString() + ")={\\varphi}'(" + x.ToString() + ")=" +
                           result.Latexise() + separator;
            }

            // Определяем итоговый многочлен
            List<string> operands = new List<string>();
            for (int index = 0; index <= n; index++)
            {
                string y_i = data_y[index].ToString();
                string numerator = $"{y_i}*({aux})";

                string x_i = data_x[index].ToString();
                string aux_der_i = data_aux_der[index].ToString();
                string denominator = $"{aux_der_i}*(x-{x_i})";

                string operand = $"({numerator})/({denominator})";
                operands.Add(operand);
            }
            expression = String.Join("+", operands).Replace(",", "."); // FIXME 1*: Локализация (выше).
            var polynom = expression.Simplify();

            Solution = Solution + "P(x)=" + expression.Latexise() + "=" + polynom.Latexise();
            Polynom = polynom.ToString();
            Plot = new PlotInfo(data_x.Min() - 1, data_x.Max() + 1);

            m_polynom = polynom.Compile<double, double>("x"); // FIXME: А если переменной не будет в polynom?
        }

        public string Solution { get; private set; }
        public string Polynom { get; private set; }
        public PlotInfo Plot { get; set; }

        protected Func<double, double> m_polynom;

        public double GetValue(double x)
        {
            return m_polynom(x);
        }
    }
}
