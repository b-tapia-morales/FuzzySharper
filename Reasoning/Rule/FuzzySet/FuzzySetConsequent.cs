using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Abstractions;

namespace Reasoning.Rule.FuzzySet;

public class FuzzySetConsequent(FuzzyProposition proposition) : IRuleOutput
{
    public FuzzyProposition Proposition { get; } = proposition;

    public string Target { get; } = proposition.Variable;
}