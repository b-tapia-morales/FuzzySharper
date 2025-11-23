using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Base.Abstractions;
using Reasoning.Base.Exceptions;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Extensions;
using Shared.Primitives.Implementation;

namespace Reasoning.Base.Implementations;

public class RuleBase : IRuleBase
{
    private RuleBase(params IEnumerable<IRule> rules)
    {
        foreach (var rule in rules)
            ProductionRules.Add(rule);
    }

    public ICollection<IRule> ProductionRules { get; } = new List<IRule>();

    public static IRuleBase Create() =>
        new RuleBase();

    public static IRuleBase Create(params IEnumerable<IRule> rules) =>
        new RuleBase(rules);

    public void Add(IRule rule)
    {
        if (!rule.IsValid())
            throw new InvalidRuleException();
        ProductionRules.Add(rule);
    }

    public void AddAll(ICollection<IRule> rules)
    {
        if (rules.Any(e => !e.IsValid()))
            throw new InvalidRuleException();
        foreach (var rule in rules)
            ProductionRules.Add(rule);
    }

    public void AddAll(params IEnumerable<IRule> rules) =>
        AddAll(rules.ToList());

    public bool Remove(IRule rule) =>
        ProductionRules.Remove(rule);

    public void RemoveAll(params IEnumerable<IRule> rules)
    {
        foreach (var rule in rules)
            ProductionRules.Remove(rule);
    }

    public IEnumerable<IRule> FindByPremise(StringOrType variableName) =>
        ProductionRules.FindByPremise(variableName);

    public IEnumerable<IRule> FindByConclusion(string variableName) =>
        ProductionRules.FindByConclusion(variableName);

    public ISet<StringOrType> GetBaseVariables() =>
        ProductionRules.GetBaseVariables();

    public ISet<string> GetInferredVariables() =>
        ProductionRules.GetInferredVariables();

    public ISet<StringOrType> GetAllVariables() =>
        ProductionRules.GetAllVariables();

    public ISet<Type> GetBooleanVariables() =>
        ProductionRules.GetBooleanVariables();

    public ISet<string> GetFuzzyVariables() =>
        ProductionRules.GetFuzzyVariables();

    public ISet<StringOrType> FindDependentVariables(string variableName) =>
        ProductionRules.FindDependentVariables(variableName);

    public IDictionary<StringOrType, List<StringOrType>> BuildDependencyGraph() =>
        ProductionRules.BuildDependencyGraph();

    public IDictionary<string, List<IRule>> BuildRuleDependencyMap() =>
        ProductionRules.BuildRuleDependencyMap();

    public IEnumerable<IRule> FilterByApplicability(IWorkingMemory memory) =>
        ProductionRules.GetEvaluable(memory);

    public IEnumerable<IRule> FilterByResolutionMethod(string variableName, IComparer<IRule> ruleComparer) =>
        ProductionRules.FilterByResolutionMethod(variableName, ruleComparer);

    public IEnumerable<IRule> FilterFacts(IWorkingMemory workingMemory) =>
        ProductionRules.FilterFacts(workingMemory);

    public IEnumerable<IRule> FilterCircularDependencies(string variableName) =>
        ProductionRules.FilterCircularDependencies(variableName);

    public void UpdateLearning(AdaptationConfig config) =>
        ProductionRules.UpdateLearning(config);

    public void ResetLearning() =>
        ProductionRules.ResetLearning();

    public IRuleBase DeepCopy() =>
        new RuleBase(ProductionRules);
}