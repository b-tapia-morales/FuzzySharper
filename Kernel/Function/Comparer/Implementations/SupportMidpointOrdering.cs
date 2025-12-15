using Kernel.Function.Abstractions;
using Kernel.Function.Comparer.Abstractions;
using Shared.Options.Factory;

namespace Kernel.Function.Comparer.Implementations;

public class SupportMidpointOrdering : IFunctionComparer
{
    public int CompareMethod(IMembershipFunction x, IMembershipFunction y)
    {
        var firstExists = x.EffectiveSupport.Midpoint.IsSomeVal(out var a);
        var secondExists = y.EffectiveSupport.Midpoint.IsSomeVal(out var b);
        return (firstExists, secondExists) switch
        {
            (false, false) => +0,
            (false, true) => -1,
            (true, false) => +1,
            _ => a.CompareTo(b)
        };
    }
}