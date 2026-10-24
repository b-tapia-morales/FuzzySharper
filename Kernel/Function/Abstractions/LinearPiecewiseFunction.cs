using Kernel.Function.Extensions.Metric;
using Kernel.Number;
using Shared.Approx;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Kernel.Function.Abstractions;

public abstract class LinearPiecewiseFunction(string name, Interval universe, double uMax = 1) : MeasurableFunction(name, universe, uMax)
{
    public abstract double LeftEdge { get; }
    public abstract double TopLeftCorner { get; }
    public abstract double TopRightCorner { get; }
    public abstract double RightEdge { get; }
    protected abstract double LeftSlope { get; }
    protected abstract double RightSlope { get; }
    protected abstract List<(double X, double Y)> Vertices { get; }

    public override bool HasGlobalMaximum => true;

    public override bool IsVerticallySymmetric => LeftSlope.RoughlyEquals(RightSlope);

    public override bool IsBoundedLeft => true;

    public override bool IsBoundedRight => true;

    public override bool FloorsLeft => false;

    public override bool FloorsRight => false;

    public override bool SaturatesLeft => false;

    public override bool SaturatesRight => false;

    public override bool IsOpenLeft => false;

    public override bool IsOpenRight => false;

    public override Option<double> PeakLeft => TopLeftCorner;

    public override Option<double> PeakRight => TopRightCorner;

    public override double SupportLeft => LeftEdge;

    public override double SupportRight => RightEdge;

    public override Option<double> CoreLeft => UMax.IsRoughlyOne() ? TopLeftCorner : Option<double>.None();

    public override Option<double> CoreRight => UMax.IsRoughlyOne() ? TopRightCorner : Option<double>.None();

    protected override double CalculateOriginalMetric(MetricType metricType) =>
        metricType.CalculateMetric(Vertices);

    public List<(double X, double Y)> AlphaCutLeftRegion(FuzzyNumber alpha, bool useUoD = true)
    {
        var x0 = useUoD ? RestrictedSupportLeft : EffectiveSupportLeft;
        var cut = useUoD ? AlphaCutLeftClipped(alpha) : AlphaCutLeft(alpha);

        if (!cut.IsSome(out var a0) || a0.RoughlyEquals(x0))
            return [];

        if (EffectiveSupportLeft.RoughlyEquals(x0))
            return [(x0, 0), (a0, 0), (a0, alpha)];

        var h0 = PureFunction(x0);
        return [(x0, 0), (a0, 0), (a0, alpha), (x0, h0)];
    }

    public List<(double X, double Y)> AlphaCutRightRegion(FuzzyNumber alpha, bool useUoD = true)
    {
        var x1 = useUoD ? RestrictedSupportRight : EffectiveSupportRight;
        var cut = useUoD ? AlphaCutRightClipped(alpha) : AlphaCutRight(alpha);

        if (!cut.IsSome(out var a1) || a1.RoughlyEquals(x1))
            return [];

        if (EffectiveSupportRight.RoughlyEquals(x1))
            return [(a1, 0), (x1, 0), (a1, alpha)];

        var h1 = PureFunction(x1);
        return [(a1, 0), (x1, 0), (x1, h1), (a1, alpha)];
    }
}
