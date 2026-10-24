using Kernel.Function.Abstractions;
using Kernel.Function.Extensions.Metric;
using Kernel.Function.Implementations;
using Kernel.Function.Implication.Larsen;

namespace Kernel.Tests.Function;

public class LarsenEvaluatorTests
{
    [Theory]
    [InlineData(MetricType.Area, 2.5)]
    [InlineData(MetricType.MomentX, 12.5)]
    [InlineData(MetricType.MomentY, 5.0 / 12)]
    [InlineData(MetricType.MomentXx, 875.0 / 12)]
    [InlineData(MetricType.MomentXy, 25.0 / 12)]
    [InlineData(MetricType.MomentYy, 5.0 / 48)]
    [InlineData(MetricType.CentroidX, 5)]
    [InlineData(MetricType.CentroidY, 1.0 / 6)]
    public void Evaluate_ScaledTriangle_ReturnsExpectedMetric(MetricType metric, double expected)
    {
        var function = (MeasurableFunction)TriangleFunction.Create("Triangle", 0, 5, 10);
        var evaluator = LarsenEvaluator.BuildPlan(function, 0.5, true);

        var actual = evaluator.Evaluate(metric, function, 8);

        Assert.Equal(expected, actual, 8);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Evaluate_ZeroOrIdentityFactor_ReturnsExpectedMetrics(double scale)
    {
        var function = (MeasurableFunction)TriangleFunction.Create("Triangle", 0, 5, 10);
        var evaluator = LarsenEvaluator.BuildPlan(function, scale, false);

        foreach (var metric in Enum.GetValues<MetricType>())
        {
            var expected = scale == 0 ? 0 : metric.GetOriginalMetric(function);

            Assert.Equal(expected, evaluator.Evaluate(metric, function, 8), 8);
        }
    }

    [Theory]
    [InlineData(0.25, 0.625)]
    [InlineData(0.5, 1.25)]
    [InlineData(0.75, 1.875)]
    [InlineData(1, 2.5)]
    public void Evaluate_NonUnitPeak_ScalesOriginalArea(double scale, double expected)
    {
        var function = (MeasurableFunction)TriangleFunction.Create("Triangle", 0, 5, 10, 0.5);
        var evaluator = LarsenEvaluator.BuildPlan(function, scale, false);

        var actual = evaluator.Evaluate(MetricType.Area, function, 8);

        Assert.Equal(expected, actual, 8);
    }
}
