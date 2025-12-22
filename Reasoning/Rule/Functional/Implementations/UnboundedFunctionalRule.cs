using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Components;
using Reasoning.Rule.Extensions;
using Reasoning.Rule.Functional.Abstractions;

namespace Reasoning.Rule.Functional.Implementations;

public class UnboundedFunctionalRule :
    AbstractFunctionalRule<UnboundedFunctionalRule>,
    IRule<UnboundedFunctionalRule>,
    IContextFreeRule<UnboundedFunctionalRule>,
    IBooleanPropositionRule<UnboundedFunctionalRule>,
    IEquatable<UnboundedFunctionalRule>
{
    public bool Equals(UnboundedFunctionalRule? other) =>
        MemberwiseEquals(other);

    public override bool Equals(object? obj) =>
        obj is IFunctionalRule other && MemberwiseEquals(other);

    public override int GetHashCode() =>
        MemberwiseHashCode();

    public UnboundedFunctionalRule DeepCopy(LifecycleMode lifecycleMode = LifecycleMode.New)
    {
        this.Validate();
        var adaptationState = lifecycleMode is LifecycleMode.New or LifecycleMode.ResetLearning ? new AdaptationState() : AdaptationState.DeepCopy();
        return new UnboundedFunctionalRule
        {
            Conditional = Conditional!.DeepCopy(),
            Connectives = [..Connectives.Select(e => e.DeepCopy())],
            Consequent = Consequent!.DeepCopy(),
            IsFinalized = true,
            Priority = Priority,
            CertaintyFactor = CertaintyFactor,
            CreationTime = lifecycleMode is LifecycleMode.Continuous or LifecycleMode.ResetLearning ? CreationTime : DateTimeOffset.Now,
            AdaptationState = adaptationState,
        };
    }

    public UnboundedFunctionalRule If(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.If(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundedFunctionalRule IfNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.IfNot(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundedFunctionalRule And(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.And(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundedFunctionalRule AndNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.AndNot(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundedFunctionalRule Or(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.Or(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundedFunctionalRule OrNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.OrNot(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundedFunctionalRule If<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.If(this, value);

    public UnboundedFunctionalRule IfNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.IfNot(this, value);

    public UnboundedFunctionalRule And<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.And(this, value);

    public UnboundedFunctionalRule AndNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.AndNot(this, value);

    public UnboundedFunctionalRule Or<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.Or(this, value);

    public UnboundedFunctionalRule OrNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.OrNot(this, value);
}