using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Rule.Abstractions;
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

    IEnumerable<T> FindByPremise(StringOrType variableName);

    IEnumerable<T> FindByConclusion(string variableName);

    ISet<StringOrType> GetBaseVariables();

    ISet<string> GetInferredVariables();

    ISet<StringOrType> GetAllVariables();

    ISet<Type> GetBooleanVariables();

    ISet<string> GetFuzzyVariables();

    ISet<StringOrType> FindDependentVariables(string variableName);

    IDictionary<StringOrType, List<StringOrType>> BuildDependencyGraph();

    IDictionary<string, List<T>> BuildRuleDependencyMap();

    IEnumerable<IRule> FilterByApplicability(IWorkingMemory memory);

    public void UpdateLearning(AdaptationConfig config);

    public void ResetLearning();
}