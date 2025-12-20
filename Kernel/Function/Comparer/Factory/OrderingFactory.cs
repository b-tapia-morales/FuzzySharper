using Kernel.Function.Abstractions;
using Kernel.Function.Comparer.Implementations;

namespace Kernel.Function.Comparer.Factory;

public class OrderingFactory
{
    private static readonly IComparer<IMembershipFunction> CentroidOrdering = new CentroidOrdering();
    private static readonly IComparer<IMembershipFunction> PeakOrdering = new PeakOrdering();
    private static readonly IComparer<IMembershipFunction> LeftSupportOrdering = new LeftSupportOrdering();
    private static readonly IComparer<IMembershipFunction> RightSupportOrdering = new RightSupportOrdering();
    private static readonly IComparer<IMembershipFunction> SupportMidpointOrdering = new SupportMidpointOrdering();
    private static readonly IComparer<IMembershipFunction> SupportWidthOrdering = new SupportWidthOrdering();

    public static IComparer<IMembershipFunction> GetInstance(OrderingMethod method) => method switch
    {
        OrderingMethod.Centroid => CentroidOrdering,
        OrderingMethod.Peak => PeakOrdering,
        OrderingMethod.LeftSupport => LeftSupportOrdering,
        OrderingMethod.RightSupport => RightSupportOrdering,
        OrderingMethod.SupportMidpoint => SupportMidpointOrdering,
        OrderingMethod.SupportWidth => SupportWidthOrdering,
        _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
    };
}