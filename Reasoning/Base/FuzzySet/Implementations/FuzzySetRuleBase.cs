using Knowledge.Memory.Abstractions;
using Reasoning.Base.Abstractions;
using Reasoning.Base.Extensions;
using Reasoning.Base.FuzzySet.Abstractions;
using Reasoning.Base.FuzzySet.Extensions;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Components;
using Reasoning.Rule.FuzzySet.Abstractions;

namespace Reasoning.Base.FuzzySet.Implementations;

public class FuzzySetRuleBase : AbstractRuleBase<IFuzzySetRule>, IFuzzySetRuleBase
{
    private FuzzySetRuleBase()
    {
    }

    private FuzzySetRuleBase(ICollection<IFuzzySetRule> rules)
    {
        foreach (var rule in rules)
            ProductionRules.Add(rule);
    }

    public static FuzzySetRuleBase Create() =>
        new();

    public static FuzzySetRuleBase Create(params IEnumerable<IFuzzySetRule> rules) =>
        new(rules.ToList());

    public static FuzzySetRuleBase Create(ICollection<IFuzzySetRule> rules) =>
        new(rules);


    public IEnumerable<IFuzzySetRule> FilterByResolutionMethod(string variableName, IComparer<IRule> ruleComparer) =>
        ProductionRules.FilterByResolutionMethod(variableName, ruleComparer);

    public IEnumerable<IFuzzySetRule> FilterFacts(IWorkingMemory workingMemory) =>
        ProductionRules.FilterFacts(workingMemory);

    public IEnumerable<IFuzzySetRule> FilterCircularDependencies(string variableName) =>
        ProductionRules.FilterCircularDependencies(variableName);

    public override IFuzzySetRuleBase ShallowCopy() => 
        Create(ProductionRules);

    public override IFuzzySetRuleBase DeepCopy(LifecycleMode mode = LifecycleMode.NewInstance) => 
        Create(ProductionRules);

    IRuleBase<IFuzzySetRule> IRuleBase<IFuzzySetRule>.ShallowCopy() =>
        Create(ProductionRules);

    IRuleBase<IFuzzySetRule> IRuleBase<IFuzzySetRule>.DeepCopy(LifecycleMode mode) =>
        Create(ProductionRules.DeepCopy(mode));
}