using Kernel.Number;
using Kernel.Operator.Negation.Implementations.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Abstractions;

namespace Reasoning.Rule.FuzzySet.Components;

public class FuzzySetConsequent(FuzzyProposition proposition) : IRuleOutput
{
    public FuzzyProposition Proposition { get; } = proposition;
    public string Target { get; } = proposition.Identifier.AsString;

    public bool IsEvaluable(IWorkingMemory memory) =>
        Proposition.IsEvaluable(memory);

    public FuzzyNumber Evaluate(double crispValue) =>
        Proposition.Evaluate(crispValue, Negation.Standard);

    public override bool Equals(object? obj) =>
        Proposition.Equals(obj);

    public override int GetHashCode() =>
        Proposition.GetHashCode();

    public override string ToString() =>
        Proposition.ToString();
}