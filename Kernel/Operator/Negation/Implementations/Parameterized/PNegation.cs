using Kernel.Number;
using Kernel.Operator.Negation.Abstractions;

namespace Kernel.Operator.Negation.Implementations.Parameterized;

public class PNegation(PNegator @operator, double gamma) : INegation
{
    private PNegator Operator { get; } = @operator;
    private double Gamma { get; } = gamma;

    public FuzzyNumber Complement(FuzzyNumber x) => Operator.Function(x, Gamma);
}