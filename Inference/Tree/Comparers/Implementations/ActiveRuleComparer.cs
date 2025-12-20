using Inference.Tree.Abstractions;
using Inference.Tree.Comparers.Abstractions;
using Inference.Tree.Extensions;

namespace Inference.Tree.Comparers.Implementations;

public class ActiveRuleComparer<T> : IMetricComparer<T> where T : class, ITree<T>
{
    public int ComparerMethod(T x, T y) => 
        y.ActiveRuleCount().CompareTo(x.ActiveRuleCount());
}