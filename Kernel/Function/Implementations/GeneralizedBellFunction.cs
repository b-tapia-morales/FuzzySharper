using Kernel.Function.Abstractions;
using Kernel.Number;
using Shared.Approx;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using static System.Math;

// ReSharper disable MemberCanBePrivate.Global

namespace Kernel.Function.Implementations;

public class GeneralizedBellFunction : BellShapedFunction
{
    internal static IMembershipFunction Create(string name, double a, double b, double c, Interval universe, double uMax = 1)
    {
        CheckAValue(a);
        CheckValues(a, b, c);
        return new GeneralizedBellFunction(name, a, b, c, universe, uMax);
    }

    public static IMembershipFunction Create(string name, double a, double b, double c, double uMax = 1) =>
        Create(name, a, b, c, Interval.Default, uMax);


    private GeneralizedBellFunction(string name, double a, double b, double c, double uMax) :
        this(name, a, b, c, Interval.Default, uMax)
    {
    }

    private GeneralizedBellFunction(string name, double a, double b, double c, Interval universe, double uMax) :
        base(name, universe, uMax)
    {
        A = a;
        B = b;
        C = c;
    }

    public double A { get; }
    public double B { get; }
    public double C { get; }

    private double? _leftMost;
    private double? _rightMost;

    private const double RoughlyZero = DoubleApproxExt.DefaultTolerance * 1e-1;

    #region BellShapedFunctionProperties

    public override double Center => C;

    #endregion

    #region MembershipFunctionIntervals

    public override double EffectiveSupportLeft =>
        _leftMost ??= AlphaCutLeft(RoughlyZero).Get;

    public override double EffectiveSupportRight =>
        _rightMost ??= AlphaCutRight(RoughlyZero).Get;

    #endregion

    #region AlphaCuts

    public override Option<double> AlphaCutLeft(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyZero() || alpha.Value.IsRoughlyGreaterThan(UMax))
            return OptionFactory.None<double>();
        if (alpha.Value.RoughlyEquals(UMax))
            return C;
        return C - A * Pow((1 - alpha.Value) / alpha.Value, 1 / (2 * B));
    }

    public override Option<double> AlphaCutRight(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyZero() || alpha.Value.IsRoughlyGreaterThan(UMax))
            return OptionFactory.None<double>();
        if (alpha.Value.RoughlyEquals(UMax))
            return C;
        return C + A * Pow((1 - alpha.Value) / alpha.Value, 1 / (2 * B));
    }

    #endregion

    #region ImplicationMethods

    public override Func<double, double> LarsenProduct(FuzzyNumber lambda) =>
        x => lambda.Value * (1 / (1 + Pow(Abs((x - C) / A), 2 * B)));

    #endregion

    #region CloningMethods

    public override IMembershipFunction DeepCopy() =>
        new GeneralizedBellFunction(Name, A, B, C, UniverseOfDiscourse, UMax);

    public override IMembershipFunction DeepCopy(string name) =>
        new GeneralizedBellFunction(name, A, B, C, UniverseOfDiscourse, UMax);

    #endregion

    #region ObjectOverrides

    public override string ToString() =>
        $"Linguistic term: {Name} - Membership Function: {GetType().Name} - Sides: (a: {A}, b: {B}, c: {C}) - μMax: {UMax}";

    #endregion

    private static void CheckAValue(double a)
    {
        if (a.IsRoughlyZero())
            throw new ArgumentException("The value for «a» cannot be equal to 0");
    }

    private static void CheckValues(double a, double b, double c)
    {
        if (a.IsRoughlyGreaterOrEqualTo(b) || b.IsRoughlyGreaterOrEqualTo(c))
            throw new ArgumentException("a < b < c");
    }
}