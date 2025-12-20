using Kernel.Number;
using Shared.Enums;

namespace Kernel.Operator.Negation.Implementations.Parameterized;

public class PNegator : AbstractEnum<PNegator, PNegatorType>
{
    public static readonly PNegator Sugeno =
        new(nameof(Sugeno), "Sugeno", (a, gamma) => (1 - a) / (1 - gamma * a), (int) PNegatorType.Sugeno);

    public static readonly PNegator Yager =
        new(nameof(Yager), "Yager", (a, gamma) => Math.Pow(1 - Math.Pow(a, gamma), 1.0 / gamma),
            (int) PNegatorType.Yager);

    private PNegator(string name, string readableName, Func<FuzzyNumber, double, FuzzyNumber> function, int value) : base(name, value)
    {
        ReadableName = readableName;
        Function = function;
    }

    public override string ReadableName { get; }
    public Func<FuzzyNumber, double, FuzzyNumber> Function { get; }
}