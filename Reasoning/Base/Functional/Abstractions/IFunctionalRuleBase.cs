using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Base.Abstractions;
using Reasoning.Rule.Components;
using Reasoning.Rule.Functional.Abstractions;
using Shared.Options.Implementations;

namespace Reasoning.Base.Functional.Abstractions;

public interface IFunctionalRuleBase : IRuleBase<IFunctionalRule>
{
    Option<double> EvaluateTarget(string target, IWorkingMemory memory, IOperatorFamily operatorFamily);
    
    Option<double> EvaluateTarget(string target, IWorkingMemory memory, INegation negation, INorm norm, IConorm conorm);

    Option<double> EvaluateTarget(string target, IWorkingMemory memory);
    
    new IFunctionalRuleBase ShallowCopy();
    
    new IFunctionalRuleBase DeepCopy(LifecycleMode mode = LifecycleMode.NewInstance);
}