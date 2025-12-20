using Kernel.Function.Abstractions;
using Shared.Approx;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Utils.Shape;

namespace Kernel.Function.Extensions.Metric;

public static class MetricSelector
{
    private const uint DefaultPrecision = DoubleApproxExt.DefaultPrecision;

    extension(MetricType type)
    {
        public double GetOriginalMetric(MeasurableFunction function) => type switch
        {
            MetricType.Area => function.Area(MetricScope.Original),
            MetricType.MomentX => function.MomentX(MetricScope.Original),
            MetricType.MomentY => function.MomentY(MetricScope.Original),
            MetricType.MomentXx => function.MomentXx(MetricScope.Original),
            MetricType.MomentXy => function.MomentXy(MetricScope.Original),
            MetricType.MomentYy => function.MomentYy(MetricScope.Original),
            MetricType.CentroidX => function.CentroidX(MetricScope.Original),
            MetricType.CentroidY => function.CentroidY(MetricScope.Original),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        public Option<double> GetClippedMetric(MeasurableFunction function)
        {
            if (!function.IsClipped)
                return Option<double>.None();
            return type switch
            {
                MetricType.Area => function.Area(),
                MetricType.MomentX => function.MomentX(),
                MetricType.MomentY => function.MomentY(),
                MetricType.MomentXx => function.MomentXx(),
                MetricType.MomentXy => function.MomentXy(),
                MetricType.MomentYy => function.MomentYy(),
                MetricType.CentroidX => function.CentroidX(),
                MetricType.CentroidY => function.CentroidY(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public double CalculateMetric(MeasurableFunction function, double x0, double x1, uint precision = DefaultPrecision) => type switch
        {
            MetricType.Area => function.CalculateAreaAt(x0, x1, precision),
            MetricType.MomentX => function.CalculateFirstMomentAt(Axis.X, x0, x1),
            MetricType.MomentY => function.CalculateFirstMomentAt(Axis.Y, x0, x1, precision),
            MetricType.MomentXx => function.CalculateSecondMomentAt(Axis.X, Axis.X, x0, x1, precision),
            MetricType.MomentXy => function.CalculateSecondMomentAt(Axis.X, Axis.Y, x0, x1, precision),
            MetricType.MomentYy => function.CalculateSecondMomentAt(Axis.Y, Axis.Y, x0, x1, precision),
            MetricType.CentroidX => function.CalculateCentroidAt(Axis.X, x0, x1, precision),
            MetricType.CentroidY => function.CalculateCentroidAt(Axis.Y, x0, x1, precision),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        public double CalculateMetric(List<(double X, double Y)> vertices) => type switch
        {
            MetricType.Area => PolygonUtils.CalculateArea(vertices),
            MetricType.MomentX => PolygonUtils.CalculateFirstMoment(vertices, Axis.X),
            MetricType.MomentY => PolygonUtils.CalculateFirstMoment(vertices, Axis.Y),
            MetricType.MomentXx => PolygonUtils.CalculateSecondMoment(vertices, Axis.X, Axis.X),
            MetricType.MomentXy => PolygonUtils.CalculateSecondMoment(vertices, Axis.X, Axis.Y),
            MetricType.MomentYy => PolygonUtils.CalculateSecondMoment(vertices, Axis.Y, Axis.Y),
            MetricType.CentroidX => PolygonUtils.CalculateCentroid(vertices, Axis.X),
            MetricType.CentroidY => PolygonUtils.CalculateCentroid(vertices, Axis.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}