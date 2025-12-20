using Kernel.Number;
using Kernel.Operator.Conorm.Abstractions;

namespace Kernel.Operator.Conorm.Implementations.Parameterized;

public class PConorm(PIntersector @operator, double beta) : IConorm
{
    private PIntersector Operator { get; } = @operator;
    private double Beta { get; } = beta;

    public FuzzyNumber Union(FuzzyNumber x, FuzzyNumber y) => Operator.Function(x, y, Beta);
}