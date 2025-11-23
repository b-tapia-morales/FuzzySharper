using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Rule.Abstractions;
using Shared.Primitives.Implementation;

namespace Reasoning.Base.Abstractions;

public interface IRuleBase
{
    ICollection<IRule> ProductionRules { get; }

    void Add(IRule rule);

    void AddAll(ICollection<IRule> rules);

    void AddAll(params IEnumerable<IRule> rules);

    bool Remove(IRule rule);

    void RemoveAll(params IEnumerable<IRule> rules);

    IEnumerable<IRule> FindByPremise(StringOrType variableName);

    IEnumerable<IRule> FindByConclusion(string variableName);

    ISet<StringOrType> GetBaseVariables();

    ISet<string> GetInferredVariables();

    ISet<StringOrType> GetAllVariables();

    ISet<Type> GetBooleanVariables();

    ISet<string> GetFuzzyVariables();

    ISet<StringOrType> FindDependentVariables(string variableName);

    IDictionary<StringOrType, List<StringOrType>> BuildDependencyGraph();

    IDictionary<string, List<IRule>> BuildRuleDependencyMap();

    IEnumerable<IRule> FilterByApplicability(IWorkingMemory memory);

    IEnumerable<IRule> FilterByResolutionMethod(string variableName, IComparer<IRule> ruleComparer);

    IEnumerable<IRule> FilterFacts(IWorkingMemory workingMemory);

    IEnumerable<IRule> FilterCircularDependencies(string variableName);

    public void UpdateLearning(AdaptationConfig config);

    public void ResetLearning();

    IRuleBase DeepCopy();
}