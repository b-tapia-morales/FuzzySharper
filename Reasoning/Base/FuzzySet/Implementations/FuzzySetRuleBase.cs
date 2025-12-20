using Knowledge.Memory.Abstractions;
using Reasoning.Base.Abstractions;
using Reasoning.Base.FuzzySet.Abstractions;
using Reasoning.Base.FuzzySet.Extensions;
using Reasoning.Rule.Abstractions;
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
    
    public IFuzzySetRuleBase DeepCopy() => 
        Create(ProductionRules);

    public IEnumerable<IFuzzySetRule> FilterByResolutionMethod(string variableName, IComparer<IRule> ruleComparer) =>
        ProductionRules.FilterByResolutionMethod(variableName, ruleComparer);

    public IEnumerable<IFuzzySetRule> FilterFacts(IWorkingMemory workingMemory) =>
        ProductionRules.FilterFacts(workingMemory);

    public IEnumerable<IFuzzySetRule> FilterCircularDependencies(string variableName) =>
        ProductionRules.FilterCircularDependencies(variableName);
}