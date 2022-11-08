using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AngouriMath;
using AngouriMath.Extensions;
using Antlr4.Runtime.Misc;

namespace NumericalMethods.Core
{
    /// <summary>
    /// Модель интерполяционного многочлена Лагранжа.
    /// </summary>
    public sealed class LagrangeInterpolator : IInterpolator
    {
        /// <summary>Создает экземпляр без возможности изменения точек.</summary>
        /// <param name="data_x">Список абсцисс точек.</param>
        /// <param name="data_y">Список ординат точек.</param>
        /// <exception cref="ArgumentException">Входные списки несоразмерны или пусты.</exception>
        public LagrangeInterpolator(IReadOnlyCollection<double> data_x, IReadOnlyCollection<double> data_y)
        {
            if (data_x.Count != data_y.Count)
            {
                throw new ArgumentException("Параметры data_x и data_y не могут иметь разное количество элементов.");
            }
            if (data_x.Count == 0)
            {
                throw new ArgumentException("Определите не менее одной точки: многочлен нулевого порядка невозможен.");
            }

            DataX = data_x;
            DataY = data_y;
            
            Solution = new List<ActionCollection>();

            OverviewAction();
            var functions = AuxiliaryAction();
            var aux = functions.a;
            var aux_der = functions.b;
            var data_aux_der = PreparationsAction(aux_der);
            Polynomial = SubstitutionAction(aux, data_aux_der);
            Interpolate = Polynomial.Compile<double, double>("x");
        }
        
        public IReadOnlyCollection<double> DataX { get; private set; }
        public IReadOnlyCollection<double> DataY { get; private set; }

        /// <summary>
        /// Получает интерполяционных многочлен Лагранжа для данных узлов <see cref="DataX"/> и <see cref="DataY"/>.
        /// </summary>
        public Entity Polynomial { get; private set; }

        public Func<double, double> Interpolate { get; private set; }

        public IList<ActionCollection> Solution { get; private set; }

        /// <summary>
        /// Формат преобразования чисел в строки для AngouriMath (его парсер использует точку как разделитель).
        /// </summary>
        private static CultureInfo m_culture = new CultureInfo("en-US");

        /// <summary>
        /// Максимальный индекс узлов интерполяции или же максимальный порядок многочлена.
        /// </summary>
        private int N => DataX.Count - 1;

        private static App App => App.Current as App;

        private void OverviewAction()
        {
            ActionCollection overview = new ActionCollection("overview");
            overview.Add(new Action("formula.polynomial"));
            overview.Add(new Action("definition.n", DataX.Count.ToString(App.Culture), N.ToString(App.Culture)));
            Solution.Add(overview);
        }

        private Pair<Entity, Entity> AuxiliaryAction()
        {
            string aux_raw = "";
            string aux_formula = "";
            for (int index = 0; index <= N; index++)
            {
                string value = DataX.ElementAt(index).ToString(m_culture);
                aux_raw += $"(x-{value})";
                aux_formula += $"(x-x_{index})";
            }

            Entity aux = aux_raw.Simplify();
            Entity aux_der = aux.Differentiate("x").Simplify();

            ActionCollection auxiliary = new ActionCollection("auxiliary");
            auxiliary.Add(new Action("formula.auxiliary", aux_formula));
            auxiliary.Add(new Action("definition.auxiliary", aux.Latexise()));
            auxiliary.Add(new Action("derivative.auxiliary", aux_der.Latexise()));
            Solution.Add(auxiliary);

            return new Pair<Entity, Entity>(aux, aux_der);
        }

        private IList<double> PreparationsAction(Entity aux_der)
        {
            List<double> data_aux_der = new List<double>();
            Func<double, double> aux_der_compiled = aux_der.Compile<double, double>("x");
            List<Action> expressions = new List<Action>();

            for (int index = 0; index <= N; index++)
            {
                double x = DataX.ElementAt(index);
                double y = aux_der_compiled(x);
                data_aux_der.Add(y);

                var str_index = index.ToString();
                var str_x = x.ToString(App.Culture);
                var str_y = y.ToString(App.Culture);
                var expression = new Action("derivative.auxiliary.calculate", str_index, str_x, str_y);
                expressions.Add(expression);
            }

            // TODO: Сделать expression не строками, а вложенными action'ами.

            ActionCollection preparations = new ActionCollection("preparations");
            preparations.Add(new Action("derivative.auxiliary.ordinates", expressions.ToArray()));
            Solution.Add(preparations);

            return data_aux_der;
        }

        private Entity SubstitutionAction(Entity aux, IList<double> data_aux_der)
        {
            string pol_raw = "";
            for (int index = 0; index <= N; index++)
            {
                string y_i = DataY.ElementAt(index).ToString(m_culture);
                string numerator = $"{y_i}*({aux})";

                string x_i = DataX.ElementAt(index).ToString(m_culture);
                string aux_der_i = data_aux_der[index].ToString(m_culture);
                string denominator = $"{aux_der_i}*(x-{x_i})";

                string operand = $"({numerator})/({denominator})";
                if (index != 0)
                {
                    pol_raw += "+";
                }
                pol_raw += operand;
            }

            Entity pol = pol_raw.Simplify();

            ActionCollection substitution = new ActionCollection("substitution");
            substitution.Add(new Action("formula.polynomial.substitution", pol_raw.Latexise(), pol.Latexise()));
            Solution.Add(substitution);

            return pol;
        }
    }
}
