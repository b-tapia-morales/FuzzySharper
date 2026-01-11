using System.Collections.Immutable;
using System.Diagnostics;
using Kernel.Number;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Conorm.Implementations.Canonical;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Negation.Implementations.Canonical;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Norm.Implementations.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Aggregator.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Proposition.Abstractions;
using Reasoning.Proposition.Components;
using Reasoning.Rule.Components;
using Reasoning.Rule.Extensions;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Reasoning.Rule.Abstractions;

public abstract class AbstractRule : IRule
{
    public IProposition? Conditional { get; set; }
    public ICollection<IProposition> Connectives { get; protected init; } = new List<IProposition>();
    public IRuleOutput? Consequent { get; set; }
    public bool IsFinalized { get; set; }
    public Option<RulePriority> Priority { get; protected init; } = Option<RulePriority>.None();
    public Option<double> CertaintyFactor { get; protected init; } = Option<double>.None();
    public DateTimeOffset CreationTime { get; protected init; } = DateTimeOffset.Now;
    public AdaptationState AdaptationState { get; protected init; } = new();

    public IReadOnlyList<IProposition> Premise
    {
        get
        {
            this.Validate();
            return field ??= [Conditional!, ..Connectives];
        }
    }

    public int PremiseLength
    {
        get
        {
            this.Validate();
            return Connectives.Count + 1;
        }
    }

    public IReadOnlyList<StringOrType> PremiseVariables
    {
        get
        {
            this.Validate();
            return field ??= [..Premise.DistinctBy(e => e.Identifier).Select(e => e.Identifier)];
        }
    }

    public IReadOnlyDictionary<StringOrType, IReadOnlyList<IProposition>> PremiseDict
    {
        get
        {
            this.Validate();
            return field ??= Premise.GroupBy(e => e.Identifier).ToDictionary(g => g.Key, IReadOnlyList<IProposition> (g) => g.ToList());
        }
    }

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

    public bool ConsequentContains(string identifier)
    {
        this.Validate();
        return Consequent!.Contains(identifier);
    }

    public IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory, INegation negation)
    {
        this.Validate();
        return !IsPremiseEvaluable(memory)
            ? ImmutableList<FuzzyNumber>.Empty
            : Premise.Select(e => e.Evaluate(memory, negation).Get);
    }

    public IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory) =>
        ApplyUnaryOperators(memory, Negation.Standard);
    
    public Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory,
        INegation negation, INorm norm, IConorm conorm)
    {
        this.Validate();
        var numbers = new Stack<FuzzyNumber>(ApplyUnaryOperators(memory, negation));
        switch (numbers.Count)
        {
            case 0:
                return Option<FuzzyNumber>.None();
            case 1:
                return numbers.First();
        }

        var connectives = new Stack<Connective>(Connectives.Select(e => e.Connective));
        while (numbers.Count > 1)
        {
            var a = numbers.Pop();
            var b = numbers.Pop();
            var operation = connectives.Pop() == Connective.And ? norm.Intersection(a, b) : conorm.Union(a, b);
            numbers.Push(operation);
        }

        Debug.Assert(numbers.Count == 1);
        return numbers.Pop();
    }

    public Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory, IOperatorFamily operatorFamily) =>
        EvaluatePremiseWeight(memory, operatorFamily.Negation, operatorFamily.Norm, operatorFamily.Conorm);

    public Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory) =>
        EvaluatePremiseWeight(memory, Negation.Standard, Norm.Minimum, Conorm.Maximum);

    public void RecomputeAdaptation(uint maxHistorySize, IWeightAggregator aggregator) =>
        AdaptationState.Recompute(maxHistorySize, aggregator);

    public void ResetAdaptation() =>
        AdaptationState.Reset();

    public abstract IRule DeepCopy(LifecycleMode mode = LifecycleMode.NewInstance);

    public override string ToString()
    {
        this.Validate();
        return $"{Conditional} {(Connectives.Count != 0 ? $"{string.Join(' ', Connectives)} " : string.Empty)}{Consequent}";
    }
}