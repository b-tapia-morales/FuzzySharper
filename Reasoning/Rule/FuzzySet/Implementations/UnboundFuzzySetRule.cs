using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Extensions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic;
using Reasoning.Rule.FuzzySet.Extensions;

namespace Reasoning.Rule.FuzzySet.Implementations;

public sealed class UnboundFuzzySetRule :
    AbstractFuzzySetRule, IUnboundFuzzySetRule<UnboundFuzzySetRule>, IBooleanPropositionRule<UnboundFuzzySetRule>, IEquatable<UnboundFuzzySetRule>
{
    public static UnboundFuzzySetRule Create() =>
        new();

    public static UnboundFuzzySetRule Create(RulePriority priority) =>
        new()
        {
            Priority = priority
        };

    public static UnboundFuzzySetRule Create(double certaintyFactor) =>
        new()
        {
            CertaintyFactor = certaintyFactor
        };

    public bool Equals(UnboundFuzzySetRule? other) =>
        MemberwiseEquals(other);

    public override bool Equals(object? obj) =>
        obj is AbstractFuzzySetRule other && MemberwiseEquals(other);

    public override int GetHashCode() =>
        MemberwiseHashCode();

    public UnboundFuzzySetRule If(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.If(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundFuzzySetRule IfNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.IfNot(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundFuzzySetRule And(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.And(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundFuzzySetRule AndNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.AndNot(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundFuzzySetRule Or(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.Or(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundFuzzySetRule OrNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.OrNot(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundFuzzySetRule Then(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundFuzzySetRuleExt.Then(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundFuzzySetRule If<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.If(this, value);

    public UnboundFuzzySetRule IfNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.IfNot(this, value);

    public UnboundFuzzySetRule And<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.And(this, value);

    public UnboundFuzzySetRule AndNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.AndNot(this, value);

    public UnboundFuzzySetRule Or<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.Or(this, value);

    public UnboundFuzzySetRule OrNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.OrNot(this, value);
}