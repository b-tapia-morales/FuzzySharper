using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Base.Exceptions;
using Reasoning.Base.Extensions;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Components;
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

    public IEnumerable<T> FindByPremise(StringOrType identifier) =>
        ProductionRules.FindByPremise(identifier);

    public IEnumerable<T> FindByConclusion(string target) =>
        ProductionRules.FindByConclusion(target);

    public IReadOnlySet<StringOrType> GetBaseVariables() =>
        ProductionRules.GetBaseVariables();

    public IReadOnlySet<string> GetInferredVariables() =>
        ProductionRules.GetInferredVariables();

    public IReadOnlySet<StringOrType> GetAllVariables() =>
        ProductionRules.GetAllVariables();

    public IReadOnlySet<Type> GetBooleanVariables() =>
        ProductionRules.GetBooleanVariables();

    public IReadOnlySet<string> GetFuzzyVariables() =>
        ProductionRules.GetFuzzyVariables();

    public IReadOnlySet<StringOrType> FindDependentVariables(string target) =>
        ProductionRules.FindDependentVariables(target);

    public IReadOnlyDictionary<StringOrType, IReadOnlyList<StringOrType>> BuildDependencyGraph() =>
        ProductionRules.BuildDependencyGraph();

    public IReadOnlyDictionary<string, IReadOnlyList<T>> BuildRuleDependencyMap() =>
        ProductionRules.BuildRuleDependencyMap();

    public IEnumerable<IRule> GetEvaluable(IWorkingMemory memory) =>
        ProductionRules.GetEvaluable(memory);

    public IEnumerable<T> GetActivated(uint iteration) =>
        ProductionRules.GetActivated(iteration);

    public IEnumerable<T> GetUnactivated(uint iteration) =>
        ProductionRules.GetUnactivated(iteration);

    public IEnumerable<T> GetDormant(IWorkingMemory memory, uint iteration) =>
        ProductionRules.GetDormant(memory, iteration);

    public IEnumerable<T> GetNeverActivated() =>
        ProductionRules.GetNeverActivated();

    public IEnumerable<T> GetEverActivated() =>
        ProductionRules.GetEverActivated();

    public void RecomputeAdaptation(AdaptationConfig config) =>
        ProductionRules.RecomputeAdaptation(config);

    public void ResetAdaptation() =>
        ProductionRules.ResetAdaptation();

    public abstract IRuleBase<T> ShallowCopy();

    public abstract IRuleBase<T> DeepCopy(LifecycleMode mode = LifecycleMode.NewInstance);
}