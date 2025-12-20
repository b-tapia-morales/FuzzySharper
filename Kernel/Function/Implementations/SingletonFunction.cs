using Kernel.Function.Abstractions;
using Kernel.Number;
using Shared.Approx;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Kernel.Function.Implementations;

public class SingletonFunction : LinearPiecewiseFunction
{
    internal static IMembershipFunction Create(string name, double center, Interval universe, uint decimalPlaces = 4U, double uMax = 1)
    {
        CheckDecimalPlaces(decimalPlaces);
        var epsilon = Math.Pow(10, -decimalPlaces);
        return new SingletonFunction(name, decimalPlaces, center, center - epsilon / 2, center + epsilon / 2, universe, uMax);
    }

    public static IMembershipFunction Create(string name, double center, uint decimalPlaces = 4U, double uMax = 1) =>
        Create(name, center, Interval.Default, decimalPlaces, uMax);

    private SingletonFunction(string name, uint decimalPlaces, double center, double left, double right, double uMax) :
        this(name, decimalPlaces, center, left, right, Interval.Default, uMax)
    {
    }

    private SingletonFunction(string name, uint decimalPlaces, double center, double left, double right, Interval universe, double uMax) :
        base(name, universe, uMax)
    {
        DecimalPlaces = decimalPlaces;
        Center = center;
        Left = left;
        Right = right;
    }

    private uint DecimalPlaces { get; }
    private double Center { get; }
    public double Left { get; }
    public double Right { get; }

    public override double LeftEdge => Left;
    public override double TopLeftCorner => Left;
    public override double TopRightCorner => Right;
    public override double RightEdge => Right;
    override protected double LeftSlope => Left;
    override protected double RightSlope => Right;
    override protected List<(double X, double Y)> Vertices => [(Left, 0), (Left, UMax), (Right, UMax), (Right, 0)];

    public override Option<double> AlphaCutLeft(FuzzyNumber alpha) =>
        alpha.Value.IsRoughlyZero() || alpha.Value.IsRoughlyGreaterThan(UMax) ? Option<double>.None() : Left;

    public override Option<double> AlphaCutRight(FuzzyNumber alpha) =>
        alpha.Value.IsRoughlyZero() || alpha.Value.IsRoughlyGreaterThan(UMax) ? Option<double>.None() : Right;

    public override Func<double, double> LarsenProduct(FuzzyNumber lambda) =>
        x => x.IsRoughlyGreaterThan(Left) && x.IsRoughlyLesserThan(Right) ? lambda.Value : 0;

    public override IMembershipFunction DeepCopy() =>
        Create(Name, Center, DecimalPlaces, UMax);

    public override IMembershipFunction DeepCopy(string name) =>
        Create(name, Center, DecimalPlaces, UMax);

    private static void CheckDecimalPlaces(uint decimalPlaces)
    {
        var message = $"Value provided was: {decimalPlaces}.";
        switch (decimalPlaces)
        {
            case 0:
                throw new ArgumentException($"Decimal places cannot be equal to 0. {message}", nameof(decimalPlaces));
            case > 15:
                throw new ArgumentException($"Decimal places cannot exceed the maximum amount of precision that a double can represent (15). {message}",
                    nameof(decimalPlaces));
        }
    }
}