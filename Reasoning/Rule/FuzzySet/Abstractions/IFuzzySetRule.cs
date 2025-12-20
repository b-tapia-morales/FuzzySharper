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
    bool IsEvaluable(IWorkingMemory memory);

    Option<FuzzyNumber> EvaluateConclusionWeight(IWorkingMemory memory);

    Option<FuzzyNumber> EvaluateRuleWeight(IWorkingMemory memory,
        INegation negation, INorm norm, IConorm conorm, IResiduum residuum);

    Option<FuzzyNumber> EvaluateRuleWeight(IWorkingMemory memory, IOperatorFamily operatorFamily);

    Option<FuzzyNumber> EvaluateRuleWeight(IWorkingMemory memory);
}