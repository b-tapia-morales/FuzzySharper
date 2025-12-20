using Reasoning.Proposition.Abstractions;
using Reasoning.Rule.Functional.Policy.Abstractions;
using Utils.RandomGenerator;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class RandomInit(uint decimals) : BaseCoefficientInitPolicy
{
    private uint Decimals { get; } = decimals switch
    {
        0 => throw new ArgumentException("The amount of decimal places cannot be zero."),
        >= 15 => throw new ArgumentException("The amount of decimal places cannot exceed the maximum amount of precision that a double can represent (15)."),
        _ => decimals
    };

    public static RandomInit Default => new(1);

    public override IEnumerable<double> Initialize(IReadOnlyList<IProposition> premise)
    {
        var bound = Math.Pow(10, -Decimals);
        return Enumerable.Range(0, premise.Count).Select(_ => RandomUtils.NextDouble(-bound, bound));
    }
}