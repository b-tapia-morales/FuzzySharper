using Inference.Tree.Abstractions;
using Inference.Tree.Comparers.Abstractions;
using Inference.Tree.Extensions;

namespace Inference.Tree.Comparers.Implementations;

public class MaxDepthComparer<T> : IMetricComparer<T> where T : class, ITree<T>
{
    public int ComparerMethod(T x, T y) =>
        y.MaxDepth().CompareTo(x.MaxDepth());
}