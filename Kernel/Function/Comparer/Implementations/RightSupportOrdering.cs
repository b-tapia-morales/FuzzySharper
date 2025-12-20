using Kernel.Function.Abstractions;
using Kernel.Function.Comparer.Abstractions;

namespace Kernel.Function.Comparer.Implementations;

public class RightSupportOrdering: IFunctionComparer
{
    public int CompareMethod(IMembershipFunction x, IMembershipFunction y) =>
        x.EffectiveSupport.UpperBound.CompareTo(y.EffectiveSupport.UpperBound);
}