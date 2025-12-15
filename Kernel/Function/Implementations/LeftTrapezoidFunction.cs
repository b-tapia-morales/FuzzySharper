using Kernel.Function.Abstractions;
using Kernel.Number;
using Shared.Approx;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;

// ReSharper disable MemberCanBePrivate.Global

namespace Kernel.Function.Implementations;

public class LeftTrapezoidFunction : UnilateralFunction
{
    internal static IMembershipFunction Create(string name, double a, double b, Interval universe, double uMax = 1)
    {
        CheckValues(a, b);
        return new LeftTrapezoidFunction(name, a, b, universe, uMax);
    }
    
    public static IMembershipFunction Create(string name, double a, double b, double uMax = 1) =>
        Create(name, a, b, Interval.Default, uMax);
    
    private LeftTrapezoidFunction(string name, double a, double b, double uMax) :
        this(name, a, b, Interval.Default, uMax)
    {
    }

    private LeftTrapezoidFunction(string name, double a, double b, Interval universe, double uMax) :
        base(name, universe, uMax)
    {
        A = a;
        B = b;
    }

    public double A { get; }
    public double B { get; }
    
    public override double SlopeBase => B;
    
    public override double SlopePeak => A;

    public override Option<double> AlphaCutLeft(FuzzyNumber alpha) =>
        alpha.Value.IsRoughlyGreaterThan(UMax) ? OptionFactory.None<double>() : double.NegativeInfinity;

    public override Option<double> AlphaCutRight(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyZero() || alpha.Value.IsRoughlyGreaterThan(UMax))
            return OptionFactory.None<double>();
        if (alpha.Value.RoughlyEquals(UMax))
            return A;
        return B - alpha.Value * (B - A);
    }

    public override Func<double, double> LarsenProduct(FuzzyNumber lambda) =>
        x =>
        {
            if (x > A && x < B)
                return lambda.Value * ((B - x) / (B - A));
            if (x <= A)
                return lambda.Value;
            return 0;
        };

    public override IMembershipFunction DeepCopy() =>
        new LeftTrapezoidFunction(Name, A, B, UMax);

    public override IMembershipFunction DeepCopy(string name) =>
        new LeftTrapezoidFunction(name, A, B, UMax);

    public override string ToString() =>
        $"Linguistic term: {Name} - Membership Function: {GetType().Name} - Sides: (a: {A}, b: {B}) - μMax: {UMax}";

    private static void CheckValues(double a, double b)
    {
        if (a.IsRoughlyGreaterOrEqualTo(b))
            throw new ArgumentException(
                $"The following condition has been violated: a < b (Values provides were: {a}, {b})");
    }
}