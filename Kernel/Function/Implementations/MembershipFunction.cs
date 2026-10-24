using Kernel.Function.Abstractions;
using Kernel.Number;
using Shared.Approx;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Kernel.Function.Implementations;

public abstract class MembershipFunction : IMembershipFunction
{
    protected MembershipFunction(string name, Interval universe, double uMax = 1)
    {
        CheckName(name);
        CheckHeight(uMax);
        Name = name;
        UMax = uMax.SnapTo(1);
        UniverseOfDiscourse = universe;
        return;

        static void CheckHeight(double h)
        {
            if (!double.IsFinite(h) || h.IsRoughlyLesserOrEqualTo(0) || h.IsRoughlyGreaterThan(1))
                throw new ArgumentException(
                    $"Expected 0 < uMax <= 1. Received uMax = {h}.");
        }

        static void CheckName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    $"Expected name to contain at least one non-whitespace character. Received name = {(name is null ? "null" : $"\"{name}\"")}.");
        }
    }

    public string Name { get; }
    public double UMax { get; }
    public Interval UniverseOfDiscourse { get; internal set; }

    public Func<double, double> PureFunction =>
        LarsenProduct(FuzzyNumber.Max);

    public Func<double, double> PureFunctionClipped =>
        LarsenProductClipped(FuzzyNumber.Max);

    public abstract bool IsVerticallySymmetric { get; }

    public virtual bool HasGlobalMaximum =>
        Peak.IsSome;

    public virtual bool IsBoundedLeft =>
        !double.IsNegativeInfinity(SupportLeft);

    public virtual bool IsBoundedRight =>
        !double.IsPositiveInfinity(SupportRight);

    public abstract bool FloorsLeft { get; }

    public abstract bool FloorsRight { get; }

    public abstract bool SaturatesLeft { get; }

    public abstract bool SaturatesRight { get; }

    public virtual bool IsZeroConvergent =>
        (IsBoundedLeft || FloorsLeft) && (IsBoundedRight || FloorsRight);

    public virtual bool IsStrictlyUnimodal =>
        Peak.IsSome(out var interval) && interval.IsSingleton;

    public virtual bool IsOpenLeft =>
        UMax.IsRoughlyOne() && SaturatesLeft;

    public virtual bool IsOpenRight =>
        UMax.IsRoughlyOne() && SaturatesRight;

    public virtual bool IsClosed =>
        IsZeroConvergent;

    public virtual bool IsNormal =>
        UMax.IsRoughlyOne() && Peak.IsSome;

    public virtual bool IsPrototypical =>
        UMax.IsRoughlyOne() && Peak.IsSome(out var interval) && interval.IsSingleton;
    
    public virtual bool IsClippedByUoD =>
        (UniverseOfDiscourse.IsBoundedLeft && UniverseOfDiscourse.LowerBound.IsRoughlyGreaterThan(EffectiveSupportLeft)) ||
        (UniverseOfDiscourse.IsBoundedRight && UniverseOfDiscourse.UpperBound.IsRoughlyLesserThan(EffectiveSupportRight));

    public abstract Option<double> PeakLeft { get; }

    public abstract Option<double> PeakRight { get; }

    public virtual Option<Interval> Peak =>
        HasGlobalMaximum ? new Interval(PeakLeft.Get, PeakRight.Get) : Option<Interval>.None();

    public Option<double> PeakLeftClipped =>
        !HasGlobalMaximum
            ? Option<double>.None()
            : Math.Clamp(PeakLeft.Get, UniverseOfDiscourse.LowerBound, UniverseOfDiscourse.UpperBound);

    public Option<double> PeakRightClipped =>
        !HasGlobalMaximum
            ? Option<double>.None()
            : Math.Clamp(PeakRight.Get, UniverseOfDiscourse.LowerBound, UniverseOfDiscourse.UpperBound);

    public Option<Interval> PeakClipped =>
        PeakLeftClipped.IsSome(out var left) && PeakRightClipped.IsSome(out var right)
            ? new Interval(left, right)
            : Option<Interval>.None();

    public abstract double SupportLeft { get; }

    public abstract double SupportRight { get; }

    public virtual Interval Support =>
        new(SupportLeft, SupportRight);

    public virtual double EffectiveSupportLeft =>
        SupportLeft;

    public virtual double EffectiveSupportRight =>
        SupportRight;

    public virtual Interval EffectiveSupport =>
        new(EffectiveSupportLeft, EffectiveSupportRight);

    public virtual double RestrictedSupportLeft =>
        Math.Max(EffectiveSupportLeft, UniverseOfDiscourse.LowerBound);

    public virtual double RestrictedSupportRight =>
        Math.Min(EffectiveSupportRight, UniverseOfDiscourse.UpperBound);

    public virtual Interval RestrictedSupport =>
        new(RestrictedSupportLeft, RestrictedSupportRight);

    public abstract Option<double> CoreLeft { get; }

    public abstract Option<double> CoreRight { get; }

    public virtual Option<Interval> Core =>
        IsNormal ? new Interval(CoreLeft.Get, CoreRight.Get) : Option<Interval>.None();

    public double CrossoverLeft =>
        AlphaCutLeft(UMax / 2).Get;

    public double CrossoverRight =>
        AlphaCutRight(UMax / 2).Get;

    public Interval Crossover =>
        new(CrossoverLeft, CrossoverRight);

    public abstract Option<double> AlphaCutLeft(FuzzyNumber alpha);

    public abstract Option<double> AlphaCutRight(FuzzyNumber alpha);

    public virtual Option<Interval> AlphaCut(FuzzyNumber alpha) =>
        alpha.Value.IsRoughlyLesserOrEqualTo(UMax)
            ? new Interval(AlphaCutLeft(alpha).Get, AlphaCutRight(alpha).Get)
            : Option<Interval>.None();

    public virtual Option<double> AlphaCutLeftClipped(FuzzyNumber alpha)
    {
        if (!AlphaCutLeft(alpha).IsSome(out var a0) ||
            !AlphaCutRight(alpha).IsSome(out var a1) || 
            a0 > RestrictedSupportRight || 
            a1 < RestrictedSupportLeft)
            return Option<double>.None();

        return Math.Max(a0, RestrictedSupportLeft);
    }

    public virtual Option<double> AlphaCutRightClipped(FuzzyNumber alpha)
    {
        if (!AlphaCutRight(alpha).IsSome(out var a1) ||
            !AlphaCutLeft(alpha).IsSome(out var a0) || 
            a0 > RestrictedSupportRight || 
            a1 < RestrictedSupportLeft)
            return Option<double>.None();

        return Math.Min(a1, RestrictedSupportRight);
    }

    public virtual Option<Interval> AlphaCutClipped(FuzzyNumber alpha) =>
        AlphaCutLeftClipped(alpha).IsSome(out var a0) && AlphaCutRightClipped(alpha).IsSome(out var a1)
            ? new Interval(a0, a1)
            : Option<Interval>.None();

    public abstract Func<double, double> LarsenProduct(FuzzyNumber lambda);

    public Func<double, double> LarsenProductClipped(FuzzyNumber lambda) => x =>
    {
        var (x0, x1) = RestrictedSupport.ToTuple();
        return x.IsRoughlyLesserThan(x0) || x.IsRoughlyGreaterThan(x1) ? 0 : LarsenProduct(lambda)(x);
    };

    public Func<double, double> MamdaniMinimum(FuzzyNumber alpha) =>
        PrecomputedMamdani(this, alpha);

    public Func<double, double> MamdaniMinimumClipped(FuzzyNumber alpha) =>
        PrecomputedMamdani(this, alpha, true);

    public FuzzyNumber MembershipDegree(double x) =>
        PureFunction(x);

    public FuzzyNumber MembershipDegreeClipped(double x) =>
        PureFunctionClipped(x);

    public abstract IMembershipFunction DeepCopy(bool copyState = true);

    public abstract IMembershipFunction DeepCopy(string name, bool copyState = true);

    private static Func<double, double> PrecomputedMamdani(MembershipFunction function, FuzzyNumber alpha,
        bool clipToUniverse = false)
    {
        if (alpha.Value.IsRoughlyZero())
            return _ => 0;
        var func = clipToUniverse ? function.PureFunctionClipped : function.PureFunction;
        if (alpha.Value.IsRoughlyGreaterOrEqualTo(function.UMax))
            return func;
        var (a0, a1) = (clipToUniverse ? function.AlphaCutClipped(alpha) : function.AlphaCut(alpha)).Get.ToTuple();
        return x => x.IsRoughlyGreaterOrEqualTo(a0) && x.IsRoughlyLesserOrEqualTo(a1) ? alpha.Value : func(x);
    }
}
