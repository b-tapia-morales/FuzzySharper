using System.Diagnostics.CodeAnalysis;
using Kernel.Function.Abstractions;
using Kernel.Number;
using MathNet.Numerics.Integration;
using Shared.Approx;
using Utils.Shape;

namespace Kernel.Function.Extensions;

internal static class ShapeUtils
{
    private const uint DefaultPrecision = DoubleApproxExt.DefaultPrecision;

    extension(IMembershipFunction function)
    {
        public double CalculateArea(ImplicationMethod method,
            FuzzyNumber implicationParam, double x0, double x1, uint precision = DefaultPrecision) =>
            // A = ∫ f(x) dx
            function.CalculateIntegral(IntegralType.Area, method, implicationParam, x0, x1, precision);

        public double CalculateFirstMomentAt(Axis axis, ImplicationMethod method,
            FuzzyNumber implicationParam, double x0, double x1, uint precision = DefaultPrecision)
        {
            // Mx = (1 / 2) ⋅ ∫ [f(x)]² dx
            if (axis == Axis.X)
                return (1 / 2.0) * function.CalculateIntegral(IntegralType.MomentumX, method, implicationParam, x0, x1, precision);
            // My = ∫ x ⋅ f(x) dx
            return function.CalculateIntegral(IntegralType.MomentumY, method, implicationParam, x0, x1, precision);
        }

        public double CalculateSecondMomentAt(Axis firstAxis, Axis secondAxis, ImplicationMethod method,
            FuzzyNumber implicationParam, double x0, double x1, uint precision = DefaultPrecision) =>
            (firstAxis, secondAxis) switch
            {
                // Mxx = (1 / 3) ⋅ ∫ [f(x)]³ dx
                (Axis.X, Axis.X) => (1 / 3.0) * function.CalculateIntegral(IntegralType.MomentumXX, method, implicationParam, x0, x1, precision),
                // Myy = ∫ x² ⋅ f(x) dx
                (Axis.Y, Axis.Y) => function.CalculateIntegral(IntegralType.MomentumYY, method, implicationParam, x0, x1, precision),
                // Mxy = (1 / 2) ⋅ ∫ x ⋅ [f(x)]² dx
                _ => (1 / 2.0) * function.CalculateIntegral(IntegralType.MomentumXY, method, implicationParam, x0, x1, precision)
            };

        private double CalculateIntegral(IntegralType type, ImplicationMethod method,
            FuzzyNumber implicationParam, double x0, double x1, uint precision = DefaultPrecision)
        {
            if (implicationParam == FuzzyNumber.Min)
                return 0;

            var (xMin, xMax) = function.EffectiveSupport.ToTuple();
            var xi = Math.Max(x0, xMin);
            var xj = Math.Min(x1, xMax);
            var param = Math.Min(implicationParam.Value, function.UMax);
            var errorMargin = Math.Pow(10, -precision);
            var func = function.ImplicationFunction(param, method);
            return NewtonCotesTrapeziumRule.IntegrateAdaptive(IntegralFunction(func, type), xi, xj, errorMargin);
        }

        private Func<double, double> ImplicationFunction(FuzzyNumber implicationParam, ImplicationMethod method) => method switch
        {
            ImplicationMethod.Mamdani => function.MamdaniMinimum(implicationParam),
            ImplicationMethod.Larsen => function.LarsenProduct(implicationParam),
            _ => throw new NotImplementedException()
        };
    }

    private static Func<double, double> IntegralFunction(Func<double, double> f, IntegralType type) =>
        type switch
        {
            IntegralType.Area => f,
            // [f(x)]²
            IntegralType.MomentumX => x => Math.Pow(f(x), 2),
            // x ⋅ f(x)
            IntegralType.MomentumY => x => x * f(x),
            // [f(x)]³
            IntegralType.MomentumXX => x => Math.Pow(f(x), 3),
            // x² ⋅ f(x)
            IntegralType.MomentumXY => x => Math.Pow(x, 2) * f(x),
            // x ⋅ [f(x)]²
            IntegralType.MomentumYY => x => x * Math.Pow(f(x), 2),
            _ => throw new NotImplementedException()
        };
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal enum IntegralType
{
    Area,
    MomentumX,
    MomentumY,
    MomentumXX,
    MomentumXY,
    MomentumYY
}