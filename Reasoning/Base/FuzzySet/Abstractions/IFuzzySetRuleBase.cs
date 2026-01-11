using Knowledge.Memory.Abstractions;
using Reasoning.Base.Abstractions;
using Reasoning.Base.Extensions;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Components;
using Reasoning.Rule.FuzzySet.Abstractions;

namespace Reasoning.Base.FuzzySet.Abstractions;

public interface IFuzzySetRuleBase: IRuleBase<IFuzzySetRule>
{
    IEnumerable<IFuzzySetRule> FilterByResolutionMethod(string variableName, IComparer<IRule> ruleComparer);

    IEnumerable<IFuzzySetRule> FilterFacts(IWorkingMemory workingMemory);

    IEnumerable<IFuzzySetRule> FilterCircularDependencies(string variableName);

    new IFuzzySetRuleBase ShallowCopy();

    new IFuzzySetRuleBase DeepCopy(LifecycleMode mode = LifecycleMode.NewInstance);
}