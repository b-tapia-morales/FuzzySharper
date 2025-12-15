using System.Collections.Immutable;
using Kernel.Number;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Conorm.Implementations.Canonical;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Negation.Implementations.Canonical;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Norm.Implementations.Canonical;
using Kernel.Operator.Residuum.Abstractions;
using Kernel.Operator.Residuum.Implementations;
using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Aggregator.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Comparer.Implementations.Deterministic;
using Reasoning.Proposition.Abstractions;
using Reasoning.Proposition.Components;
using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Exceptions;
using Reasoning.Rule.Extensions;
using Shared.Approx;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Reasoning.Rule.Abstractions;

/// <summary>
/// <para>
/// A class representation for a fuzzy rule.
/// Fuzzy rules are built from existing <see cref="IEquatableProposition{TSelf}">Propositions</see> and <see cref="Connective">Connectives</see>.
/// </para>
/// <para>
/// According to propositional logic, to be considered valid, a rule must have both an
/// <see cref="Conditional" /> and a <see cref="Consequent" />.
/// <see cref="IEquatableProposition{TSelf}">Propositions</see> using the <see cref="Connective.And">Disjunctive</see> and <see cref="Connective.Or">Conjunctive</see>
/// operators in between are considered optional.
/// </para>
/// <para>
/// In addition to the conditions written above, the rule creation process must comply with the following policies:
/// </para>
/// <list type="number">
/// <item>
/// <description>
/// There can be one and only one proposition with the <see cref="Connective.If" /> connective.
/// </description>
/// </item>
/// <item>
/// <description>
/// To append propositions with the connectives:
/// <see cref="Connective.And" />, <see cref="Connective.Or" />, <see cref="Connective.Then" />,
/// there must already be a proposition appended with the <see cref="Connective.If" /> connective.
/// </description>
/// </item>
/// <item>
/// <description>
/// There cannot be a proposition with the connective <see cref="Connective.Then" /> using the
/// <see cref="Literal.IsNot"/> literal.
/// In other words, the consequent cannot be in negated form.
/// </description>
/// </item>
/// <item>
/// <description>
/// After a proposition with the <see cref="Connective.Then" /> connective is appended, no further
/// propositions can be appended, because the rule is considered to be <see cref="IsFinalized">Finalized</see>.
/// </description>
/// </item>
/// </list>
/// </summary>
/// <seealso cref="MissingAntecedentException" />
/// <seealso cref="DuplicatedAntecedentException" />
/// <seealso cref="NegatedConsequentException"/>
/// <seealso cref="FinalizedRuleException" />
public interface IFuzzySetRule
{
    IProposition? Conditional { get; internal set; }
    ICollection<IProposition> Connectives { get; }
    FuzzyProposition? Consequent { get; internal set; }
    bool IsFinalized { get; internal set; }
    Option<RulePriority> Priority { get; }
    Option<double> CertaintyFactor { get; }
    DateTimeOffset CreationTime { get; }
    AdaptationState AdaptationState { get; internal set; }

    IFuzzySetRule If(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IFuzzySetRule If(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IFuzzySetRule If<T>(T value) where T : struct, Enum, IEquatable<T>;

    IFuzzySetRule IfNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IFuzzySetRule IfNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IFuzzySetRule IfNot<T>(T value) where T : struct, Enum, IConvertible;

    IFuzzySetRule And(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IFuzzySetRule And(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IFuzzySetRule And<T>(T value) where T : struct, Enum, IConvertible;

    IFuzzySetRule AndNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IFuzzySetRule AndNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IFuzzySetRule AndNot<T>(T value) where T : struct, Enum, IConvertible;

    IFuzzySetRule Or(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IFuzzySetRule Or(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IFuzzySetRule Or<T>(T value) where T : struct, Enum, IConvertible;

    IFuzzySetRule OrNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IFuzzySetRule OrNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IFuzzySetRule OrNot<T>(T value) where T : struct, Enum, IConvertible;

    IFuzzySetRule Then(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    IFuzzySetRule Then(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    bool IsPremiseEvaluable(IWorkingMemory memory)
    {
        this.Validate();
        return Conditional!.IsEvaluable(memory) &&
               Connectives.Count == 0 ||
               Connectives.All(e => e.IsEvaluable(memory));
    }

    bool IsEvaluable(IWorkingMemory memory) =>
        IsPremiseEvaluable(memory) &&
        Consequent!.IsEvaluable(memory);

    bool PremiseContains(StringOrType identifier)
    {
        this.Validate();
        return Equals(Conditional!.Identifier, identifier) ||
               Connectives.Any(e => Equals(e.Identifier, identifier));
    }

    bool ConsequentContains(string variableName)
    {
        this.Validate();
        return string.Equals(Consequent!.Variable, variableName, StringComparison.OrdinalIgnoreCase);
    }

    int PremiseLength()
    {
        this.Validate();
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

    Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory,
        INegation negation, INorm norm, IConorm conorm)
    {
        var numbers = new Queue<FuzzyNumber>(ApplyUnaryOperators(memory, negation));
        switch (numbers.Count)
        {
            case 0:
                return OptionFactory.None<FuzzyNumber>();
            case 1:
                return numbers.First();
        }

        var connectives = new Queue<Connective>(Connectives.Select(e => e.Connective));
        while (numbers.Count > 1)
        {
            var a = numbers.Dequeue();
            var b = numbers.Dequeue();
            var operation = connectives.Dequeue() == Connective.And ? norm.Intersection(a, b) : conorm.Union(a, b);
            numbers.Enqueue(operation);
        }

        return numbers.Dequeue();
    }

    Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory, IOperatorFamily operatorFamily) =>
        EvaluatePremiseWeight(memory, operatorFamily.Negation, operatorFamily.Norm, operatorFamily.Conorm);

    Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory) =>
        EvaluatePremiseWeight(memory, Negation.Standard, Norm.Minimum, Conorm.Maximum);

    Option<FuzzyNumber> EvaluateConclusionWeight(IWorkingMemory memory)
    {
        this.Validate();
        return !memory.GetNumericFact(Consequent!.Variable).IsSomeVal(out var value)
            ? OptionFactory.None<FuzzyNumber>()
            : Consequent.Evaluate(value);
    }

    Option<FuzzyNumber> EvaluateRuleWeight(IWorkingMemory memory,
        INegation negation, INorm norm, IConorm conorm, IResiduum residuum) =>
        !IsEvaluable(memory)
            ? OptionFactory.None<FuzzyNumber>()
            : residuum.Implication(EvaluatePremiseWeight(memory, negation, norm, conorm).Get, EvaluateConclusionWeight(memory).Get);

    Option<FuzzyNumber> EvaluateRuleWeight(IWorkingMemory memory, IOperatorFamily operatorFamily) =>
        EvaluateRuleWeight(memory, operatorFamily.Negation, operatorFamily.Norm, operatorFamily.Conorm, operatorFamily.Residuum);

    Option<FuzzyNumber> EvaluateRuleWeight(IWorkingMemory memory) =>
        EvaluateRuleWeight(memory, Negation.Standard, Norm.Minimum, Conorm.Maximum, Residuum.Godel);

    void UpdateLearning(uint maxHistorySize, IWeightAggregator aggregator) =>
        AdaptationState.Update(maxHistorySize, aggregator);

    void ResetLearning() =>
        AdaptationState.Reset();

    internal static double ValidateFactor(double factor)
    {
        factor = factor.SnapToBounds(0, 1);
        if (factor.IsRoughlyLesserThan(0) || factor.IsRoughlyGreaterThan(1))
            throw new ArgumentOutOfRangeException(nameof(factor), factor, "Value not in range [0, 1].");
        return factor;
    }
}