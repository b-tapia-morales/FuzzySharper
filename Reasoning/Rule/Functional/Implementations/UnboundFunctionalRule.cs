using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Extensions;
using Reasoning.Rule.Functional.Abstractions;

namespace Reasoning.Rule.Functional.Implementations;

public class UnboundFunctionalRule :
    AbstractFunctionalRule<UnboundFunctionalRule>, IContextFreeRule<UnboundFunctionalRule>, IBooleanPropositionRule<UnboundFunctionalRule>, IEquatable<UnboundFunctionalRule>
{
    public bool Equals(UnboundFunctionalRule? other) =>
        MemberwiseEquals(other);

    public override bool Equals(object? obj) =>
        obj is IFunctionalRule other && MemberwiseEquals(other);
    
    public override int GetHashCode() =>
        MemberwiseHashCode();

    public UnboundFunctionalRule If(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.If(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundFunctionalRule IfNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.IfNot(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundFunctionalRule And(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.And(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundFunctionalRule AndNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.AndNot(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundFunctionalRule Or(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.Or(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundFunctionalRule OrNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.OrNot(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundFunctionalRule If<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.If(this, value);

    public UnboundFunctionalRule IfNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.IfNot(this, value);

    public UnboundFunctionalRule And<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.And(this, value);

    public UnboundFunctionalRule AndNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.AndNot(this, value);

    public UnboundFunctionalRule Or<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.Or(this, value);

    public UnboundFunctionalRule OrNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.OrNot(this, value);
}