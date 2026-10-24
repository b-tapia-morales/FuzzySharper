using Kernel.Function.Abstractions;
using Kernel.Function.Extensions;
using Kernel.Function.Extensions.Metric;
using Kernel.Function.Implementations;
using Kernel.Function.Implication.Factory;
using Shared.Approx;
using Utils.Shape;

namespace Kernel.Tests.Function;

public class MomentTests
{
    [Theory]
    [InlineData(MetricType.Area, 5)]
    [InlineData(MetricType.MomentX, 25)]
    [InlineData(MetricType.MomentY, 5.0 / 3)]
    [InlineData(MetricType.MomentXx, 875.0 / 6)]
    [InlineData(MetricType.MomentXy, 25.0 / 3)]
    [InlineData(MetricType.MomentYy, 5.0 / 6)]
    [InlineData(MetricType.CentroidX, 5)]
    [InlineData(MetricType.CentroidY, 1.0 / 3)]
    public void TriangleMetrics_PolygonAndIntegral_ReturnExactValues(MetricType metric, double expected)
    {
        var function = (MeasurableFunction)TriangleFunction.Create("Triangle", 0, 5, 10);
        List<(double X, double Y)> vertices = [(0, 0), (5, 1), (10, 0)];

        Assert.Equal(expected, metric.CalculateMetric(function, 0, 10, 10), 8);
        Assert.Equal(expected, metric.CalculateMetric(vertices), 8);
        Assert.Equal(expected, metric.GetOriginalMetric(function), 8);
    }

    [Fact]
    public void TriangleVariances_ReturnExactCoordinateVariances()
    {
        var function = (MeasurableFunction)TriangleFunction.Create("Triangle", 0, 5, 10);

        Assert.Equal(25.0 / 6, function.VarianceX(MetricScope.Original), 8);
        Assert.Equal(1.0 / 18, function.VarianceY(MetricScope.Original), 8);
        Assert.Equal(0, function.Covariance(MetricScope.Original), 8);
    }

    [Theory]
    [InlineData(0, 1, 1)]
    [InlineData(7, 2, 0.4)]
    [InlineData(-3, 0.5, 0.8)]
    public void GaussianMetrics_AnalyticValuesMatchIntegration(double center, double sigma, double height)
    {
        var function = (MeasurableFunction)GaussianFunction.Create("Gaussian", center, sigma, height);

        foreach (var metric in Enum.GetValues<MetricType>())
        {
            var integrated = metric.CalculateMetric(function, function.EffectiveSupportLeft, function.EffectiveSupportRight, 10);

            Assert.Equal(integrated, metric.GetOriginalMetric(function), 8);
        }

        Assert.Equal(center, function.CentroidX(MetricScope.Original), 8);
        Assert.Equal(0, function.Covariance(MetricScope.Original), 8);
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(-1, 3, 0.4)]
    [InlineData(2, -2, 0.8)]
    public void LogisticMetrics_MatchTransformedReferenceValues(double steepness, double center, double height)
    {
        const double area = 8.516993171413570;
        const double momentX = 34.628459472107970;
        const double momentY = 3.758696585706785;
        const double momentXx = 205.938546130592663;
        const double momentXy = 17.314229736053985;
        const double momentYy = 2.339197723804523;
        var scale = height / Math.Abs(steepness);
        var expected = new (MetricType Metric, double Value)[]
        {
            (MetricType.Area, scale * area),
            (MetricType.MomentX, scale * (center * area + momentX / steepness)),
            (MetricType.MomentY, scale * height * momentY),
            (MetricType.MomentXx, scale * (center * center * area + 2 * center * momentX / steepness + momentXx / (steepness * steepness))),
            (MetricType.MomentXy, scale * height * (center * momentY + momentXy / steepness)),
            (MetricType.MomentYy, scale * height * height * momentYy)
        };
        var function = (MeasurableFunction)LogisticFunction.Create("Logistic", steepness, center, height);

        foreach (var (metric, value) in expected)
            Assert.InRange(Math.Abs(metric.GetOriginalMetric(function) - value), 0, 1e-4);
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(-1, 3, 0.4)]
    [InlineData(2, -2, 0.8)]
    public void LogisticAnalyticMetrics_MatchTransformedReferenceValues(double steepness, double center, double height)
    {
        var function = (MeasurableFunction)LogisticFunction.Create("Logistic", steepness, center, height);
        var widthScale = 1 / Math.Abs(steepness);

        Assert.Equal(8.516993171413570 * height * widthScale, function.Area(MetricScope.Original), 10);
        Assert.Equal(3.758696585706785 * height * height * widthScale, function.MomentY(MetricScope.Original), 10);
        Assert.Equal(2.339197723804523 * height * height * height * widthScale, function.MomentYy(MetricScope.Original), 10);
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(-1, 3, 0.4)]
    public void LogisticSupport_EndpointsHaveExpectedRelativeMembership(double steepness, double center, double height)
    {
        var function = LogisticFunction.Create("Logistic", steepness, center, height);
        var tailFraction = 2 * DoubleApproxExt.DefaultTolerance;
        var leftHeight = steepness > 0 ? tailFraction : 1 - tailFraction;

        Assert.Equal(leftHeight * height, function.PureFunction(function.EffectiveSupportLeft), 10);
        Assert.Equal((1 - leftHeight) * height, function.PureFunction(function.EffectiveSupportRight), 10);
        Assert.False(function.IsClippedByUoD);
    }

    [Fact]
    public void MomentX_RequestedPrecisionPreservesPartialGaussianIntegral()
    {
        var function = (MeasurableFunction)GaussianFunction.Create("Gaussian", 0, 1);
        var expected = Math.Exp(-8) - Math.Exp(-0.3 * 0.3 / 2);

        var actual = MetricType.MomentX.CalculateMetric(function, -4, 0.3, 10);

        Assert.Equal(expected, actual, 10);
    }

    [Theory]
    [InlineData(Axis.X)]
    [InlineData(Axis.Y)]
    public void MamdaniGaussian_MomentOverAreaMatchesDirectCentroid(Axis axis)
    {
        var function = GaussianFunction.Create("Gaussian", 3, 2);
        var area = function.CalculateArea(ImplicationMethod.Mamdani, 0.5, precision: 10);
        var firstMoment = function.CalculateFirstMoment(axis, ImplicationMethod.Mamdani, 0.5, precision: 10);

        var centroid = function.CalculateCentroid(axis, ImplicationMethod.Mamdani, 0.5, precision: 10);

        Assert.Equal(centroid, firstMoment / area, 8);
    }
}
