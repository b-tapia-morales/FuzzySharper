using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Base.Abstractions;
using Reasoning.Base.Extensions;
using Reasoning.Base.Functional.Abstractions;
using Reasoning.Base.Functional.Extensions;
using Reasoning.Rule.Components;
using Reasoning.Rule.Functional.Abstractions;
using Shared.Options.Implementations;

namespace Reasoning.Base.Functional.Implementations;

public sealed class FunctionalRuleBase : AbstractRuleBase<IFunctionalRule>, IFunctionalRuleBase
{
    private FunctionalRuleBase()
    {
    }

    private FunctionalRuleBase(ICollection<IFunctionalRule> rules)
    {
        foreach (var rule in rules)
            ProductionRules.Add(rule);
    }

    public static FunctionalRuleBase Create() =>
        new();

    public static FunctionalRuleBase Create(params IEnumerable<IFunctionalRule> rules) =>
        new([..rules]);

    public static FunctionalRuleBase Create(ICollection<IFunctionalRule> rules) =>
        new(rules);

    public Option<double> EvaluateTarget(string target, IWorkingMemory memory, IOperatorFamily operatorFamily) =>
        ProductionRules.EvaluateTarget(target, memory, operatorFamily);

    public Option<double> EvaluateTarget(string target, IWorkingMemory memory, INegation negation, INorm norm, IConorm conorm) =>
        ProductionRules.EvaluateTarget(target, memory, negation, norm, conorm);

    public Option<double> EvaluateTarget(string target, IWorkingMemory memory) =>
        ProductionRules.EvaluateTarget(target, memory);

    public override IFunctionalRuleBase ShallowCopy() =>
        Create(ProductionRules);

    public override IFunctionalRuleBase DeepCopy(LifecycleMode mode = LifecycleMode.NewInstance) =>
        Create(ProductionRules.DeepCopy(mode));
    
    IRuleBase<IFunctionalRule> IRuleBase<IFunctionalRule>.ShallowCopy() =>
        ShallowCopy();
    
    IRuleBase<IFunctionalRule> IRuleBase<IFunctionalRule>.DeepCopy(LifecycleMode mode) =>
        DeepCopy(mode);
}