using ScottPlot;

namespace Console_Project
{
    public class Lab5
    {
        static public void Calculate(double[] x, double[] y)
        {
            if (x.Length != y.Length)
                throw new IndexOutOfRangeException($"{nameof(x)} length != {nameof(y)} length");

            Plot plt = new();
            plt.XLabel("X");
            plt.YLabel("Y");
            plt.Title("Pre-correlation field");

            double funY11(int i = 0) => 2 * x[i] * x[^1] / (x[i] - x[^1]);
            var yRes0 = funY11();
            var diffs = x.Select(x => Math.Abs(x - yRes0)).ToArray();
            // TODO: test this
            var minIndex = Array.IndexOf(diffs, diffs.Min());
            var beforeMin = diffs[..minIndex];
            var withoutMin = beforeMin.Concat(diffs.Skip(beforeMin.Length).ToArray());
            var yRes2Index = Array.IndexOf(diffs, withoutMin.Min());

            // BUG: returns NaN
            double funY112(int i1, int i2, double value) =>
                y[i2] + (y[i1] - y[i2]) / (x[i1] - x[i2]) * (value - x[i2]);
            var yError1 = Math.Abs(funY112(minIndex, yRes2Index, yRes0) - ((y[0] + y[^1]) / 2));

            plt.AddScatter(x, y);
            plt.SaveFig("Pre-correlation field.png");
        }
    }
}
