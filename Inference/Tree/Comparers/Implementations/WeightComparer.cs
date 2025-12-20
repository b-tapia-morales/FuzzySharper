using System.ComponentModel;
using Inference.Tree.Abstractions;
using Inference.Tree.Comparers.Abstractions;
using Inference.Tree.Comparers.Factory;
using Inference.Tree.Extensions;
using Shared.Approx;

namespace Inference.Tree.Comparers.Implementations;

public class WeightComparer<T> : IMetricComparer<T> where T : class, ITree<T>
{
    private readonly List<(TreeMetric Method, double Weight)> _weightedMetrics;

    public WeightComparer(params IEnumerable<(TreeMetric Metric, double Weight)> weightedMetrics)
    {
        ArgumentNullException.ThrowIfNull(weightedMetrics);
        var metrics = ValidateSize(weightedMetrics);
        _weightedMetrics = [..metrics.Select(pair => (ValidateEnum(pair.Metric), ValidateFactor(pair.Weight)))];
    }

    public int ComparerMethod(T x, T y) =>
        CalculateScore(y).CompareTo(CalculateScore(x));

    private double CalculateScore(T treeNode) => 
        _weightedMetrics.Aggregate(0D, (sum, tuple) => sum + tuple.Weight * treeNode.CalculateMetric(tuple.Method));

    private static List<(TreeMetric Metric, double Weight)> ValidateSize(IEnumerable<(TreeMetric Metric, double Weight)> weightedMetrics)
    {
        var metrics = weightedMetrics.ToList();
        return metrics.Count == 0
            ? throw new ArgumentException("At least one metric must be provided.", nameof(weightedMetrics))
            : metrics;
    }

    private static bool IsUndefined(TreeMetric metric) =>
        !Enum.IsDefined(metric);

    private static TreeMetric ValidateEnum(TreeMetric metric) =>
        IsUndefined(metric)
            ? throw new InvalidEnumArgumentException()
            : metric;

    private static bool IsOutsideUnitInterval(double weight) =>
        weight.IsRoughlyLesserThan(0) || weight.IsRoughlyGreaterThan(1) || double.IsInfinity(weight) || double.IsNaN(weight);

    private static double ValidateFactor(double weight) =>
        IsOutsideUnitInterval(weight)
            ? throw new ArgumentOutOfRangeException(nameof(weight), weight, "Value not in range [0, 1].")
            : weight.SnapToBounds(0, 1);
}