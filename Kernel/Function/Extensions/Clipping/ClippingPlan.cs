using Kernel.Function.Abstractions;
using Shared.Approx;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Kernel.Function.Extensions.Clipping;

public class ClippingPlan
{
    private const double Threshold = 0.1;

    public bool HasLeftCut { get; private init; }
    public bool HasRightCut { get; private init; }

    public Option<double> LeftCut { get; private init; } = OptionFactory.None<double>();
    public Option<double> RightCut { get; private init; } = OptionFactory.None<double>();

    public ClippingMode Mode { get; private init; }

    public static ClippingPlan BuildPlan(MeasurableFunction function)
    {
        var centroid = function.CentroidX();

        var (fx0, fx1) = function.EffectiveSupport.ToTuple();
        var (ux0, ux1) = function.UniverseOfDiscourse.ToTuple();

        var hasLeftCut = ux0.IsRoughlyGreaterThan(fx0);
        var hasRightCut = ux1.IsRoughlyLesserThan(fx1);

        if (!(hasLeftCut || hasRightCut))
            return new ClippingPlan
            {
                HasLeftCut = false,
                HasRightCut = false,
                Mode = ClippingMode.None
            };

        var hasCutPastCentroid = ux0.IsRoughlyGreaterThan(centroid) || ux1.IsRoughlyLesserThan(centroid);
        var outerWidth = fx1 - fx0;
        var innerWidth = ux1 - ux0;
        var innerRegionIsSmall = innerWidth.IsRoughlyLesserOrEqualTo(outerWidth * Threshold);

        return new ClippingPlan
        {
            HasLeftCut = hasLeftCut,
            HasRightCut = hasRightCut,
            LeftCut = hasLeftCut ? ux0 : OptionFactory.None<double>(),
            RightCut = hasRightCut ? ux1 : OptionFactory.None<double>(),
            Mode = hasCutPastCentroid || innerRegionIsSmall ? ClippingMode.Inner : ClippingMode.LeftPlusRight
        };
    }
}