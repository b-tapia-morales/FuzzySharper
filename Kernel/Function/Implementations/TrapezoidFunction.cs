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
        CheckTriangleShaped(b, c);
        CheckRectangleShaped(a, b, c, d);
        CheckEdges(a, b, c, d);
        var function = new TrapezoidFunction(name, a, b, c, d, universe, uMax);
        function.InitializeMetrics();
        return function;

        static void CheckTriangleShaped(double b, double c)
        {
            if (b.RoughlyEquals(c))
                throw new ArgumentException(
                    $"""
                     Expected b != c. Received b = {b}, c = {c}.
                     The resulting shape is a Triangle, not a Trapezoidal.
                     Use {nameof(TriangleFunction)} instead.
                     """);
        }

        static void CheckRectangleShaped(double a, double b, double c, double d)
        {
            if (a.RoughlyEquals(b) && c.RoughlyEquals(d))
                throw new ArgumentException(
                    $"""
                     At least one of these two conditions were expected to be met: a != b ∨ c != d. Received a = {a}, b = {b}, c = {c}, d = {d}.
                     The resulting shape is either a Rectangle or a Square, not a Trapezoidal.
                     """);
        }

        static void CheckEdges(double a, double b, double c, double d)
        {

            if (a.IsRoughlyGreaterThan(b) || b.IsRoughlyGreaterThan(c) || c.IsRoughlyGreaterThan(d))
                throw new ArgumentException(
                    $"""
                     Expected a <= b < c <= d. Received a = {a}, b = {b}, c = {c}, d = {d}.
                     The resulting shape is not a Trapezoid.
                     """);

        }
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
        return A + (alpha.Value / UMax) * (B - A);
    }

    public override Option<double> AlphaCutRight(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyZero() || alpha.Value.IsRoughlyGreaterThan(UMax))
            return Option<double>.None();
        if (alpha.Value.RoughlyEquals(UMax))
            return C;
        return D - (alpha.Value / UMax) * (D - C);
    }

    public override Func<double, double> LarsenProduct(FuzzyNumber lambda) => x =>
    {
        if (x.IsRoughlyGreaterThan(A) && x.IsRoughlyLesserThan(B))
            return lambda.Value * UMax * ((x - A) / (B - A));
        if (x.IsRoughlyGreaterOrEqualTo(B) && x.IsRoughlyLesserOrEqualTo(C))
            return lambda.Value * UMax;
        if (x.IsRoughlyGreaterThan(C) && x.IsRoughlyLesserThan(D))
            return lambda.Value * UMax * ((D - x) / (D - C));
        return 0;
    };

    public override IMembershipFunction DeepCopy(bool copyState = true) =>
        DeepCopy(Name, copyState);

    public override IMembershipFunction DeepCopy(string name, bool copyState = true)
    {
        var copy = new TrapezoidFunction(name, A, B, C, D, copyState ? UniverseOfDiscourse : Interval.Default, UMax);
        copy.CopyMetrics(this, copyState);
        return copy;
    }

    public override string ToString() =>
        $"Linguistic term: {Name} - Membership Function: {GetType().Name} - Sides: (a: {A}, b: {B}, c: {C}, d: {D}) - μMax: {UMax}";

}
