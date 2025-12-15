using Reasoning.Proposition.Abstractions;
using Reasoning.Rule.Functional.Policy.Abstractions;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class SupportWidthInit : ICoefficientInitPolicy
{
    public (IList<double> Coefficients, double Bias) Initialize(IList<IProposition> propositions) =>
        ([..propositions.Select(p => ICoefficientInitPolicy.EvaluateCoefficient(p, prop => prop.Function.RestrictedSupport.Width.Get))], 0);
}