using Reasoning.Proposition.Abstractions;
using Reasoning.Rule.Functional.Policy.Abstractions;
using static Reasoning.Rule.Functional.Policy.Abstractions.ICoefficientInitPolicy;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class SupportMidpointInit : ICoefficientInitPolicy
{
    public (IList<double> Coefficients, double Bias) Initialize(IList<IProposition> propositions) => 
        ([..propositions.Select(p => EvaluateCoefficient(p, prop => prop.Function.RestrictedSupport.Midpoint.Get))], 0);
}