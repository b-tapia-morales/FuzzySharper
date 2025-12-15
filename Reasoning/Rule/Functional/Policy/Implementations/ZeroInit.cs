using Reasoning.Proposition.Abstractions;
using Reasoning.Rule.Functional.Policy.Abstractions;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class ZeroInit : ICoefficientInitPolicy
{
    public (IList<double> Coefficients, double Bias) Initialize(IList<IProposition> propositions) =>
        ([..Enumerable.Range(0, propositions.Count).Select(_ => 0)], 0);
}