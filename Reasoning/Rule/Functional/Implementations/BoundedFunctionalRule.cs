using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Components;
using Reasoning.Rule.Extensions;
using Reasoning.Rule.Functional.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic;

namespace Reasoning.Rule.Functional.Implementations;

public class BoundedFunctionalRule :
    AbstractFunctionalRule<BoundedFunctionalRule>,
    IRule<BoundedFunctionalRule>,
    IContextFreeRule<BoundedFunctionalRule>,
    IContextBoundRule<BoundedFunctionalRule>,
    IBooleanPropositionRule<BoundedFunctionalRule>,
    IEquatable<BoundedFunctionalRule>
{
    public required ILinguisticBase LinguisticBase { get; init; }

    public static BoundedFunctionalRule Create(ILinguisticBase linguisticBase) =>
        new()
        {
            LinguisticBase = linguisticBase
        };

    public static BoundedFunctionalRule Create(ILinguisticBase linguisticBase, RulePriority priority) =>
        new()
        {
            LinguisticBase = linguisticBase,
            Priority = priority
        };

    public static BoundedFunctionalRule Create(ILinguisticBase linguisticBase, double certaintyFactor) =>
        new()
        {
            LinguisticBase = linguisticBase,
            CertaintyFactor = certaintyFactor
        };

    public bool Equals(BoundedFunctionalRule? other) =>
        MemberwiseEquals(other);

    public override bool Equals(object? obj) =>
        obj is IFunctionalRule other && MemberwiseEquals(other);

    public override int GetHashCode() =>
        MemberwiseHashCode();

    public BoundedFunctionalRule DeepCopy(LifecycleMode lifecycleMode = LifecycleMode.New)
    {
        this.Validate();
        var adaptationState = lifecycleMode is LifecycleMode.New or LifecycleMode.ResetLearning ? new AdaptationState() : AdaptationState.DeepCopy();
        return new BoundedFunctionalRule
        {
            LinguisticBase = LinguisticBase,
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

    public BoundedFunctionalRule If(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.If(this, linguisticBase, variableName, termName, hedgeType);

    public BoundedFunctionalRule If(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.If(this, variableName, termName, hedgeType);

    public BoundedFunctionalRule IfNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.IfNot(this, linguisticBase, variableName, termName, hedgeType);

    public BoundedFunctionalRule IfNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.IfNot(this, variableName, termName, hedgeType);

    public BoundedFunctionalRule And(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.And(this, linguisticBase, variableName, termName, hedgeType);

    public BoundedFunctionalRule And(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.And(this, variableName, termName, hedgeType);

    public BoundedFunctionalRule AndNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.AndNot(this, linguisticBase, variableName, termName, hedgeType);

    public BoundedFunctionalRule AndNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.AndNot(this, variableName, termName, hedgeType);

    public BoundedFunctionalRule Or(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.Or(this, linguisticBase, variableName, termName, hedgeType);

    public BoundedFunctionalRule Or(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.Or(this, variableName, termName, hedgeType);

    public BoundedFunctionalRule OrNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.OrNot(this, linguisticBase, variableName, termName, hedgeType);

    public BoundedFunctionalRule OrNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.OrNot(this, variableName, termName, hedgeType);

    public BoundedFunctionalRule If<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.If(this, value);

    public BoundedFunctionalRule IfNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.IfNot(this, value);

    public BoundedFunctionalRule And<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.And(this, value);

    public BoundedFunctionalRule AndNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.AndNot(this, value);

    public BoundedFunctionalRule Or<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.Or(this, value);

    public BoundedFunctionalRule OrNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.OrNot(this, value);
}