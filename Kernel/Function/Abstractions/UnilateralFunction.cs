using Kernel.Function.Implementations;
using Kernel.Number;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Kernel.Function.Abstractions;

public abstract class UnilateralFunction(string name, Interval universe, double uMax = 1) : MembershipFunction(name, universe, uMax)
{
    public abstract double SlopeBase { get; }

    public abstract double SlopePeak { get; }

    public override bool IsVerticallySymmetric => false;

    public override bool HasGlobalMaximum => true;

    public override bool IsBoundedLeft => SlopeBase < SlopePeak;

    public override bool IsBoundedRight => SlopeBase > SlopePeak;

    public override bool FloorsLeft => false;

    public override bool FloorsRight => false;

    public override bool SaturatesLeft => false;

    public override bool SaturatesRight => false;

    public override bool IsOpenLeft => !IsBoundedLeft && IsNormal;

    public override bool IsOpenRight => !IsBoundedRight && IsNormal;

    public override Option<double> PeakLeft => !IsBoundedLeft ? double.NegativeInfinity : SlopePeak;

    public override Option<double> PeakRight => !IsBoundedRight ? double.PositiveInfinity : SlopePeak;

    public override double SupportLeft => !IsBoundedLeft ? double.NegativeInfinity : SlopeBase;

    public override double SupportRight => !IsBoundedRight ? double.PositiveInfinity : SlopeBase;

    public override double EffectiveSupportLeft => IsBoundedLeft ? SlopeBase : SlopePeak;

    public override double EffectiveSupportRight => IsBoundedRight ? SlopePeak : SlopeBase;

    public override Option<double> CoreLeft => !IsNormal ? OptionFactory.None<double>() : PeakLeft;

    public override Option<double> CoreRight => !IsNormal ? OptionFactory.None<double>() : PeakRight;

    public override abstract Option<double> AlphaCutLeft(FuzzyNumber alpha);

    public override abstract Option<double> AlphaCutRight(FuzzyNumber alpha);

    public override abstract Func<double, double> LarsenProduct(FuzzyNumber lambda);

    public override abstract IMembershipFunction DeepCopy();

    public override abstract IMembershipFunction DeepCopy(string name);
}