using Kernel.Function.Abstractions;
using Kernel.Number;
using Shared.Approx;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Utils.Trigonometric;

// ReSharper disable MemberCanBePrivate.Global

namespace Kernel.Function.Implementations;

public class TriangleFunction : LinearPiecewiseFunction
{
    internal static IMembershipFunction Create(string name, double a, double b, double c, Interval universe, double uMax = 1)
    {
        CheckEdges(a, b, c);
        CheckSides(a, b, c);
        var function = new TriangleFunction(name, a, b, c, universe, uMax);
        function.InitializeMetrics();
        return function;

        static void CheckEdges(double a, double b, double c)
        {
            if (a.IsRoughlyGreaterThan(b) || b.IsRoughlyGreaterThan(c))
                throw new ArgumentException(
                    $"""
                     Expected a <= b <= c. Received a = {a}, b = {b}, c = {c}.
                     The resulting shape is not a Triangle.
                     """);
        }

        static void CheckSides(double a, double b, double c)
        {
            if (a.RoughlyEquals(b) && b.RoughlyEquals(c))
                throw new ArgumentException(
                    $"""
                     At least one of these two conditions were expected to be met: a != b ∨ b != c. Received a = {a}, b = {b}, c = {c}.
                     The resulting membership function is the Singleton function.
                     """);
        }
    }

    public static IMembershipFunction Create(string name, double a, double b, double c, double uMax = 1) =>
        Create(name, a, b, c, Interval.Default, uMax);

    private TriangleFunction(string name, double a, double b, double c, double uMax = 1) :
        this(name, a, b, c, Interval.Default, uMax)
    {
    }

    private TriangleFunction(string name, double a, double b, double c, Interval universe, double uMax = 1) :
        base(name, universe, uMax)
    {
        A = a;
        B = b;
        C = c;
    }

    public double A { get; }
    public double B { get; }
    public double C { get; }

    public override double LeftEdge => A;

    public override double TopLeftCorner => B;

    public override double TopRightCorner => B;

    public override double RightEdge => C;

    protected override double LeftSlope => TrigonometricUtils.Distance((A, 0), (B, UMax));

    protected override double RightSlope => TrigonometricUtils.Distance((B, UMax), (C, 0));

    protected override List<(double X, double Y)> Vertices => [(A, 0), (B, UMax), (C, 0)];

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
            return B;
        return C - (alpha.Value / UMax) * (C - B);
    }

    public override Func<double, double> LarsenProduct(FuzzyNumber lambda) => x =>
    {
        if (x.IsRoughlyGreaterThan(A) && x.IsRoughlyLesserThan(B))
            return lambda.Value * UMax * ((x - A) / (B - A));
        if (x.RoughlyEquals(B))
            return lambda.Value * UMax;
        if (x.IsRoughlyGreaterThan(B) && x.IsRoughlyLesserThan(C))
            return lambda.Value * UMax * ((C - x) / (C - B));
        return 0;
    };

    public override IMembershipFunction DeepCopy(bool copyState = true) =>
        DeepCopy(Name, copyState);

    public override IMembershipFunction DeepCopy(string name, bool copyState = true)
    {
        var copy = new TriangleFunction(name, A, B, C, copyState ? UniverseOfDiscourse : Interval.Default, UMax);
        copy.CopyMetrics(this, copyState);
        return copy;
    }

    public override string ToString() =>
        $"Linguistic term: {Name} - Membership Function: Triangular - Sides: (a: {A}, b: {B}, c: {C}) - μMax: {UMax}";

}
