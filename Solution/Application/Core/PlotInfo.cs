namespace NumericalMethods.Core
{
    public class PlotInfo
    {
        public PlotInfo(double a, double b, double step = double.NaN)
        {
            A = a;
            B = b;

            if (A > B)
            {
                double c = A;
                A = B;
                B = c;
            }

            Step = double.IsNaN(step) ? (b - a) / 100d : step;

            // FIXME: double.IsNegativeInfinity, double.IsPositiveInfinity.
        }

        public double A { get; }
        public double B { get; }
        public double Step { get; }
    }
}
