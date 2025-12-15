using System.Collections.Immutable;
using Kernel.Number;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Negation.Implementations.Canonical;
using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Comparer.Implementations.Deterministic;
using Reasoning.Proposition.Abstractions;
using Reasoning.Proposition.Components;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Reasoning.Rule.Abstractions;

public interface IRule
{
    IProposition? Conditional { get; internal set; }
    ICollection<IProposition> Connectives { get; }
    bool IsFinalized { get; internal set; }
    Option<RulePriority> Priority { get; }
    Option<double> CertaintyFactor { get; }
    DateTimeOffset CreationTime { get; }
    AdaptationState AdaptationState { get; internal set; }
    
    bool IsPremiseEvaluable(IWorkingMemory memory)
    {
        return Conditional!.IsEvaluable(memory) &&
               Connectives.Count == 0 ||
               Connectives.All(e => e.IsEvaluable(memory));
    }

    bool PremiseContains(StringOrType identifier)
    {
        return Equals(Conditional!.Identifier, identifier) ||
               Connectives.Any(e => Equals(e.Identifier, identifier));
    }

    int PremiseLength()
    {
        return Connectives.Count + 1;
    }

    IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory, INegation negation)
    {
        return !IsPremiseEvaluable(memory)
            ? ImmutableList<FuzzyNumber>.Empty
            : Connectives
                .Prepend(Conditional!)
                .Select(e => e.Evaluate(memory, negation).Get);
    }

    IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory, IOperatorFamily operatorFamily) =>
        ApplyUnaryOperators(memory, operatorFamily.Negation);

    IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory) =>
        ApplyUnaryOperators(memory, Negation.Standard);
    
    IRule If(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IRule If(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IRule If<T>(T value) where T : struct, Enum, IEquatable<T>;

    IRule IfNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IRule IfNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IRule IfNot<T>(T value) where T : struct, Enum, IConvertible;

    IRule And(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IRule And(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IRule And<T>(T value) where T : struct, Enum, IConvertible;

    IRule AndNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IRule AndNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IRule AndNot<T>(T value) where T : struct, Enum, IConvertible;

    IRule Or(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IRule Or(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IRule Or<T>(T value) where T : struct, Enum, IConvertible;

    IRule OrNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IRule OrNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IRule OrNot<T>(T value) where T : struct, Enum, IConvertible;
}