using System;

namespace NumericalMethods
{
    /// <summary>Интерфейс интерполяции многочленами.</summary>
    internal interface IInterpolator
    {
        /// <summary>
        /// Получает многочлен, который решает задачу интерполяции в заданных моделью условиях.
        /// </summary>
        string Polynom { get; }

        /// <summary>
        /// Получает последовательность действий для нахождения многочлена <see cref="Polynom"/> в формате LaTeX.
        /// </summary>
        string Solution { get; }

        /// <summary>Вычисляет значение многочлена.</summary>
        /// <param name="x">Аргумент <see cref="Polynom"/>.</param>
        /// <returns>значение многочлена <see cref="Polynom"/> в точке <paramref name="x"/>.</returns>
        double GetValue(double x);
    }
}
