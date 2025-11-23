using Kernel.Function.Abstractions;
using Kernel.Function.Implementations;
using Kernel.Number;
using Shared.Approx;
using Utils.Shape;

namespace Kernel.Function.Extensions;

public static class ShapeGeometry
{
    private const uint DefaultPrecision = DoubleApproxExt.DefaultPrecision;

    /// <param name="function">The membership function.</param>
    extension(IMembershipFunction function)
    {
        public double CalculateArea(uint precision = DefaultPrecision) =>
            function.CalculateArea(ImplicationMethod.Larsen, FuzzyNumber.Max, precision);

        /// <summary>
        /// Calculates the area of a membership function after applying the specified
        /// implication method.
        /// </summary>
        /// <param name="method"> The implication method to apply while computing the area.</param>
        /// <param name="implicationParam">The parameter used by the implication method.</param>
        /// <param name="precision">
        ///     The number of decimal places <i>d</i> that determines the expected accuracy of the area calculation <i>ε = 10<sup>‑d</sup></i>;
        ///     defaults to <see cref="DefaultPrecision"/>.
        /// </param>
        /// <returns>The computed area as a <c>double</c>.</returns>
        public double CalculateArea(ImplicationMethod method,
            FuzzyNumber implicationParam, uint precision = DefaultPrecision)
        {
            var (x0, x1) = function.EffectiveSupport.ToTuple();
            return function.CalculateAreaAt(method, implicationParam, x0, x1, precision);
        }

        /// <summary>
        /// Calculates the area of a membership function after applying the specified
        /// implication method and making two vertical cuts at the points <paramref name="x0"/> 
        /// and <paramref name="x1"/>.
        /// </summary>
        /// <param name="method">
        ///     The implication method to apply while computing the area.
        /// </param>
        /// <param name="implicationParam">
        ///     The parameter used by the implication method.
        /// </param>
        /// <param name="x0">The x-coordinate for the first vertical cut (lower support bound).</param>
        /// <param name="x1">The x-coordinate for the second vertical cut (upper support bound).</param>
        /// <param name="precision">
        ///     The number of decimal places <i>d</i> that determines the expected accuracy of the area calculation <i>ε = 10<sup>‑d</sup></i>;
        ///     defaults to <see cref="DefaultPrecision"/>.
        /// </param>
        /// <returns>The computed area as a <c>double</c>.</returns>
        public double CalculateAreaAt(ImplicationMethod method,
            FuzzyNumber implicationParam, double x0, double x1, uint precision = DefaultPrecision)
        {
            if (implicationParam == FuzzyNumber.Min)
                return 0;

            // If the implication parameter is greater or equal to the function's maximum truth value μMax, the new weight will be α = μMax.
            // No alpha cut or scaling down is performed.
            var param = Math.Min(implicationParam.Value, function.UMax);
            // We keep track of whether an alpha cut or a scaling down needs to be performed.
            var noCutNeeded = param.RoughlyEquals(function.UMax);

            return function switch
            {
                // Case 1: the triangle either remains as is if α >= UMax, or it is scaled down by α if α < UMax.
                // The triangle does not transform into a trapezoid, nor any of its original x-coordinates change.
                TriangleFunction tri when method == ImplicationMethod.Larsen || noCutNeeded =>
                    TriangleUtils.CalculateSideCutArea(tri.A, tri.B, tri.C, param, x0, x1),
                // Case 2: the trapezoid either remains as is if α >= UMax, or it is scaled down by α if α < UMax.
                // The trapezoid's original x-coordinates remain unchanged.
                TrapezoidFunction tra when method == ImplicationMethod.Larsen || noCutNeeded =>
                    TrapezoidUtils.CalculateSideCutArea(tra.A, tra.B, tra.C, tra.D, param, x0, x1),
                // Case 3: A horizontal cut is performed at α. If the shape is a triangle, it will transform into a trapezoid; if it is a trapezoid,
                // the x-coordinates for its upper base will change. In both cases, the new upper base's x-coordinates will be
                // the leftmost and rightmost x-coordinates xᵢ and xⱼ at which μ(x) = α.
                LinearPiecewiseFunction lp when method == ImplicationMethod.Mamdani => lp.CalculateAlphaCutArea(param, x0, x1),
                _ => ShapeUtils.CalculateArea(function, method, param, x0, x1, precision)
            };
        }

        public double CalculateFirstMoment(Axis axis, uint precision = DefaultPrecision) =>
            function.CalculateFirstMoment(axis, ImplicationMethod.Larsen, FuzzyNumber.Max, precision);

        public double CalculateFirstMoment(Axis axis, ImplicationMethod method,
            FuzzyNumber implicationParam, uint precision = DefaultPrecision)
        {
            var (x0, x1) = function.EffectiveSupport.ToTuple();
            return CalculateFirstMomentAt(function, axis, method, implicationParam, x0, x1, precision);
        }

        public double CalculateFirstMomentAt(Axis axis, ImplicationMethod method,
            FuzzyNumber implicationParam, double x0, double x1, uint precision = DefaultPrecision)
        {
            if (implicationParam == FuzzyNumber.Min)
                return 0;

            var param = Math.Min(implicationParam.Value, function.UMax);
            var vertices = function.BuildVertices(method, param, x0, x1);
            return function is LinearPiecewiseFunction ? PolygonUtils.CalculateFirstMoment(vertices, axis) : ShapeUtils.CalculateFirstMomentAt(function, axis, method, param, x0, x1, precision);
        }

        public double CalculateSecondMoment(Axis firstAxis, Axis secondAxis, uint precision = DefaultPrecision) =>
            function.CalculateSecondMoment(firstAxis, secondAxis, ImplicationMethod.Larsen, FuzzyNumber.Max, precision);

        public double CalculateSecondMoment(Axis firstAxis, Axis secondAxis, ImplicationMethod method,
            FuzzyNumber implicationParam, uint precision = DefaultPrecision)
        {
            var (x0, x1) = function.EffectiveSupport.ToTuple();
            return CalculateSecondMomentAt(function, firstAxis, secondAxis, method, implicationParam, x0, x1, precision);
        }

        public double CalculateSecondMomentAt(Axis firstAxis, Axis secondAxis, ImplicationMethod method,
            FuzzyNumber implicationParam, double x0, double x1, uint precision = DefaultPrecision)
        {
            if (implicationParam == FuzzyNumber.Min)
                return 0;

            var param = Math.Min(implicationParam.Value, function.UMax);
            var vertices = function.BuildVertices(method, param, x0, x1);
            return function is LinearPiecewiseFunction
                ? PolygonUtils.CalculateSecondMoment(vertices, firstAxis, secondAxis)
                : ShapeUtils.CalculateSecondMomentAt(function, firstAxis, secondAxis, method, param, x0, x1, precision);
        }

        public double CalculateCentroid(Axis axis, ImplicationMethod method,
            FuzzyNumber implicationParam, uint precision = DefaultPrecision)
        {
            var (x0, x1) = function.EffectiveSupport.ToTuple();
            return CalculateCentroidAt(function, axis, method, implicationParam, x0, x1, precision);
        }

        public double CalculateCentroidAt(Axis axis, ImplicationMethod method,
            FuzzyNumber implicationParam, double x0, double x1, uint precision = DefaultPrecision)
        {
            var area = function.CalculateAreaAt(method, implicationParam, x0, x1, precision);
            if (area.IsRoughlyZero())
                throw new ArgumentException("The area of the shape is roughly zero");
            var momentum = CalculateFirstMomentAt(function, axis, method, implicationParam, x0, x1, precision);
            return momentum / area;
        }

        private List<(double X, double Y)> BuildVertices(ImplicationMethod method, double alpha, double x0, double x1)
        {
            // We keep track of whether an alpha cut or a scaling down needs to be performed.
            var noCutNeeded = alpha.RoughlyEquals(function.UMax);
            // If the function is a polygon, we need to build a list of vertices resulting from applying the vertical cuts at points
            // x0 and x1 and the horizontal alpha-cut at point α (only if the implication method is Mamdani).
            var vertices = function switch
            {
                TriangleFunction tri when method == ImplicationMethod.Larsen || noCutNeeded => TriangleUtils.ToVertices(tri.A, tri.B, tri.C, alpha, x0, x1),
                TrapezoidFunction tra when method == ImplicationMethod.Larsen || noCutNeeded => TrapezoidUtils.ToVertices(tra.A, tra.B, tra.C, tra.D, alpha, x0, x1),
                LinearPiecewiseFunction lp when method == ImplicationMethod.Mamdani => lp.GetAlphaCutVertices(alpha, x0, x1),
                _ => []
            };

            if (vertices.Count > 0)
                PolygonUtils.OrderCounterClockwise(vertices);

            return vertices;
        }
    }

    extension(LinearPiecewiseFunction function)
    {
        private double CalculateAlphaCutArea(double alpha, double x0, double x1)
        {
            var (ai, aj) = function.AlphaCutClipped(alpha).Get.ToTuple();
            return TrapezoidUtils.CalculateSideCutArea(function.LeftEdge, ai, aj, function.RightEdge, alpha, x0, x1);
        }

        private List<(double X, double Y)> GetAlphaCutVertices(double alpha, double x0, double x1)
        {
            var (ai, aj) = function.AlphaCutClipped(alpha).Get.ToTuple();
            return TrapezoidUtils.ToVertices(function.LeftEdge, ai, aj, function.RightEdge, alpha, x0, x1);
        }
    }
}