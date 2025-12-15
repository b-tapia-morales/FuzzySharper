using Kernel.Function.Abstractions;
using Kernel.Function.Comparer.Abstractions;
using Kernel.Function.Extensions;
using Utils.Shape;

namespace Kernel.Function.Comparer.Implementations;

public class CentroidOrdering : IFunctionComparer
{
    public int CompareMethod(IMembershipFunction x, IMembershipFunction y) =>
        x.CalculateCentroid(Axis.X, false).CompareTo(y.CalculateCentroid(Axis.X, false));
}