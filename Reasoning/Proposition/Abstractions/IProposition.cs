using Kernel.Number;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Negation.Implementations.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Proposition.Components;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Reasoning.Proposition.Abstractions;

public interface IProposition
{
    public StringOrType Identifier { get; }
    public string Variable { get; }
    public Connective Connective { get; }
    public Literal Literal { get; }
    public string Term { get; }

    bool IsEvaluable(IWorkingMemory memory);

    Option<FuzzyNumber> Evaluate(IWorkingMemory memory, INegation negation);

    Option<FuzzyNumber> Evaluate(IWorkingMemory memory) => Evaluate(memory, Negation.Standard);

    FuzzyNumber Evaluate(DoubleOrEnum value, INegation negation);

    FuzzyNumber Evaluate(DoubleOrEnum value) => Evaluate(value, Negation.Standard);
}