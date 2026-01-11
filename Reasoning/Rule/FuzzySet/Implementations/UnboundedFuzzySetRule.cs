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

public sealed class UnboundedFuzzySetRule :
    AbstractFuzzySetRule,
    IUnboundedFuzzySetRule<UnboundedFuzzySetRule>,
    IBooleanPropositionRule<UnboundedFuzzySetRule>,
    IEquatable<UnboundedFuzzySetRule>
{
    public static UnboundedFuzzySetRule Create() =>
        new();

    public static UnboundedFuzzySetRule Create(RulePriority priority) =>
        new()
        {
            Priority = priority
        };

    public static UnboundedFuzzySetRule Create(double certaintyFactor) =>
        new()
        {
            CertaintyFactor = certaintyFactor
        };

    public bool Equals(UnboundedFuzzySetRule? other) =>
        MemberwiseEquals(other);

    public override bool Equals(object? obj) =>
        obj is AbstractFuzzySetRule other && MemberwiseEquals(other);

    public override int GetHashCode() =>
        MemberwiseHashCode();
    
    public override IFuzzySetRule DeepCopy(LifecycleMode mode = LifecycleMode.NewInstance)
    {
        this.Validate();
        var adaptationState = mode is LifecycleMode.NewInstance or LifecycleMode.ResetLearning ? new AdaptationState() : AdaptationState.DeepCopy();
        return new UnboundedFuzzySetRule
        {
            Conditional = Conditional!.DeepCopy(),
            Connectives = [..Connectives.Select(e => e.DeepCopy())],
            Consequent = Consequent!.DeepCopy(),
            IsFinalized = true,
            Priority = Priority,
            CertaintyFactor = CertaintyFactor,
            CreationTime = mode is LifecycleMode.PreserveState or LifecycleMode.ResetLearning ? CreationTime : DateTimeOffset.Now,
            AdaptationState = adaptationState,
        };
    }

    public UnboundedFuzzySetRule If(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.If(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundedFuzzySetRule IfNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.IfNot(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundedFuzzySetRule And(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.And(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundedFuzzySetRule AndNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.AndNot(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundedFuzzySetRule Or(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.Or(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundedFuzzySetRule OrNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundRuleExt.OrNot(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundedFuzzySetRule Then(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        UnboundFuzzySetRuleExt.Then(this, linguisticBase, variableName, termName, hedgeType);

    public UnboundedFuzzySetRule If<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.If(this, value);

    public UnboundedFuzzySetRule IfNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.IfNot(this, value);

    public UnboundedFuzzySetRule And<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.And(this, value);

    public UnboundedFuzzySetRule AndNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.AndNot(this, value);

    public UnboundedFuzzySetRule Or<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.Or(this, value);

    public UnboundedFuzzySetRule OrNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
        BooleanRuleExt.OrNot(this, value);
}