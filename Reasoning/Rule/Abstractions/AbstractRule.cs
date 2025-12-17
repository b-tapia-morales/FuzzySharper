using System.Collections.Immutable;
using Kernel.Number;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Negation.Implementations.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Aggregator.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Comparer.Implementations.Deterministic;
using Reasoning.Proposition.Abstractions;
using Reasoning.Rule.Extensions;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Reasoning.Rule.Abstractions;

public abstract class AbstractRule : IRule
{
    public IProposition? Conditional { get; set; }
    public ICollection<IProposition> Connectives { get; } = new List<IProposition>();
    public IRuleOutput? Consequent { get; set; }
    public bool IsFinalized { get; set; }
    public Option<RulePriority> Priority { get; protected init; } = OptionFactory.None<RulePriority>();
    public Option<double> CertaintyFactor { get; protected init; } = OptionFactory.None<double>();
    public DateTimeOffset CreationTime { get; } = DateTimeOffset.Now;
    public AdaptationState AdaptationState { get; set; } = new();

    public bool IsPremiseEvaluable(IWorkingMemory memory)
    {
        this.Validate();
        return Conditional!.IsEvaluable(memory) &&
               Connectives.Count == 0 ||
               Connectives.All(e => e.IsEvaluable(memory));
    }

    public bool PremiseContains(StringOrType identifier)
    {
        this.Validate();
        return Equals(Conditional!.Identifier, identifier) ||
               Connectives.Any(e => Equals(e.Identifier, identifier));
    }

    public bool ConsequentContains(string variableName)
    {
        this.Validate();
        return Consequent!.ConsequentContains(variableName);
    }

    public int PremiseLength()
    {
        this.Validate();
        return Connectives.Count + 1;
    }

    public IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory, INegation negation)
    {
        this.Validate();
        return !IsPremiseEvaluable(memory)
            ? ImmutableList<FuzzyNumber>.Empty
            : Connectives
                .Prepend(Conditional!)
                .Select(e => e.Evaluate(memory, negation).Get);
    }

    public IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory, IOperatorFamily operatorFamily) =>
        ApplyUnaryOperators(memory, operatorFamily.Negation);

    public IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory) =>
        ApplyUnaryOperators(memory, Negation.Standard);

    public void UpdateLearning(uint maxHistorySize, IWeightAggregator aggregator) =>
        AdaptationState.Update(maxHistorySize, aggregator);

    public void ResetLearning() =>
        AdaptationState.Reset();
}