using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Functional.Policy.Abstractions;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class SupportWidthInit : BaseCoefficientInitPolicy
{
    public override IEnumerable<double> Initialize(IReadOnlyList<FuzzyProposition> premise) =>
        premise.Select(prop => prop.Function.RestrictedSupport.Width.Get);
}