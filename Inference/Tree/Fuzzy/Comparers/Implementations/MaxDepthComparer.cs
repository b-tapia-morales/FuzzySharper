using Inference.Tree.Extensions;
using Inference.Tree.Fuzzy.Abstractions;
using Inference.Tree.Fuzzy.Comparers.Abstractions;

namespace Inference.Tree.Fuzzy.Comparers.Implementations;

public class MaxDepthComparer<T> : IMetricComparer<T> where T : class, IMamdaniGoalTree<T>
{
    public int ComparerMethod(T x, T y) =>
        y.MaxDepth().CompareTo(x.MaxDepth());
}