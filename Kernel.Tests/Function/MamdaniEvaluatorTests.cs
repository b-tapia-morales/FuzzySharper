using Kernel.Function.Abstractions;
using Kernel.Function.Extensions;
using Kernel.Function.Extensions.Metric;
using Kernel.Function.Implementations;
using Kernel.Function.Implication.Factory;
using Kernel.Function.Implication.Mamdani;
using MathNet.Numerics;
using Utils.Shape;

namespace Kernel.Tests.Function;

public class MamdaniEvaluatorTests
{
    [Theory]
    [InlineData(0, 5, 10, 0.5, Axis.X, 5)]
    [InlineData(0, 5, 10, 0.5, Axis.Y, 2.0 / 9)]
    [InlineData(0, 13, 25, 0.5, Axis.X, 227.0 / 18)]
    [InlineData(0, 0, 10, 0.5, Axis.X, 35.0 / 9)]
    [InlineData(0, 10, 10, 0.5, Axis.X, 55.0 / 9)]
    [InlineData(0, 5, 10, 1, Axis.X, 5)]
    [InlineData(0, 5, 10, 1, Axis.Y, 1.0 / 3)]
    public void CalculateCentroid_Triangle_ReturnsExpectedCoordinate(
        double a, double b, double c, double height, Axis axis, double expected)
    {
        var function = TriangleFunction.Create("Triangle", a, b, c);

        var actual = function.CalculateCentroid(axis, ImplicationMethod.Mamdani, height);

        Assert.Equal(expected, actual, 8);
    }

    [Theory]
    [InlineData(MetricType.Area, 15.0 / 4)]
    [InlineData(MetricType.MomentX, 75.0 / 4)]
    [InlineData(MetricType.MomentY, 5.0 / 6)]
    [InlineData(MetricType.MomentXx, 3625.0 / 32)]
    [InlineData(MetricType.MomentXy, 25.0 / 6)]
    [InlineData(MetricType.MomentYy, 25.0 / 96)]
    public void Evaluate_ClippedTriangle_ReturnsExpectedAdditiveMetric(MetricType metric, double expected)
    {
        var function = (MeasurableFunction)TriangleFunction.Create("Triangle", 0, 5, 10);
        var evaluator = MamdaniEvaluator.BuildPlan(function, 0.5, true);

        var actual = evaluator.Evaluate(metric, function, 8);

        Assert.Equal(expected, actual, 8);
    }

    [Theory]
    [InlineData(-3, 0.5)]
    [InlineData(7, 2)]
    public void CalculateCentroid_ClippedGaussian_ReturnsCenterOnXAxis(double center, double sigma)
    {
        var function = GaussianFunction.Create("Gaussian", center, sigma);

        var actual = function.CalculateCentroid(Axis.X, ImplicationMethod.Mamdani, 0.5, precision: 10);

        Assert.Equal(center, actual, 8);
    }

    [Fact]
    public void CalculateCentroid_ClippedGaussian_ReturnsAnalyticCoordinateOnYAxis()
    {
        var function = GaussianFunction.Create("Gaussian", 0, 1);
        var cutPoint = Math.Sqrt(2 * Math.Log(2));
        var area = cutPoint + Math.Sqrt(2 * Math.PI) *
            (SpecialFunctions.Erf(4 / Math.Sqrt(2)) - SpecialFunctions.Erf(cutPoint / Math.Sqrt(2)));
        var firstMoment = cutPoint / 4 + Math.Sqrt(Math.PI) / 2 *
            (SpecialFunctions.Erf(4) - SpecialFunctions.Erf(cutPoint));

        var actual = function.CalculateCentroid(Axis.Y, ImplicationMethod.Mamdani, 0.5, precision: 10);

        Assert.Equal(firstMoment / area, actual, 8);
    }
}
