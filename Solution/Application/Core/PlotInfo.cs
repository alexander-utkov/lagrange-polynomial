namespace NumericalMethods.Core
{
    public class PlotInfo
    {
        public PlotInfo(double from, double to, double step = double.NaN)
        {
            From = from;
            To = to;
            Step = double.IsNaN(step) ? (to - from) / 100d : step;
        }

        public readonly double From;
        public readonly double To;
        public readonly double Step;
    }
}
