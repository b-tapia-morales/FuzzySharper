using Inference.Tree.Fuzzy.Abstractions;
using Inference.Tree.Fuzzy.Comparers.Abstractions;

namespace Inference.Tree.Fuzzy.Comparers.Implementations;

public class RuleCountComparer<T> : IMetricComparer<T> where T : class, IMamdaniGoalTree<T>
{
    public int ComparerMethod(T x, T y) => 
        y.Rules.Count.CompareTo(x.Rules.Count);
}