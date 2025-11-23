using Kernel.Number;
using Kernel.Operator.Norm.Abstractions;
using Shared.Enums;
using static System.Math;

namespace Kernel.Operator.Norm.Implementations.Canonical;

public class Norm : AbstractEnum<Norm, NormType>, INorm
{
    public static readonly Norm Minimum =
        new(nameof(Minimum), "Minimum", (a, b) => Min(a.Value, b.Value), (int) NormType.Minimum);

    public static readonly Norm Product =
        new(nameof(Product), "Product", (a, b) => a.Value * b.Value, (int) NormType.Product);

    public static readonly Norm Lukasiewicz =
        new(nameof(Lukasiewicz), "Łukasiewicz", (a, b) => Max(0, a.Value + b.Value - 1), (int) NormType.Lukasiewicz);

    public static readonly Norm NilpotentMinimum =
        new(nameof(NilpotentMinimum), "Nilpotent Minimum", (a, b) => a.Value + b.Value > 1 ? Min(a.Value, b.Value) : 0,
            (int) NormType.NilpotentMinimum);

    public static readonly Norm Drastic =
        new(nameof(Drastic), "Drastic", (a, b) => (a | b) == FuzzyNumber.Max ? (a & b) : FuzzyNumber.Min, (int) NormType.Drastic);

    private Norm(string name, string readableName, Func<FuzzyNumber, FuzzyNumber, FuzzyNumber> function, int value) : base(name, value)
    {
        ReadableName = readableName;
        Function = function;
    }

    public override string ReadableName { get; }
    public Func<FuzzyNumber, FuzzyNumber, FuzzyNumber> Function { get; }

    public FuzzyNumber Intersection(FuzzyNumber x, FuzzyNumber y) => Function(x, y);
}