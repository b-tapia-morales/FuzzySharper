using Shared.Approx;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Kernel.Function.Abstractions;

public abstract class BellShapedFunction(string name, Interval universe, double uMax = 1) : MeasurableFunction(name, universe, uMax)
{
    public abstract double Center { get; }

    public override bool IsVerticallySymmetric => true;

    public override bool HasGlobalMaximum => true;

    public override bool IsBoundedLeft => false;

    public override bool IsBoundedRight => false;

    public override bool FloorsLeft => true;

    public override bool FloorsRight => true;

    public override bool SaturatesLeft => false;

    public override bool SaturatesRight => false;

    public override bool IsOpenLeft => false;

    public override bool IsOpenRight => false;

    public override Option<double> PeakLeft => Center;

    public override Option<double> PeakRight => Center;

    public override Option<double> CoreLeft => UMax.IsRoughlyOne() ? Center : Option<double>.None();

    public override Option<double> CoreRight => UMax.IsRoughlyOne() ? Center : Option<double>.None();

    public override double SupportLeft => double.NegativeInfinity;

    public override double SupportRight => double.PositiveInfinity;

    public abstract override double EffectiveSupportLeft { get; }

    public abstract override double EffectiveSupportRight { get; }
}