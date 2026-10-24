using Kernel.Function.Abstractions;
using Kernel.Function.Extensions.Metric;
using MathNet.Numerics.Integration;
using Shared.Approx;
using Utils.Shape;

namespace Kernel.Function.Extensions;

internal static class ShapeUtils
{
    private const uint DefaultPrecision = DoubleApproxExt.DefaultPrecision;

    extension(IMembershipFunction function)
    {
        public double CalculateAreaAt(double x0, double x1, uint precision = DefaultPrecision) =>
            // A = ∫ f(x) dx
            function.CalculateIntegral(MetricType.Area, x0, x1, precision);

        public double CalculateFirstMomentAt(Axis axis, double x0, double x1, uint precision = DefaultPrecision)
        {
            // Mx = ∫ x ⋅ f(x) dx
            if (axis == Axis.X)
                return function.CalculateIntegral(MetricType.MomentX, x0, x1, precision);
            // My = (1 / 2) ⋅ ∫ [f(x)]² dx
            return function.CalculateIntegral(MetricType.MomentY, x0, x1, precision);
        }

        public double CalculateSecondMomentAt(Axis firstAxis, Axis secondAxis, double x0, double x1, uint precision = DefaultPrecision) =>
            (firstAxis, secondAxis) switch
            {
                // Mxx = ∫ x² ⋅ f(x) dx
                (Axis.X, Axis.X) => function.CalculateIntegral(MetricType.MomentXx, x0, x1, precision),
                // Myy = (1 / 3) ⋅ ∫ [f(x)]³ dx
                (Axis.Y, Axis.Y) => function.CalculateIntegral(MetricType.MomentYy, x0, x1, precision),
                // Mxy = Myx = (1 / 2) ⋅ ∫ x ⋅ [f(x)]² dx
                _ => function.CalculateIntegral(MetricType.MomentXy, x0, x1, precision)
            };
        
        public double CalculateCentroidAt(Axis axis, double x0, double x1, uint precision = DefaultPrecision) =>
            function.CalculateFirstMomentAt(axis, x0, x1, precision) / function.CalculateAreaAt(x0, x1, precision);

        public double CalculateIntegral(MetricType type, double x0, double x1, uint precision = DefaultPrecision)
        {
            var errorMargin = Math.Pow(10, -precision);
            var func = function.PureFunction;
            return GaussKronrodRule.Integrate(IntegralFunction(func, type), x0, x1, out _, out _, errorMargin);
        }
    }

    private static Func<double, double> IntegralFunction(Func<double, double> f, MetricType type) =>
        type switch
        {
            MetricType.Area => f,
            // x ⋅ f(x)
            MetricType.MomentX => x => x * f(x),
            // [f(x)]² / 2
            MetricType.MomentY => x => Math.Pow(f(x), 2) / 2,
            // x² ⋅ f(x)
            MetricType.MomentXx => x => Math.Pow(x, 2) * f(x),
            // x ⋅ [f(x)]² / 2
            MetricType.MomentXy => x => x * Math.Pow(f(x), 2) / 2,
            // [f(x)]³ / 3
            MetricType.MomentYy => x => Math.Pow(f(x), 3) / 3,
            _ => throw new NotImplementedException()
        };
}
