using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Functional.Components;
using Reasoning.Rule.Functional.Policy.Abstractions;
using Reasoning.Rule.Functional.Policy.Factory;
using Shared.Options.Implementations;

namespace Reasoning.Rule.Functional.Abstractions;

public interface IFunctionalRule : IRule
{
    FunctionalRuleState State { get; set; }
    
    Option<double> EvaluateConsequentValue(IWorkingMemory memory);

    Option<double> EvaluateRuleOutput(IWorkingMemory memory, INegation negation, INorm norm, IConorm conorm);

    Option<double> EvaluateRuleOutput(IWorkingMemory memory, IOperatorFamily family);
    
    Option<double> EvaluateRuleOutput(IWorkingMemory memory);
}

public interface IFunctionalRule<out T> : IFunctionalRule where T : class, IFunctionalRule<T>
{
    T Then(string target, IReadOnlyList<double> coefficients, double bias, FunctionalRuleState state = FunctionalRuleState.Frozen);

    T Then(string target, CoefficientInitMethod method, FunctionalRuleState state = FunctionalRuleState.Initialized);

    T Then(string target, ICoefficientInitPolicy policy, FunctionalRuleState state = FunctionalRuleState.Initialized);
}