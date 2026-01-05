using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Components;
using Shared.Primitives.Implementation;

namespace Reasoning.Base.Abstractions;

public interface IRuleBase<T> where T : class, IRule
{
    ICollection<T> ProductionRules { get; }

    void Add(T rule);

    void AddAll(ICollection<T> rules);

    void AddAll(params IEnumerable<T> rules);

    bool Remove(T rule);

    void RemoveAll(params IEnumerable<T> rules);

    IEnumerable<T> FindByPremise(StringOrType identifier);

    IEnumerable<T> FindByConclusion(string target);

    IReadOnlySet<StringOrType> GetBaseVariables();

    IReadOnlySet<string> GetInferredVariables();

    IReadOnlySet<StringOrType> GetAllVariables();

    IReadOnlySet<Type> GetBooleanVariables();

    IReadOnlySet<string> GetFuzzyVariables();

    IReadOnlySet<StringOrType> FindDependentVariables(string target);

    IReadOnlyDictionary<StringOrType, IReadOnlyList<StringOrType>> BuildDependencyGraph();

    IReadOnlyDictionary<string, IReadOnlyList<T>> BuildRuleDependencyMap();

    IEnumerable<IRule> GetEvaluable(IWorkingMemory memory);

    IEnumerable<T> GetActivated(uint iteration);

    IEnumerable<T> GetUnactivated(uint iteration);

    IEnumerable<T> GetDormant(IWorkingMemory memory, uint iteration);

    IEnumerable<T> GetNeverActivated();

    IEnumerable<T> GetEverActivated();

    void RecomputeAdaptation(AdaptationConfig config);

    void ResetAdaptation();

    ICollection<T> ShallowCopy();

    ICollection<T> DeepCopy(LifecycleMode mode = LifecycleMode.New);
}