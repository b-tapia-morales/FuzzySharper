using System.Diagnostics;
using Kernel.Function.Abstractions;
using Kernel.Function.Extensions.Metric;
using Kernel.Function.Implementations;
using Kernel.Function.Implication.Factory;
using Kernel.Number;
using Shared.Approx;
using Shared.Options.Factory;
using Utils.Shape;

namespace Kernel.Function.Extensions;

public static class ShapeGeometry
{
    private const uint DefaultPrecision = DoubleApproxExt.DefaultPrecision;
    private const string UnknownMetricMessage = "Can't operate metric on function of unknown type.";

    extension(IMembershipFunction function)
    {
        public double CalculateArea(bool useUoD = true) =>
            function switch
            {
                UnilateralFunction unilateral => unilateral.CalculateMetric(MetricType.Area, useUoD),
                MeasurableFunction measurable => measurable.Area(useUoD ? MetricScope.Clipped : MetricScope.Original),
                _ => throw new InvalidOperationException(UnknownMetricMessage)
            };

        public double CalculateArea(ImplicationMethod method, FuzzyNumber implicationParam, bool useUoD = true,
            uint precision = DefaultPrecision)
        {
            if (implicationParam == FuzzyNumber.Min)
                return 0;

            if (function is UnilateralFunction unilateral)
                return unilateral.CalculateMetric(MetricType.Area, method, implicationParam, useUoD);

            Debug.Assert(function is MeasurableFunction);
            var measurable = (MeasurableFunction) function;
            var evaluator = ImplicationFactory.GetInstance(method, measurable, implicationParam, useUoD);
            return evaluator.Evaluate(MetricType.Area, measurable, precision);
        }

        public double CalculateFirstMoment(Axis axis, bool useUoD = true)
        {
            var firstMomentMetric = MetricType.ResolveFirstMoment(axis);
            return function switch
            {
                UnilateralFunction unilateral => unilateral.CalculateMetric(firstMomentMetric, useUoD),
                MeasurableFunction measurable => measurable.ResolveFirstMoment(axis, useUoD),
                _ => throw new InvalidOperationException(UnknownMetricMessage)
            };
        }

        public double CalculateFirstMoment(Axis axis, ImplicationMethod method, FuzzyNumber implicationParam,
            bool useUoD = true, uint precision = DefaultPrecision)
        {
            if (implicationParam == FuzzyNumber.Min)
                return 0;

            var firstMomentMetric = MetricType.ResolveFirstMoment(axis);
            if (function is UnilateralFunction unilateral)
                return unilateral.CalculateMetric(firstMomentMetric, method, implicationParam, useUoD);

            Debug.Assert(function is MeasurableFunction);
            var measurable = (MeasurableFunction) function;
            var evaluator = ImplicationFactory.GetInstance(method, measurable, implicationParam, useUoD);
            return evaluator.Evaluate(firstMomentMetric, measurable, precision);
        }

        public double CalculateSecondMoment(Axis firstAxis, Axis secondAxis, bool useUoD = true)
        {
            var secondMomentMetric = MetricType.ResolveSecondMoment(firstAxis, secondAxis);
            return function switch
            {
                UnilateralFunction unilateral => unilateral.CalculateMetric(secondMomentMetric, useUoD),
                MeasurableFunction measurable => measurable.ResolveSecondMoment(firstAxis, secondAxis, useUoD),
                _ => throw new InvalidOperationException(UnknownMetricMessage)
            };
        }

        public double CalculateSecondMoment(Axis firstAxis, Axis secondAxis, ImplicationMethod method,
            FuzzyNumber implicationParam, bool useUoD = true, uint precision = DefaultPrecision)
        {
            if (implicationParam == FuzzyNumber.Min)
                return 0;

            var secondMomentMetric = MetricType.ResolveSecondMoment(firstAxis, secondAxis);
            if (function is UnilateralFunction unilateral)
                return unilateral.CalculateMetric(secondMomentMetric, method, implicationParam, useUoD);

            Debug.Assert(function is MeasurableFunction);
            var measurable = (MeasurableFunction) function;
            var evaluator = ImplicationFactory.GetInstance(method, measurable, implicationParam, useUoD);
            return evaluator.Evaluate(secondMomentMetric, measurable, precision);
        }

        public double CalculateCentroid(Axis axis, bool useUoD = true)
        {
            var centroidMetric = MetricType.ResolveCentroid(axis);
            return function switch
            {
                UnilateralFunction unilateral => unilateral.CalculateMetric(centroidMetric, useUoD),
                MeasurableFunction measurable => measurable.ResolveCentroid(axis, useUoD),
                _ => throw new InvalidOperationException(UnknownMetricMessage)
            };
        }

        public double CalculateCentroid(Axis axis, ImplicationMethod method,
            FuzzyNumber implicationParam, bool useUoD = true, uint precision = DefaultPrecision)
        {
            if (implicationParam == FuzzyNumber.Min)
                return 0;

            var centroid = MetricType.ResolveCentroid(axis);
            if (function is UnilateralFunction unilateral)
                return unilateral.CalculateMetric(centroid, method, implicationParam, useUoD);

            Debug.Assert(function is MeasurableFunction);
            var measurable = (MeasurableFunction) function;
            var evaluator = ImplicationFactory.GetInstance(method, measurable, implicationParam, useUoD);
            return evaluator.Evaluate(centroid, measurable, precision);
        }
    }

    extension(UnilateralFunction function)
    {
        private double CalculateMetric(MetricType metricType, bool useUoD = true) =>
            function.CalculateMetric(metricType, ImplicationMethod.Larsen, FuzzyNumber.Max, useUoD);

        private double CalculateMetric(MetricType metricType, ImplicationMethod method,
            FuzzyNumber implicationParam, bool useUoD = true)
        {
            Debug.Assert(implicationParam >= FuzzyNumber.Min);
            var (x0, x1) = useUoD ? function.RestrictedSupport.ToTuple() : function.EffectiveSupport.ToTuple();
            var height = method switch
            {
                ImplicationMethod.Mamdani => function.UMax,
                ImplicationMethod.Larsen => function.UMax * implicationParam.Value,
                _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
            };
            var vertices = function switch
            {
                LeftTrapezoidFunction left => TriangleUtils.ToVertices(left.A, left.A, left.B, height, x0, x1),
                RightTrapezoidFunction right => TriangleUtils.ToVertices(right.A, right.B, right.B, height, x0, x1),
                _ => throw new InvalidOperationException(UnknownMetricMessage)
            };

            if (method != ImplicationMethod.Mamdani || implicationParam.Value.IsRoughlyGreaterOrEqualTo(function.UMax))
                return metricType.CalculateMetric(vertices);
            
            var alphaCutVertices = function.AlphaCutVertices(vertices, implicationParam, x0, x1);
            return metricType.CalculateMetric(alphaCutVertices);

        }

        private List<(double X, double)> AlphaCutVertices(List<(double X, double Y)> vertices, FuzzyNumber alpha, double x0, double x1)
        {
            var alphaCutVertices = vertices.Select(v => (v.X, Math.Min(v.Y, alpha))).ToList();

            var cut = function switch
            {
                LeftTrapezoidFunction left => left.AlphaCutRight(alpha),
                RightTrapezoidFunction right => right.AlphaCutLeft(alpha),
                _ => throw new InvalidOperationException(UnknownMetricMessage)
            };

            if (cut.IsSome(out var x) && x0 < x && x < x1)
                alphaCutVertices.Add((x, alpha.Value));

            PolygonUtils.OrderCounterClockwise(alphaCutVertices);
            return alphaCutVertices;
        }
    }

    extension(MeasurableFunction function)
    {
        private double ResolveFirstMoment(Axis axis, bool useUoD = true) => axis switch
        {
            Axis.X => function.MomentX(useUoD ? MetricScope.Clipped : MetricScope.Original),
            Axis.Y => function.MomentY(useUoD ? MetricScope.Clipped : MetricScope.Original),
            _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
        };

        private double ResolveSecondMoment(Axis firstAxis, Axis secondAxis, bool useUoD = true) => (firstAxis, secondAxis) switch
        {
            (Axis.X, Axis.X) => function.MomentXx(useUoD ? MetricScope.Clipped : MetricScope.Original),
            (Axis.X, Axis.Y) or (Axis.Y, Axis.X) => function.MomentXy(useUoD ? MetricScope.Clipped : MetricScope.Original),
            (Axis.Y, Axis.Y) => function.MomentYy(useUoD ? MetricScope.Clipped : MetricScope.Original),
            _ => throw new ArgumentOutOfRangeException(nameof(firstAxis), firstAxis, null)
        };

        private double ResolveCentroid(Axis axis, bool useUoD = true) => axis switch
        {
            Axis.X => function.CentroidX(useUoD ? MetricScope.Clipped : MetricScope.Original),
            Axis.Y => function.CentroidY(useUoD ? MetricScope.Clipped : MetricScope.Original),
            _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
        };
    }

    extension(MetricType)
    {
        private static MetricType ResolveFirstMoment(Axis axis) => axis switch
        {
            Axis.X => MetricType.MomentX,
            Axis.Y => MetricType.MomentY,
            _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
        };

        private static MetricType ResolveSecondMoment(Axis firstAxis, Axis secondAxis) => (firstAxis, secondAxis) switch
        {
            (Axis.X, Axis.X) => MetricType.MomentXx,
            (Axis.X, Axis.Y) or (Axis.Y, Axis.X) => MetricType.MomentXy,
            (Axis.Y, Axis.Y) => MetricType.MomentYy,
            _ => throw new ArgumentOutOfRangeException(nameof(firstAxis), firstAxis, null)
        };

        private static MetricType ResolveCentroid(Axis axis) => axis switch
        {
            Axis.X => MetricType.CentroidX,
            Axis.Y => MetricType.CentroidY,
            _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
        };
    }
}