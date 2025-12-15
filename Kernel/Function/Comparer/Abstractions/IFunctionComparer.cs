using Kernel.Function.Abstractions;

namespace Kernel.Function.Comparer.Abstractions;

public interface IFunctionComparer: IComparer<IMembershipFunction>
{
    int IComparer<IMembershipFunction>.Compare(IMembershipFunction? x, IMembershipFunction? y)
    {
        if (x == null && y == null)
            return +0;
        if (x != null && y == null)
            return -1;
        if (x == null && y != null)
            return +1;
        var a = x ?? throw new ArgumentNullException(nameof(x));
        var b = y ?? throw new ArgumentNullException(nameof(y));
        return CompareMethod(a, b);
    }

    int CompareMethod(IMembershipFunction x, IMembershipFunction y);
}