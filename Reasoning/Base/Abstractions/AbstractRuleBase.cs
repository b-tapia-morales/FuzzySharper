using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Base.Exceptions;
using Reasoning.Base.Extensions;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Extensions;
using Shared.Primitives.Implementation;

namespace Reasoning.Base.Abstractions;

public abstract class AbstractRuleBase<T> : IRuleBase<T> where T : class, IRule
{
    public ICollection<T> ProductionRules { get; } = new List<T>();

    public void Add(T rule)
    {
        if (!rule.IsValid())
            throw new InvalidRuleException();
        ProductionRules.Add(rule);
    }

    public void AddAll(ICollection<T> rules)
    {
        if (rules.Any(e => !e.IsValid()))
            throw new InvalidRuleException();
        foreach (var rule in rules)
            ProductionRules.Add(rule);
    }

    public void AddAll(params IEnumerable<T> rules) =>
        AddAll(rules.ToList());

    public bool Remove(T rule) =>
        ProductionRules.Remove(rule);

    public void RemoveAll(params IEnumerable<T> rules)
    {
        foreach (var rule in rules)
            ProductionRules.Remove(rule);
    }

    public IEnumerable<T> FindByPremise(StringOrType variableName) => 
        ProductionRules.FindByPremise(variableName);

    public IEnumerable<T> FindByConclusion(string variableName) => 
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
    
    public IDictionary<string, List<T>> BuildRuleDependencyMap() =>
        ProductionRules.BuildRuleDependencyMap();

    public IEnumerable<IRule> GetEvaluable(IWorkingMemory memory) =>
        ProductionRules.GetEvaluable(memory);

    public void UpdateLearning(AdaptationConfig config) =>
        ProductionRules.RecomputeAdaptation(config);

    public void ResetLearning() =>
        ProductionRules.ResetAdaptation();
}