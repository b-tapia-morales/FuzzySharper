using Kernel.Number;
using Shared.Enums;

namespace Kernel.Operator.Residuum.Implementations.Parameterized;

public class PResiduumOperator : AbstractEnum<PResiduumOperator, PResiduumType>
{
    public static readonly PResiduumOperator PseudoLukasiewicz1 =
        new(nameof(PseudoLukasiewicz1), "Pseudo-Łukasiewicz 1", (a, b, omega) => Math.Min(1, (1 - a + (1 - omega) * b) / (1 + omega * a)),
            (int) PResiduumType.PseudoLukasiewicz1);

    public static readonly PResiduumOperator PseudoLukasiewicz2 =
        new(nameof(PseudoLukasiewicz2), "Pseudo-Łukasiewicz 2", (a, b, omega) => Math.Min(1, 1 - Math.Pow(a, omega) + Math.Pow(b, omega)),
            (int) PResiduumType.PseudoLukasiewicz2);

    private PResiduumOperator(string name, string readableName, Func<FuzzyNumber, FuzzyNumber, double, FuzzyNumber> function, int value) : base(name, value)
    {
        ReadableName = readableName;
        Function = function;
    }

    public override string ReadableName { get; }
    public Func<FuzzyNumber, FuzzyNumber, double, FuzzyNumber> Function { get; }
}