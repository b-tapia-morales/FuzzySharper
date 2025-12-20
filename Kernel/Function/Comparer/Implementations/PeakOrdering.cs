using Kernel.Function.Abstractions;
using Kernel.Function.Comparer.Abstractions;

namespace Kernel.Function.Comparer.Implementations;

public class PeakOrdering : IFunctionComparer
{
    public int CompareMethod(IMembershipFunction x, IMembershipFunction y) => 
        y.UMax.CompareTo(x.UMax);
}