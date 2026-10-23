using Reasoning.Base.Abstractions;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Components;
using Reasoning.Rule.FuzzySet.Abstractions;

namespace Reasoning.Base.FuzzySet.Abstractions;

public interface IFuzzySetRuleBase: IRuleBase<IFuzzySetRule>
{
    IEnumerable<IFuzzySetRule> FilterByResolutionMethod(string variableName, IComparer<IRule> ruleComparer);

    new IFuzzySetRuleBase ShallowCopy();

    new IFuzzySetRuleBase DeepCopy(LifecycleMode mode = LifecycleMode.NewInstance);
}