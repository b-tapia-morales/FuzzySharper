using Reasoning.Proposition.Abstractions;
using Reasoning.Rule.Functional.Policy.Abstractions;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class SupportMidpointInit : BaseCoefficientInitPolicy
{
    public override IEnumerable<double> Initialize(IReadOnlyList<IProposition> premise) => 
        premise.Select(p => EvaluateCoefficient(p, prop => prop.Function.RestrictedSupport.Midpoint.Get));
}