using Kernel.Function.Abstractions;
using Kernel.Number;
using Shared.Approx;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;

// ReSharper disable MemberCanBePrivate.Global

namespace Kernel.Function.Implementations;

public class RightTrapezoidFunction : UnilateralFunction
{
    public RightTrapezoidFunction(string name, double a, double b, double uMax = 1) :
        this(name, a, b, Interval.Default, uMax)
    {
    }

    public RightTrapezoidFunction(string name, double a, double b, Interval universe, double uMax = 1) :
        base(name, universe, uMax)
    {
        CheckValues(a, b);
        A = a;
        B = b;
    }

    public double A { get; }
    public double B { get; }

    public override double SlopeBase => A;

    public override double SlopePeak => B;

    public override Option<double> PeakLeft => B;

    public override Option<double> PeakRight => double.PositiveInfinity;

    public override Option<double> AlphaCutLeft(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyZero() || alpha.Value.IsRoughlyGreaterThan(UMax))
            return OptionFactory.None<double>();
        if (alpha.Value.RoughlyEquals(UMax))
            return A;
        return A + alpha.Value * (B - A);
    }

    public override Option<double> AlphaCutRight(FuzzyNumber alpha) =>
        alpha.Value.IsRoughlyGreaterThan(UMax) ? OptionFactory.None<double>() : double.PositiveInfinity;

    public override Func<double, double> LarsenProduct(FuzzyNumber lambda) => x =>
    {
        if (x > A && x < B)
            return lambda.Value * ((x - A) / (B - A));
        if (x >= B)
            return lambda.Value;
        return 0;
    };

    public override IMembershipFunction DeepCopy() =>
        new RightTrapezoidFunction(Name, A, B, UMax);

    public override IMembershipFunction DeepCopy(string name) =>
        new RightTrapezoidFunction(name, A, B, UMax);

    public override string ToString() =>
        $"Linguistic term: {Name} - Membership Function: Open Right Trapezoidal - Sides: (a: {A}, b: {B}) - μMax: {UMax}";

    private static void CheckValues(double a, double b)
    {
        if (a.IsRoughlyGreaterOrEqualTo(b))
            throw new ArgumentException(
                $"The following condition has been violated: a < b (Values provides were: {a}, {b})");
    }
}