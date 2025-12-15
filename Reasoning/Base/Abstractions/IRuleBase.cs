using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Rule.Abstractions;
using Shared.Primitives.Implementation;

namespace Reasoning.Base.Abstractions;

public interface IRuleBase
{
    ICollection<IFuzzySetRule> ProductionRules { get; }

    void Add(IFuzzySetRule rule);

    void AddAll(ICollection<IFuzzySetRule> rules);

    void AddAll(params IEnumerable<IFuzzySetRule> rules);

    bool Remove(IFuzzySetRule rule);

    void RemoveAll(params IEnumerable<IFuzzySetRule> rules);

    IEnumerable<IFuzzySetRule> FindByPremise(StringOrType variableName);

    IEnumerable<IFuzzySetRule> FindByConclusion(string variableName);

    ISet<StringOrType> GetBaseVariables();

    ISet<string> GetInferredVariables();

    ISet<StringOrType> GetAllVariables();

    ISet<Type> GetBooleanVariables();

    ISet<string> GetFuzzyVariables();

    ISet<StringOrType> FindDependentVariables(string variableName);

    IDictionary<StringOrType, List<StringOrType>> BuildDependencyGraph();

    IDictionary<string, List<IFuzzySetRule>> BuildRuleDependencyMap();

    IEnumerable<IFuzzySetRule> FilterByApplicability(IWorkingMemory memory);

    IEnumerable<IFuzzySetRule> FilterByResolutionMethod(string variableName, IComparer<IFuzzySetRule> ruleComparer);

    IEnumerable<IFuzzySetRule> FilterFacts(IWorkingMemory workingMemory);

    IEnumerable<IFuzzySetRule> FilterCircularDependencies(string variableName);

    public void UpdateLearning(AdaptationConfig config);

    public void ResetLearning();

    IRuleBase DeepCopy();
}