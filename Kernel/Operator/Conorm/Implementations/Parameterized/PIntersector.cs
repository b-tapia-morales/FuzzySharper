using Kernel.Number;
using Shared.Enums;

namespace Kernel.Operator.Conorm.Implementations.Parameterized;

public class PIntersector : AbstractEnum<PIntersector, PIntersectorType>
{
    public static readonly PIntersector Hamacher =
        new(nameof(Hamacher), "Hamacher", (x, y, beta) => (x + y + (beta - 1) * x * y) / (1 + beta * x * y),
            (int) PIntersectorType.Hamacher);

    public static readonly PIntersector SugenoWeber =
        new(nameof(SugenoWeber), "Sugeno-Weber", (x, y, beta) => Math.Min(x + y + beta * x * y, 1),
            (int) PIntersectorType.SugenoWeber);

    private PIntersector(string name, string readableName, Func<FuzzyNumber, FuzzyNumber, double, FuzzyNumber> function, int value) : base(name, value)
    {
        ReadableName = readableName;
        Function = function;
    }

    public override string ReadableName { get; }
    public Func<FuzzyNumber, FuzzyNumber, double, FuzzyNumber> Function { get; }
}