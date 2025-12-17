using Kernel.Number;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Aggregator.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Comparer.Implementations.Deterministic;
using Reasoning.Proposition.Abstractions;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Reasoning.Rule.Abstractions;

public interface IRule
{
    IProposition? Conditional { get; internal set; }
    ICollection<IProposition> Connectives { get; }
    IRuleOutput? Consequent { get; internal set; }
    bool IsFinalized { get; internal set; }
    Option<RulePriority> Priority { get; }
    Option<double> CertaintyFactor { get; }
    DateTimeOffset CreationTime { get; }
    AdaptationState AdaptationState { get; }

    bool IsPremiseEvaluable(IWorkingMemory memory);

    bool PremiseContains(StringOrType identifier);

    bool ConsequentContains(string variableName);

    int PremiseLength();

    IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory, INegation negation);

    IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory, IOperatorFamily operatorFamily);

    IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory);

    void UpdateLearning(uint maxHistorySize, IWeightAggregator aggregator);

    void ResetLearning();
}