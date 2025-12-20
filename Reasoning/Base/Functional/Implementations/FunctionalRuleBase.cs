using Reasoning.Base.Abstractions;
using Reasoning.Base.Functional.Abstractions;
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
    
    public IFunctionalRuleBase DeepCopy() => 
        Create(ProductionRules);
}