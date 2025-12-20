using Kernel.Number;
using Kernel.Operator.Residuum.Abstractions;
using Shared.Enums;
using static System.Math;

namespace Kernel.Operator.Residuum.Implementations;

public class Residuum : AbstractEnum<Residuum, ResiduumType>, IResiduum
{
    public static readonly Residuum Godel =
        new(nameof(Godel), "Gödel", (a, b) => a.Value <= b.Value ? 1 : b.Value, (int) ResiduumType.Godel);

    public static readonly Residuum Goguen =
        new(nameof(Goguen), "Goguen", (a, b) => a.Value <= b.Value ? 1 : b.Value / a.Value, (int) ResiduumType.Goguen);

    public static readonly Residuum Lukasiewicz =
        new(nameof(Lukasiewicz), "Łukasiewicz", (a, b) => Min(1, 1 - a.Value + b.Value),
            (int) ResiduumType.Lukasiewicz);

    public static readonly Residuum KleeneDienes =
        new(nameof(KleeneDienes), "Kleene-Dienes", (a, b) => Max(1 - a.Value, b.Value),
            (int) ResiduumType.KleeneDienes);

    public static readonly Residuum Reichenbach =
        new(nameof(Reichenbach), "Reichenbach", (a, b) => 1 - a.Value + a.Value * b.Value,
            (int) ResiduumType.Reichenbach);

    public static readonly Residuum Wu =
        new(nameof(Wu), "Wu", (a, b) => a.Value <= b.Value ? 1 : Min(1 - a.Value, b.Value),
            (int) ResiduumType.Wu);

    public static readonly Residuum Zadeh =
        new(nameof(Zadeh), "Zadeh", (a, b) => Max(1 - a.Value, Min(a.Value, b.Value)),
            (int) ResiduumType.Zadeh);

    private Residuum(string name, string readableName, Func<FuzzyNumber, FuzzyNumber, FuzzyNumber> function, int value) : base(name, value)
    {
        ReadableName = readableName;
        Function = function;
    }

    public override string ReadableName { get; }
    public Func<FuzzyNumber, FuzzyNumber, FuzzyNumber> Function { get; }

    public FuzzyNumber Implication(FuzzyNumber x, FuzzyNumber y) => Function(x, y);
}