using Inference.Tree.Abstractions;
using Inference.Tree.Comparers.Abstractions;

namespace Inference.Tree.Comparers.Implementations;

public class RuleCountComparer<T> : IMetricComparer<T> where T : class, ITree<T>
{
    public int ComparerMethod(T x, T y) => 
        y.Rules.Count.CompareTo(x.Rules.Count);
}