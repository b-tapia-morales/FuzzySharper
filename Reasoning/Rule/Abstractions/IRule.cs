using Kernel.Number;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Aggregator.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Proposition.Abstractions;
using Reasoning.Rule.Components;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic;
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

    IReadOnlyList<IProposition> Premise { get; }
    int PremiseLength { get; }
    IReadOnlyList<StringOrType> PremiseVariables { get; }
    IReadOnlyDictionary<StringOrType, IReadOnlyList<IProposition>> PremiseDict { get; }

    bool IsPremiseEvaluable(IWorkingMemory memory);

    bool PremiseContains(StringOrType identifier);

    bool ConsequentContains(string variableName);

    IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory, INegation negation);

    IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory, IOperatorFamily operatorFamily);

    IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory);

    Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory,
        INegation negation, INorm norm, IConorm conorm);

    Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory, IOperatorFamily operatorFamily);

    Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory);

    void RecomputeAdaptation(uint maxHistorySize, IWeightAggregator aggregator);

    void ResetAdaptation();

    IRule DeepCopy(LifecycleMode mode = LifecycleMode.New);
}