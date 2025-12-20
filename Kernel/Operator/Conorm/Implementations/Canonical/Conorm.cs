using Kernel.Number;
using Kernel.Operator.Conorm.Abstractions;
using Shared.Enums;
using static System.Math;

namespace Kernel.Operator.Conorm.Implementations.Canonical;

public class Conorm : AbstractEnum<Conorm, ConormType>, IConorm
{
    public static readonly Conorm Maximum =
        new(nameof(Maximum), "Maximum", (a, b) => Max(a.Value, b.Value), (int) ConormType.Maximum);

    public static readonly Conorm ProbabilisticSum =
        new(nameof(ProbabilisticSum), "Probabilistic Sum", (a, b) => a.Value + b.Value - a.Value * b.Value,
            (int) ConormType.ProbabilisticSum);

    public static readonly Conorm Lukasiewicz =
        new(nameof(Lukasiewicz), "Łukasiewicz", (a, b) => Min(1, a.Value + b.Value), (int) ConormType.Lukasiewicz);

    public static readonly Conorm NilpotentMaximum =
        new(nameof(NilpotentMaximum), "Nilpotent Maximum", (a, b) => a.Value + b.Value < 1 ? Max(a.Value, b.Value) : 1,
            (int) ConormType.NilpotentMaximum);

    public static readonly Conorm Drastic =
        new(nameof(Drastic), "Drastic", (a, b) => (a & b) == FuzzyNumber.Min ? (a | b) : FuzzyNumber.Max, (int) ConormType.Drastic);

    private Conorm(string name, string readableName, Func<FuzzyNumber, FuzzyNumber, FuzzyNumber> function, int value) : base(name, value)
    {
        ReadableName = readableName;
        Function = function;
    }

    public override string ReadableName { get; }
    private Func<FuzzyNumber, FuzzyNumber, FuzzyNumber> Function { get; }

    public FuzzyNumber Union(FuzzyNumber x, FuzzyNumber y) => Function(x, y);
}