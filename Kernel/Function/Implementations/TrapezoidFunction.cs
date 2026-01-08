using Kernel.Function.Abstractions;
using Kernel.Number;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Utils.Trigonometric;
using static Shared.Approx.DoubleApproxExt;

// ReSharper disable MemberCanBePrivate.Global

namespace Kernel.Function.Implementations;

public class TrapezoidFunction : LinearPiecewiseFunction
{
    internal static IMembershipFunction Create(string name, double a, double b, double c, double d, Interval universe, double uMax = 1)
    {
        CheckIfTriangle(b, c);
        CheckIfRectangular(a, b, c, d);
        CheckEdges(a, b, c, d);
        return new TrapezoidFunction(name, a, b, c, d, universe, uMax);
    }

    public static IMembershipFunction Create(string name, double a, double b, double c, double d, double uMax = 1) =>
        Create(name, a, b, c, d, Interval.Default, uMax);

    private TrapezoidFunction(string name, double a, double b, double c, double d, double uMax = 1) :
        this(name, a, b, c, d, Interval.Default, uMax)
    {
    }

    private TrapezoidFunction(string name, double a, double b, double c, double d, Interval universe, double uMax) :
        base(name, universe, uMax)
    {
        A = a;
        B = b;
        C = c;
        D = d;
    }

    public double A { get; }
    public double B { get; }
    public double C { get; }
    public double D { get; }

    public override double LeftEdge => A;

    public override double TopLeftCorner => B;

    public override double TopRightCorner => C;

    public override double RightEdge => D;

    protected override double LeftSlope => TrigonometricUtils.Distance((A, 0), (B, UMax));

    protected override double RightSlope => TrigonometricUtils.Distance((C, UMax), (D, 0));

    protected override List<(double X, double Y)> Vertices => [(A, 0), (B, UMax), (C, UMax), (D, 0)];

    public override Option<double> AlphaCutLeft(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyZero() || alpha.Value.IsRoughlyGreaterThan(UMax))
            return Option<double>.None();
        if (alpha.Value.RoughlyEquals(UMax))
            return B;
        return A + alpha.Value * (B - A);
    }

    public override Option<double> AlphaCutRight(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyZero() || alpha.Value.IsRoughlyGreaterThan(UMax))
            return Option<double>.None();
        if (alpha.Value.RoughlyEquals(UMax))
            return B;
        return D - alpha.Value * (D - C);
    }

    public override Func<double, double> LarsenProduct(FuzzyNumber lambda) => x =>
    {
        if (x.IsRoughlyGreaterThan(A) && x.IsRoughlyLesserThan(B))
            return lambda.Value * ((x - A) / (B - A));
        if (x.IsRoughlyGreaterOrEqualTo(B) && x.IsRoughlyLesserOrEqualTo(C))
            return lambda.Value;
        if (x.IsRoughlyGreaterThan(C) && x.IsRoughlyLesserThan(D))
            return lambda.Value * ((D - x) / (D - C));
        return 0;
    };

    public override IMembershipFunction DeepCopy() =>
        new TrapezoidFunction(Name, A, B, C, D, UMax);

    public override IMembershipFunction DeepCopy(string name) =>
        new TrapezoidFunction(name, A, B, C, D, UMax);

    public override string ToString() =>
        $"Linguistic term: {Name} - Membership Function: {GetType().Name} - Sides: (a: {A}, b: {B}, c: {C}, d: {D}) - μMax: {UMax}";

    private static void CheckIfTriangle(double b, double c)
    {
        if (b.RoughlyEquals(c))
            throw new ArgumentException(
                $"""
                 The following condition has been violated: B ≠ C (Values provides were: {b}, {c}).
                 The resulting shape is a Triangle, not a Trapezoidal.
                 If you wish to use a triangular function, create an instance of the {nameof(TriangleFunction)} class instead."");
                 """);
    }

    private static void CheckIfRectangular(double a, double b, double c, double d)
    {
        if (a.RoughlyEquals(b) && c.RoughlyEquals(d))
            throw new ArgumentException(
                $"""
                 The following condition has been violated: A ≠ B ∨ C ≠ D (Values provides were: {a}, {b}, {c}, {d})
                 The resulting shape is either a Rectangle or a Square, not a Trapezoidal.
                 """);
    }

    private static void CheckEdges(double a, double b, double c, double d)
    {

        if (a.IsRoughlyGreaterThan(b) || b.IsRoughlyGreaterThan(c) || c.IsRoughlyGreaterThan(d))
            throw new ArgumentException(
                $"""
                 The following condition has been violated: A <= B < C <= D (Values provides were: {a}, {b}, {c}, {d})
                 The resulting shape is not a Trapezoid.
                 """);

    }
}