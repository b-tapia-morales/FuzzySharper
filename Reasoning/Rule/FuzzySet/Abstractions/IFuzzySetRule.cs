using Kernel.Number;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Residuum.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Abstractions;
using Shared.Options.Implementations;

namespace Reasoning.Rule.FuzzySet.Abstractions;

public interface IFuzzySetRule : IRule
{
    public bool IsEvaluable(IWorkingMemory memory);

    public Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory,
        INegation negation, INorm norm, IConorm conorm);

    public Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory, IOperatorFamily operatorFamily);

    public Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory);

    public Option<FuzzyNumber> EvaluateConclusionWeight(IWorkingMemory memory);

    public Option<FuzzyNumber> EvaluateRuleWeight(IWorkingMemory memory,
        INegation negation, INorm norm, IConorm conorm, IResiduum residuum);

    public Option<FuzzyNumber> EvaluateRuleWeight(IWorkingMemory memory, IOperatorFamily operatorFamily);

    Option<FuzzyNumber> EvaluateRuleWeight(IWorkingMemory memory);
}