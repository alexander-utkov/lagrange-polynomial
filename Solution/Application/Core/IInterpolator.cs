using AngouriMath;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;

namespace NumericalMethods.Core
{
    /// <summary>Интерфейс интерполяции многочленами.</summary>
    public interface IInterpolator
    {
        IReadOnlyCollection<double> DataX { get; }
        IReadOnlyCollection<double> DataY { get; }

        /// <summary>
        /// Получает многочлен, который решает задачу интерполяции в заданных моделью условиях.
        /// </summary>
        Entity Polynomial { get; }

        /// <summary>
        /// Возвращает ординату <see cref="Polynom"/> для входной абсциссы.
        /// </summary>
        Func<double, double> Interpolate { get; }

        /// <summary>
        /// Получает последовательность действий для нахождения многочлена <see cref="Polynom"/>.
        /// </summary>
        IList<ActionCollection> Solution { get; }
    }
}
