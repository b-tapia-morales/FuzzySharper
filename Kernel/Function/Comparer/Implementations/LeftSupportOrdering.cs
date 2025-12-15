using Kernel.Function.Abstractions;
using Kernel.Function.Comparer.Abstractions;

namespace Kernel.Function.Comparer.Implementations;

public class LeftSupportOrdering : IFunctionComparer
{
    public int CompareMethod(IMembershipFunction x, IMembershipFunction y) =>
        x.EffectiveSupport.LowerBound.CompareTo(y.EffectiveSupport.LowerBound);
}