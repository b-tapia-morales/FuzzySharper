using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Extensions;
using Reasoning.Rule.Functional.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic;

namespace Reasoning.Rule.Functional.Implementations;

public class BoundFunctionalRule :
    AbstractFunctionalRule<BoundFunctionalRule>, IContextFreeRule<BoundFunctionalRule>, IContextBoundRule<BoundFunctionalRule>, IBooleanPropositionRule<BoundFunctionalRule>, IEquatable<BoundFunctionalRule>
{
    public required ILinguisticBase LinguisticBase { get; init; }

    public static BoundFunctionalRule Create(ILinguisticBase linguisticBase) =>
        new()
        {
            LinguisticBase = linguisticBase
        };

    public static BoundFunctionalRule Create(ILinguisticBase linguisticBase, RulePriority priority) =>
        new()
        {
            LinguisticBase = linguisticBase,
            Priority = priority
        };

    public static BoundFunctionalRule Create(ILinguisticBase linguisticBase, double certaintyFactor) =>
        new()
        {
            LinguisticBase = linguisticBase,
            CertaintyFactor = certaintyFactor
        };

    public bool Equals(BoundFunctionalRule? other) =>
        MemberwiseEquals(other);

    public override bool Equals(object? obj) =>
        obj is IFunctionalRule other && MemberwiseEquals(other);
    
    public override int GetHashCode() =>
        MemberwiseHashCode();

    public BoundFunctionalRule If(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.If(this, linguisticBase, variableName, termName, hedgeType);

    public BoundFunctionalRule If(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.If(this, variableName, termName, hedgeType);

    public BoundFunctionalRule IfNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.IfNot(this, linguisticBase, variableName, termName, hedgeType);

    public BoundFunctionalRule IfNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.IfNot(this, variableName, termName, hedgeType);

    public BoundFunctionalRule And(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.And(this, linguisticBase, variableName, termName, hedgeType);

    public BoundFunctionalRule And(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.And(this, variableName, termName, hedgeType);

    public BoundFunctionalRule AndNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.AndNot(this, linguisticBase, variableName, termName, hedgeType);

    public BoundFunctionalRule AndNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.AndNot(this, variableName, termName, hedgeType);

    public BoundFunctionalRule Or(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.Or(this, linguisticBase, variableName, termName, hedgeType);

    public BoundFunctionalRule Or(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.Or(this, variableName, termName, hedgeType);

    public BoundFunctionalRule OrNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.OrNot(this, linguisticBase, variableName, termName, hedgeType);

    public BoundFunctionalRule OrNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.OrNot(this, variableName, termName, hedgeType);

    public BoundFunctionalRule If<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.If(this, value);

    public BoundFunctionalRule IfNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.IfNot(this, value);

    public BoundFunctionalRule And<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.And(this, value);

    public BoundFunctionalRule AndNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.AndNot(this, value);

    public BoundFunctionalRule Or<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.Or(this, value);

    public BoundFunctionalRule OrNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.OrNot(this, value);
}