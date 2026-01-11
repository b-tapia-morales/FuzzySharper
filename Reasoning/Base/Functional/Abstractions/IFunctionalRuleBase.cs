using Reasoning.Base.Abstractions;
using Reasoning.Rule.Components;
using Reasoning.Rule.Functional.Abstractions;

namespace Reasoning.Base.Functional.Abstractions;

public interface IFunctionalRuleBase : IRuleBase<IFunctionalRule>
{
    new IFunctionalRuleBase ShallowCopy();
    
    new IFunctionalRuleBase DeepCopy(LifecycleMode mode = LifecycleMode.NewInstance);
}