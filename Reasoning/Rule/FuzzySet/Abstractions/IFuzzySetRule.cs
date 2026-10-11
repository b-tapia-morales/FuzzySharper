using Kernel.Number;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Residuum.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic;
using Shared.Options.Implementations;

namespace Reasoning.Rule.FuzzySet.Abstractions;

public interface IFuzzySetRule : IRule
{
    /// <summary>
    /// Gets the optional priority level assigned to the rule, representing its relative importance among competing
    /// rules during inference.
    /// </summary>
    Option<RulePriority> Priority { get; }

    /// <summary>
    /// Gets the optional certainty factor associated with the rule, representing the level of confidence an expert has
    /// in its correctness and suitability for use among competing rules during inference.
    /// </summary>
    Option<double> CertaintyFactor { get; }

    bool IsRuleEvaluable(IWorkingMemory memory);

    Option<FuzzyNumber> EvaluateConclusionWeight(IWorkingMemory memory);

    Option<FuzzyNumber> EvaluateRuleStrength(IWorkingMemory memory,
        INegation negation, INorm norm, IConorm conorm, IResiduum residuum);

    Option<FuzzyNumber> EvaluateRuleStrength(IWorkingMemory memory, IOperatorFamily operatorFamily);

    Option<FuzzyNumber> EvaluateRuleStrength(IWorkingMemory memory);
}