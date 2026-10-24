using System.Reflection;
using Kernel.Function.Abstractions;
using Kernel.Function.Extensions.Metric;
using Kernel.Function.Implementations;
using Shared.Deferred;
using Shared.Intervals.Implementations;
using Shared.Options.Implementations;

namespace Kernel.Tests.Function;

public class FunctionCopyTests
{
    private static readonly string[] OriginalMetrics =
    [
        "DeferredArea", "DeferredMomentX", "DeferredMomentY",
        "DeferredMomentXx", "DeferredMomentXy", "DeferredMomentYy"
    ];

    private static readonly string[] ClippedMetrics =
    [
        "ClippedArea", "ClippedMomentX", "ClippedMomentY",
        "ClippedMomentXx", "ClippedMomentXy", "ClippedMomentYy"
    ];

    [Theory]
    [InlineData("Triangle", false)]
    [InlineData("Triangle", true)]
    [InlineData("Trapezoid", false)]
    [InlineData("Trapezoid", true)]
    [InlineData("Singleton", false)]
    [InlineData("Singleton", true)]
    [InlineData("Gaussian", false)]
    [InlineData("Gaussian", true)]
    [InlineData("GeneralizedBell", false)]
    [InlineData("GeneralizedBell", true)]
    [InlineData("Logistic", false)]
    [InlineData("Logistic", true)]
    public void DeepCopy_Default_PreservesUniverseAndIndependentMetricState(string shape, bool computeMetrics)
    {
        var source = CreateFunction(shape);
        if (computeMetrics)
            ComputeMetrics(source);

        var names = OriginalMetrics.Concat(ClippedMetrics).ToArray();
        var before = names.Select(name => GetCache(source, name).Get.TryGetValue(out _)).ToArray();

        var copy = (MeasurableFunction)((IMembershipFunction)source).DeepCopy();

        Assert.NotSame(source, copy);
        Assert.Equal(source.GetType(), copy.GetType());
        Assert.Equal(source.Name, copy.Name);
        Assert.Equal(source.UMax, copy.UMax);
        Assert.Equal(source.UniverseOfDiscourse.ToTuple(), copy.UniverseOfDiscourse.ToTuple());

        for (var i = 0; i < names.Length; i++)
        {
            var original = GetCache(source, names[i]).Get;
            var copied = GetCache(copy, names[i]).Get;
            Assert.NotSame(original, copied);
            Assert.Equal(before[i], original.TryGetValue(out var originalValue));
            Assert.Equal(before[i], copied.TryGetValue(out var copiedValue));
            if (before[i])
                Assert.Equal(originalValue, copiedValue);

            original.Invalidate();
            Assert.Equal(before[i], copied.HasValue);
        }
    }

    [Theory]
    [InlineData("Triangle")]
    [InlineData("Trapezoid")]
    [InlineData("Singleton")]
    [InlineData("Gaussian")]
    [InlineData("GeneralizedBell")]
    [InlineData("Logistic")]
    public void DeepCopy_WithoutState_PreservesOriginalCachesAndDiscardsClipping(string shape)
    {
        var source = CreateFunction(shape);
        ComputeMetrics(source);

        var copy = (MeasurableFunction)((IMembershipFunction)source).DeepCopy("Copy", copyState: false);

        Assert.Equal("Copy", copy.Name);
        Assert.Equal(shape, source.Name);
        Assert.True(copy.UniverseOfDiscourse.IsFullyUnbounded);
        Assert.False(copy.IsClippedByUoD);
        Assert.False(source.UniverseOfDiscourse.IsFullyUnbounded);

        foreach (var name in OriginalMetrics)
        {
            var original = GetCache(source, name).Get;
            var copied = GetCache(copy, name).Get;
            Assert.NotSame(original, copied);
            Assert.True(copied.TryGetValue(out var value));
            Assert.Equal(original.Value, value);
        }

        foreach (var name in ClippedMetrics)
        {
            Assert.True(GetCache(copy, name).IsNone);
            Assert.True(GetCache(source, name).Get.HasValue);
        }

        Assert.Equal(source.Area(MetricScope.Original), copy.Area());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DeepCopy_UncomputedMetric_EvaluatesOnlyTheCopy(bool copyState)
    {
        var source = CreateFunction("GeneralizedBell");
        var sourceMetric = GetCache(source, "DeferredMomentYy").Get;
        Assert.False(sourceMetric.HasValue);

        var copy = (MeasurableFunction)source.DeepCopy(copyState);
        var result = copy.MomentYy(MetricScope.Original);

        Assert.True(double.IsFinite(result));
        Assert.True(result > 0);
        Assert.True(GetCache(copy, "DeferredMomentYy").Get.HasValue);
        Assert.False(sourceMetric.HasValue);
    }

    private static void ComputeMetrics(MeasurableFunction function)
    {
        foreach (var metric in Enum.GetValues<MetricType>())
        {
            metric.GetOriginalMetric(function);
            metric.GetClippedMetric(function);
        }
    }

    private static Option<DeferredValue<double>> GetCache(MeasurableFunction function, string name) =>
        (Option<DeferredValue<double>>)typeof(MeasurableFunction)
            .GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(function)!;

    private static MeasurableFunction CreateFunction(string shape)
    {
        var universe = new Interval(2, 4);
        (Type Type, object[] Arguments) factory = shape switch
        {
            "Triangle" => (typeof(TriangleFunction), [shape, 0d, 3d, 6d, universe, 1d]),
            "Trapezoid" => (typeof(TrapezoidFunction), [shape, 0d, 2d, 4d, 6d, universe, 1d]),
            "Singleton" => (typeof(SingletonFunction), [shape, 3d, new Interval(2.98, 3.02), 1u, 1d]),
            "Gaussian" => (typeof(GaussianFunction), [shape, 3d, 1d, universe, 1d]),
            "GeneralizedBell" => (typeof(GeneralizedBellFunction), [shape, 1d, 2d, 3d, universe, 1d]),
            "Logistic" => (typeof(LogisticFunction), [shape, 1d, 3d, universe, 1d]),
            _ => throw new ArgumentOutOfRangeException(nameof(shape))
        };

        return (MeasurableFunction)factory.Type
            .GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic)!
            .Invoke(null, factory.Arguments)!;
    }
}
