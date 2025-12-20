using Kernel.Function.Abstractions;
using Kernel.Number;
using Shared.Approx;
using Shared.Deferred;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using static System.Math;

// ReSharper disable MemberCanBePrivate.Global

namespace Kernel.Function.Implementations;

public class LogisticFunction : SigmoidFunction
{
    internal static IMembershipFunction Create(string name, double a, double c, Interval universe, double uMax = 1)
    {
        CheckAValue(a);
        return new LogisticFunction(name, a, c, universe, uMax);
    }

    public static IMembershipFunction Create(string name, double a, double c, double uMax = 1) =>
        Create(name, a, c, Interval.Default, uMax);

    private LogisticFunction(string name, double a, double c, double uMax) :
        this(name, a, c, Interval.Default, uMax)
    {
    }

    private LogisticFunction(string name, double a, double c, Interval universe, double uMax) :
        base(name, universe, uMax)
    {
        A = a;
        C = c;
    }

    public double A { get; }
    public double C { get; }

    private double? _leftMost;
    private double? _rightMost;

    private const double RoughlyZero = DoubleApproxExt.DefaultTolerance * 1e-1;
    private const double RoughlyOne = 1 - RoughlyZero;

    public override double Center => C;

    public override double Steepness => A;

    public override double EffectiveSupportLeft =>
        _leftMost ??= AlphaCutLeft(SaturatesRight ? RoughlyZero : RoughlyOne).Get;

    public override double EffectiveSupportRight =>
        _rightMost ??= AlphaCutRight(SaturatesLeft ? RoughlyZero : RoughlyOne).Get;

    public override Option<double> AlphaCutLeft(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyGreaterOrEqualTo(UMax) || alpha.Value.IsRoughlyZero())
            return Option<double>.None();
        return SaturatesLeft ? double.NegativeInfinity : C - (double.Log(UMax / alpha.Value) - 1) / A;
    }

    public override Option<double> AlphaCutRight(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyGreaterOrEqualTo(UMax) || alpha.Value.IsRoughlyZero())
            return Option<double>.None();
        return SaturatesRight ? double.PositiveInfinity : C - (double.Log(UMax / alpha.Value) - 1) / A;
    }

    public override Func<double, double> LarsenProduct(FuzzyNumber lambda) =>
        x => lambda.Value * (1 / (1 + Exp(-A * (x - C))));

    // [μMax * (Log [a * (1 + ℇ^(a * (x1 - c))] - Log [a * (1 + ℇ^(a * x0 - c))]/a
    override protected DeferredValue<double> DeferredArea
    {
        get
        {
            var (x0, x1) = EffectiveSupport.ToTuple();
            var negativePart = Log(A * (1 + Pow(E, A * (x0 - C))));
            var positivePart = Log(A * (1 + Pow(E, A * (x1 - C))));
            return new DeferredValue<double>((UMax / A) * (negativePart - positivePart));
        }
    }

    // (μMax^2 * (1/(1 + ℇ^(a (c - x0))) - 1/(1 + ℇ^(a (c - x1))) - Log[a] + Log[(a (ℇ^(a c) + ℇ^(a x1)))/(ℇ^(a c) + ℇ^(a x0))]))/(2 a)
    override protected DeferredValue<double> DeferredMomentX
    {
        get
        {
            var (x0, x1) = EffectiveSupport.ToTuple();
            var negativeSigmoid = 1 / (1 + Pow(E, A * (C - x1)));
            var positiveSigmoid = 1 / (1 + Pow(E, A * (C - x0)));
            var logNumerator = Pow(E, A * C) + Pow(E, A * x1);
            var logDenominator = Pow(E, A * C) + Pow(E, A * x0);
            return new DeferredValue<double>((Pow(UMax, 2) / (2 * A)) * (positiveSigmoid - negativeSigmoid - Log(A) + Log(A * (logNumerator / logDenominator))));
        }
    }

    override protected DeferredValue<double> DeferredMomentXx
    {
        get
        {
            var (x0, x1) = EffectiveSupport.ToTuple();
            // (3 + 2 ℇ^(a (c - x0)))/(1 + ℇ^(a (c - x0)))^2
            var positiveSquare = (3 + 2 * Pow(E, A * (C - x0))) / Pow(1 + Pow(E, A * (C - x0)), 2);
            // (3 + 2 ℇ^(a (c - x1)))/(1 + ℇ^(a (c - x1)))^2
            var negativeSquare = (3 + 2 * Pow(E, A * (C - x1))) / Pow(1 + Pow(E, A * (C - x1)), 2);
            // 2 Log[ℇ^(a (c - x0))]
            var positiveLog1 = 2 * Log(Pow(E, A * (C - x0)));
            // 2 Log[ℇ^(a (c - x1))]
            var negativeLog1 = 2 * Log(Pow(E, A * (C - x1)));
            // 2 Log[a (1 + ℇ^(a (c - x1)))]
            var positiveLog2 = 2 * Log(A * (1 + Pow(E, A * (C - x1))));
            // 2 Log[a (1 + ℇ^(a (c - x0)))]
            var negativeLog2 = 2 * Log(A * (1 + Pow(E, A * (C - x0))));
            return new DeferredValue<double>((Pow(UMax, 3) / (6 * A)) * (positiveSquare - negativeSquare + positiveLog1 - negativeLog1 + positiveLog2 - negativeLog2));
        }
    }

    public override IMembershipFunction DeepCopy() =>
        new LogisticFunction(Name, A, C, UMax);

    public override IMembershipFunction DeepCopy(string name) =>
        new LogisticFunction(name, A, C, UMax);

    #region ObjectOverrides

    public override string ToString() =>
        $"Linguistic term: {Name} - Membership Function: {GetType().Name} - Sides: (A: {A}, C: {C}) - μMax: {UMax}";

    #endregion

    private static void CheckAValue(double a)
    {
        if (a.IsRoughlyZero())
            throw new ArgumentException("The value for «A» cannot be equal to 0");
    }
}