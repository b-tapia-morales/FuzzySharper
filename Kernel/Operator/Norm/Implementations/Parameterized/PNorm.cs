using Kernel.Number;
using Kernel.Operator.Norm.Abstractions;

namespace Kernel.Operator.Norm.Implementations.Parameterized;

public class PNorm(PUnitor @operator, double alpha) : INorm
{
    private PUnitor Operator { get; } = @operator;
    private double Alpha { get; } = alpha;

    public FuzzyNumber Intersection(FuzzyNumber x, FuzzyNumber y) => Operator.Function(x, y, Alpha);
}