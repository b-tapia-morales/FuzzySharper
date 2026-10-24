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
        CheckWidth(a);
        CheckShape(b);
        CheckCenter(c);
        var function = new GeneralizedBellFunction(name, a, b, c, universe, uMax);
        function.InitializeMetrics();
        return function;

        static void CheckWidth(double a)
        {
            if (!double.IsFinite(a) || a.IsRoughlyLesserOrEqualTo(0)) 
                throw new ArgumentException($"Expected a > 0 and finite. Received a = {a}.", nameof(a));
        }

        static void CheckShape(double b)
        {
            if (!double.IsFinite(b) || b.IsRoughlyLesserOrEqualTo(0)) 
                throw new ArgumentException($"Expected b > 0 and finite. Received b = {b}.", nameof(b));
        }

        static void CheckCenter(double c)
        {
            if (!double.IsFinite(c)) 
                throw new ArgumentException($"Expected c to be finite. Received c = {c}.", nameof(c));
        }
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
        var d = A * Pow(1 / Q - 1, 1 / (2 * B));
        EffectiveSupportLeft = C - d;
        EffectiveSupportRight = C + d;
    }

    public double A { get; }
    public double B { get; }
    public double C { get; }

    private const double Q = 2 * DoubleApproxExt.DefaultTolerance;

    #region BellShapedFunctionProperties

    public override double Center => C;

    #endregion

    #region MembershipFunctionIntervals

    public override double EffectiveSupportLeft { get; }

    public override double EffectiveSupportRight { get; }

    #endregion

    #region AlphaCuts

    public override Option<double> AlphaCutLeft(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyZero() || alpha.Value.IsRoughlyGreaterThan(UMax))
            return Option<double>.None();
        if (alpha.Value.RoughlyEquals(UMax))
            return C;
        return C - A * Pow(UMax / alpha.Value - 1, 1 / (2 * B));
    }

    public override Option<double> AlphaCutRight(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyZero() || alpha.Value.IsRoughlyGreaterThan(UMax))
            return Option<double>.None();
        if (alpha.Value.RoughlyEquals(UMax))
            return C;
        return C + A * Pow(UMax / alpha.Value - 1, 1 / (2 * B));
    }

    #endregion

    #region ImplicationMethods

    public override Func<double, double> LarsenProduct(FuzzyNumber lambda) =>
        x => lambda.Value * UMax * (1 / (1 + Pow(Abs((x - C) / A), 2 * B)));

    #endregion

    #region CloningMethods

    public override IMembershipFunction DeepCopy(bool copyState = true) =>
        DeepCopy(Name, copyState);

    public override IMembershipFunction DeepCopy(string name, bool copyState = true)
    {
        var copy = new GeneralizedBellFunction(name, A, B, C, copyState ? UniverseOfDiscourse : Interval.Default, UMax);
        copy.CopyMetrics(this, copyState);
        return copy;
    }

    #endregion

    #region ObjectOverrides

    public override string ToString() =>
        $"Linguistic term: {Name} - Membership Function: {GetType().Name} - Sides: (a: {A}, b: {B}, c: {C}) - μMax: {UMax}";

    #endregion

}
