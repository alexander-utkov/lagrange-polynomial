namespace NumericalMethods.Core
{
    /// <summary>Интерфейс интерполяции многочленами.</summary>
    public interface IInterpolator
    {
        /// <summary>
        /// Получает многочлен, который решает задачу интерполяции в заданных моделью условиях.
        /// </summary>
        string Polynom { get; }

        /// <summary>
        /// Получает последовательность действий для нахождения многочлена <see cref="Polynom"/> в формате LaTeX.
        /// </summary>
        string Solution { get; }

        /// <summary>
        /// Получает или задает информацию об графике <see cref="Polynom"/>.
        /// </summary>
        PlotInfo Plot { get; set; }

        /// <summary>Вычисляет значение многочлена.</summary>
        /// <param name="x">Аргумент <see cref="Polynom"/>.</param>
        /// <returns>значение многочлена <see cref="Polynom"/> в точке <paramref name="x"/>.</returns>
        double GetValue(double x);
    }
}
