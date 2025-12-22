using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Components;
using Reasoning.Rule.Extensions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic;
using Reasoning.Rule.FuzzySet.Extensions;

namespace Reasoning.Rule.FuzzySet.Implementations;

public sealed class BoundedFuzzySetRule :
    AbstractFuzzySetRule,
    IRule<BoundedFuzzySetRule>,
    IUnboundedFuzzySetRule<BoundedFuzzySetRule>,
    IBoundedFuzzySetRule<BoundedFuzzySetRule>,
    IBooleanPropositionRule<BoundedFuzzySetRule>,
    IEquatable<BoundedFuzzySetRule>
{
    public required ILinguisticBase LinguisticBase { get; init; }

    public static BoundedFuzzySetRule Create(ILinguisticBase linguisticBase) =>
        new()
        {
            LinguisticBase = linguisticBase
        };

    public static BoundedFuzzySetRule Create(ILinguisticBase linguisticBase, RulePriority priority) =>
        new()
        {
            LinguisticBase = linguisticBase,
            Priority = priority
        };

    public static BoundedFuzzySetRule Create(ILinguisticBase linguisticBase, double certaintyFactor) =>
        new()
        {
            LinguisticBase = linguisticBase,
            CertaintyFactor = certaintyFactor
        };

    public bool Equals(BoundedFuzzySetRule? other) =>
        MemberwiseEquals(other);

    public override bool Equals(object? obj) =>
        obj is AbstractFuzzySetRule other && MemberwiseEquals(other);

    public override int GetHashCode() =>
        MemberwiseHashCode();

    public BoundedFuzzySetRule DeepCopy(LifecycleMode lifecycleMode = LifecycleMode.New)
    {
        this.Validate();
        var adaptationState = lifecycleMode is LifecycleMode.New or LifecycleMode.ResetLearning ? new AdaptationState() : AdaptationState.DeepCopy();
        return new BoundedFuzzySetRule
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

    public BoundedFuzzySetRule If(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.If(this, linguisticBase, variableName, termName, hedgeType);

    public BoundedFuzzySetRule If(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.If(this, variableName, termName, hedgeType);

    public BoundedFuzzySetRule IfNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.IfNot(this, linguisticBase, variableName, termName, hedgeType);

    public BoundedFuzzySetRule IfNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.IfNot(this, variableName, termName, hedgeType);

    public BoundedFuzzySetRule And(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.And(this, linguisticBase, variableName, termName, hedgeType);

    public BoundedFuzzySetRule And(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.And(this, variableName, termName, hedgeType);

    public BoundedFuzzySetRule AndNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.AndNot(this, linguisticBase, variableName, termName, hedgeType);

    public BoundedFuzzySetRule AndNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.AndNot(this, variableName, termName, hedgeType);

    public BoundedFuzzySetRule Or(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.Or(this, linguisticBase, variableName, termName, hedgeType);

    public BoundedFuzzySetRule Or(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.Or(this, variableName, termName, hedgeType);

    public BoundedFuzzySetRule OrNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.OrNot(this, linguisticBase, variableName, termName, hedgeType);

    public BoundedFuzzySetRule OrNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundRuleExt.OrNot(this, variableName, termName, hedgeType);

    public BoundedFuzzySetRule Then(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundFuzzySetRuleExt.Then(this, linguisticBase, variableName, termName, hedgeType);

    public BoundedFuzzySetRule Then(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        BoundFuzzySetRuleExt.Then(this, variableName, termName, hedgeType);

    public BoundedFuzzySetRule If<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.If(this, value);

    public BoundedFuzzySetRule IfNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.IfNot(this, value);

    public BoundedFuzzySetRule And<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.And(this, value);

    public BoundedFuzzySetRule AndNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.AndNot(this, value);

    public BoundedFuzzySetRule Or<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.Or(this, value);

    public BoundedFuzzySetRule OrNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.OrNot(this, value);
}