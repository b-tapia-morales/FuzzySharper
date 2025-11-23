using Kernel.Function.Abstractions;
using Kernel.Number;
using Shared.Approx;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using static System.Math;

// ReSharper disable MemberCanBePrivate.Global

namespace Kernel.Function.Implementations;

public class LogisticFunction : SigmoidFunction
{
    private const double RoughlyZero = DoubleApproxExt.DefaultTolerance * 10e-1;
    private const double RoughlyOne = 1 - RoughlyZero;

    private double? _leftMost;
    private double? _rightMost;

    public LogisticFunction(string name, double a, double c, double uMax = 1) :
        this(name, a, c, Interval.Default, uMax)
    {
    }

    public LogisticFunction(string name, double a, double c, Interval universe, double uMax = 1) : base(name, universe, uMax)
    {
        CheckAValue(A);
        A = a;
        C = c;
    }

    public double A { get; }
    public double C { get; }

    public override double Center => C;

    public override double Steepness => A;

    public override double EffectiveSupportLeft =>
        _leftMost ??= AlphaCutLeft(SaturatesRight ? RoughlyZero : RoughlyOne).Get;

    public override double EffectiveSupportRight =>
        _rightMost ??= AlphaCutRight(SaturatesLeft ? RoughlyZero : RoughlyOne).Get;

    public override Option<double> AlphaCutLeft(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyGreaterOrEqualTo(UMax) || alpha.Value.IsRoughlyZero())
            return OptionFactory.None<double>();
        return SaturatesLeft ? double.NegativeInfinity : C - (double.Log(UMax / alpha.Value) - 1) / A;
    }

    public override Option<double> AlphaCutRight(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyGreaterOrEqualTo(UMax) || alpha.Value.IsRoughlyZero())
            return OptionFactory.None<double>();
        return SaturatesRight ? double.PositiveInfinity : C - (double.Log(UMax / alpha.Value) - 1) / A;
    }

    public override Func<double, double> LarsenProduct(FuzzyNumber lambda) =>
        x => lambda.Value * (1 / (1 + Exp(-A * (x - C))));

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