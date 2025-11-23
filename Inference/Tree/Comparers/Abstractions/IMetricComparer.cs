using Inference.Tree.Abstractions;

namespace Inference.Tree.Comparers.Abstractions;

public interface IMetricComparer<in T> : IComparer<T> where T : class, ITree<T>
{
    int IComparer<T>.Compare(T? x, T? y)
    {
        if (x == null && y == null)
            return +0;
        if (x != null && y == null)
            return -1;
        if (x == null && y != null)
            return +1;
        var a = x ?? throw new ArgumentNullException(nameof(x));
        var b = y ?? throw new ArgumentNullException(nameof(y));
        return ComparerMethod(a, b);
    }

    int ComparerMethod(T x, T y);
}