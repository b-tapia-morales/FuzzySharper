using Kernel.Number;
using Kernel.Operator.Residuum.Abstractions;

namespace Kernel.Operator.Residuum.Implementations.Parameterized;

public class PResiduum(PResiduumOperator @operator, double omega) : IResiduum
{
    private PResiduumOperator Operator { get; } = @operator;
    private double Omega { get; } = omega;

    public FuzzyNumber Implication(FuzzyNumber x, FuzzyNumber y) => Operator.Function(x, y, Omega);
}