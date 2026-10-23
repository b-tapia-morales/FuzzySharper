using Inference.Tree.Fuzzy.Abstractions;
using Inference.Tree.Fuzzy.Comparers.Implementations;

namespace Inference.Tree.Fuzzy.Comparers.Factory;

public static class MetricComparerFactory<T> where T: class, IMamdaniGoalTree<T>
{
    private static readonly IComparer<T> NodeCountComparer = new NodeCountComparer<T>();
    private static readonly IComparer<T> MaxDepthComparer = new MaxDepthComparer<T>();
    private static readonly IComparer<T> RuleCountComparer = new RuleCountComparer<T>();
    private static readonly IComparer<T> ActiveRuleCountComparer = new CoveredRuleComparer<T>();
    private static readonly IComparer<T> UncoveredRuleCountComparer = new UncoveredRuleComparer<T>();

    public static IComparer<T> GetInstance(TreeMetric method) => method switch
    {
        TreeMetric.NodeCount => NodeCountComparer,
        TreeMetric.MaxDepth => MaxDepthComparer,
        TreeMetric.CoveredRuleCount => ActiveRuleCountComparer,
        TreeMetric.RuleCount => RuleCountComparer,
        TreeMetric.UncoveredRuleCount => UncoveredRuleCountComparer,
        _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
    };
}