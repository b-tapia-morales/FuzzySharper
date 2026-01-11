using Reasoning.Base.Abstractions;
using Reasoning.Base.Extensions;
using Reasoning.Base.Functional.Abstractions;
using Reasoning.Rule.Components;
using Reasoning.Rule.Functional.Abstractions;

namespace Reasoning.Base.Functional.Implementations;

public sealed class FunctionalRuleBase : AbstractRuleBase<IFunctionalRule>, IFunctionalRuleBase
{
    private FunctionalRuleBase()
    {
    }

    private FunctionalRuleBase(ICollection<IFunctionalRule> rules)
    {
        foreach (var rule in rules)
            ProductionRules.Add(rule);
    }

    public static FunctionalRuleBase Create() =>
        new();

    public static FunctionalRuleBase Create(params IEnumerable<IFunctionalRule> rules) =>
        new(rules.ToList());

    public static FunctionalRuleBase Create(ICollection<IFunctionalRule> rules) =>
        new(rules);

    public override IFunctionalRuleBase ShallowCopy() =>
        Create(ProductionRules);

    public override IFunctionalRuleBase DeepCopy(LifecycleMode mode = LifecycleMode.NewInstance) =>
        Create(ProductionRules.DeepCopy(mode));
    
    IRuleBase<IFunctionalRule> IRuleBase<IFunctionalRule>.ShallowCopy() =>
        Create(ProductionRules);
    
    IRuleBase<IFunctionalRule> IRuleBase<IFunctionalRule>.DeepCopy(LifecycleMode mode) =>
        Create(ProductionRules.DeepCopy(mode));
}