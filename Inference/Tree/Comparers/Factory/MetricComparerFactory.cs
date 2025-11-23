using Inference.Tree.Abstractions;
using Inference.Tree.Comparers.Implementations;

namespace Inference.Tree.Comparers.Factory;

public static class MetricComparerFactory<T> where T: class, ITree<T>
{
    private static readonly IComparer<T> NodeCountComparer = new NodeCountComparer<T>();
    private static readonly IComparer<T> MaxDepthComparer = new MaxDepthComparer<T>();
    private static readonly IComparer<T> RuleCountComparer = new RuleCountComparer<T>();
    private static readonly IComparer<T> ActiveRuleCountComparer = new ActiveRuleComparer<T>();
    private static readonly IComparer<T> UncoveredRuleCountComparer = new UncoveredRuleComparer<T>();

    public static IComparer<T> GetInstance(TreeMetric method) => method switch
    {
        TreeMetric.NodeCount => NodeCountComparer,
        TreeMetric.MaxDepth => MaxDepthComparer,
        TreeMetric.ActiveRuleCount => ActiveRuleCountComparer,
        TreeMetric.RuleCount => RuleCountComparer,
        TreeMetric.UncoveredRuleCount => UncoveredRuleCountComparer,
        _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
    };
}