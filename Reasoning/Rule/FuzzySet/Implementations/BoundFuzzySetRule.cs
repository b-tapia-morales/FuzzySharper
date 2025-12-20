using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Extensions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic;
using Reasoning.Rule.FuzzySet.Extensions;

namespace Reasoning.Rule.FuzzySet.Implementations;

public sealed class BoundFuzzySetRule :
    AbstractFuzzySetRule, IUnboundFuzzySetRule<BoundFuzzySetRule>, IBoundFuzzySetRule<BoundFuzzySetRule>, IBooleanPropositionRule<BoundFuzzySetRule>, IEquatable<BoundFuzzySetRule>
{
    public required ILinguisticBase LinguisticBase { get; init; }

    public static BoundFuzzySetRule Create(ILinguisticBase linguisticBase) =>
        new()
        {
            LinguisticBase = linguisticBase
        };

    public static BoundFuzzySetRule Create(ILinguisticBase linguisticBase, RulePriority priority) =>
        new()
        {
            LinguisticBase = linguisticBase,
            Priority = priority
        };

    public static BoundFuzzySetRule Create(ILinguisticBase linguisticBase, double certaintyFactor) =>
        new()
        {
            LinguisticBase = linguisticBase,
            CertaintyFactor = certaintyFactor
        };
    
    public bool Equals(BoundFuzzySetRule? other) =>
        MemberwiseEquals(other);
    
    public override bool Equals(object? obj) =>
        obj is AbstractFuzzySetRule other && MemberwiseEquals(other);

    public override int GetHashCode() =>
        MemberwiseHashCode();

    public BoundFuzzySetRule If(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.If(this, linguisticBase, variableName, termName, hedgeType);

    public BoundFuzzySetRule If(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.If(this, variableName, termName, hedgeType);

    public BoundFuzzySetRule IfNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.IfNot(this, linguisticBase, variableName, termName, hedgeType);

    public BoundFuzzySetRule IfNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.IfNot(this, variableName, termName, hedgeType);

    public BoundFuzzySetRule And(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.And(this, linguisticBase, variableName, termName, hedgeType);

    public BoundFuzzySetRule And(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.And(this, variableName, termName, hedgeType);

    public BoundFuzzySetRule AndNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.AndNot(this, linguisticBase, variableName, termName, hedgeType);

    public BoundFuzzySetRule AndNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.AndNot(this, variableName, termName, hedgeType);

    public BoundFuzzySetRule Or(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.Or(this, linguisticBase, variableName, termName, hedgeType);

    public BoundFuzzySetRule Or(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.Or(this, variableName, termName, hedgeType);

    public BoundFuzzySetRule OrNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.OrNot(this, linguisticBase, variableName, termName, hedgeType);

    public BoundFuzzySetRule OrNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.OrNot(this, variableName, termName, hedgeType);

    public BoundFuzzySetRule Then(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundFuzzySetRuleExt.Then(this, linguisticBase, variableName, termName, hedgeType);

    public BoundFuzzySetRule Then(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundFuzzySetRuleExt.Then(this, variableName, termName, hedgeType);

    public BoundFuzzySetRule If<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.If(this, value);

    public BoundFuzzySetRule IfNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.IfNot(this, value);

    public BoundFuzzySetRule And<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.And(this, value);

    public BoundFuzzySetRule AndNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.AndNot(this, value);

    public BoundFuzzySetRule Or<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.Or(this, value);

    public BoundFuzzySetRule OrNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.OrNot(this, value);
}