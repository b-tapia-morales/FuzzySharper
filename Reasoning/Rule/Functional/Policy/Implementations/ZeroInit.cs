using Reasoning.Proposition.Abstractions;
using Reasoning.Rule.Functional.Policy.Abstractions;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class ZeroInit : BaseCoefficientInitPolicy
{
    public override IEnumerable<double> Initialize(IReadOnlyList<IProposition> premise) =>
        Enumerable.Range(0, premise.Count).Select(_ => 0D);
}