using Kernel.Function.Implementations;
using Shared.Approx;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Kernel.Function.Abstractions;

public abstract class SigmoidFunction(string name, Interval universe, double uMax = 1) : MembershipFunction(name, universe, uMax)
{
    public abstract double Center { get; }

    public abstract double Steepness { get; }

    public override bool IsVerticallySymmetric => false;

    public override bool HasGlobalMaximum => false;

    public override bool IsBoundedLeft => false;

    public override bool IsBoundedRight => false;

    public override bool FloorsLeft => Steepness > 0;
    
    public override bool FloorsRight => Steepness < 0;

    public override bool SaturatesLeft => Steepness > 0;

    public override bool SaturatesRight => Steepness < 0;

    public override bool IsOpenLeft => UMax.IsRoughlyOne() && Steepness < 0;

    public override bool IsOpenRight => UMax.IsRoughlyOne() && Steepness > 0;

    public override double SupportLeft => double.NegativeInfinity;

    public override double SupportRight => double.PositiveInfinity;

    public override Option<double> PeakLeft => OptionFactory.None<double>();

    public override Option<double> PeakRight => OptionFactory.None<double>();
    public override Option<double> CoreLeft => OptionFactory.None<double>();

    public override Option<double> CoreRight => OptionFactory.None<double>();

    public override abstract double EffectiveSupportLeft { get; }

    public override abstract double EffectiveSupportRight { get; }
}