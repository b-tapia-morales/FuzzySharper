using Shared.Approx;
using Shared.Deferred;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Utils.Shape;

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

    protected override DeferredValue<double> DeferredArea => new(PolygonUtils.CalculateArea(Vertices));

    protected override DeferredValue<double> DeferredMomentX => new(PolygonUtils.CalculateFirstMoment(Vertices, Axis.X));

    protected override DeferredValue<double> DeferredMomentY => new(PolygonUtils.CalculateFirstMoment(Vertices, Axis.Y));

    protected override DeferredValue<double> DeferredMomentXx => new(PolygonUtils.CalculateSecondMoment(Vertices, Axis.X, Axis.X));

    protected override DeferredValue<double> DeferredMomentXy => new(PolygonUtils.CalculateSecondMoment(Vertices, Axis.X, Axis.Y));

    protected override DeferredValue<double> DeferredMomentYy => new(PolygonUtils.CalculateSecondMoment(Vertices, Axis.Y, Axis.Y));
}