using Reasoning.Rule.Functional.Components;
using Reasoning.Rule.Functional.Policy.Abstractions;
using Reasoning.Rule.Functional.Policy.Factory;

namespace Reasoning.Rule.Functional.Abstractions;

public interface IFunctionalRule<out T> : IFunctionalRule where T : class, IFunctionalRule<T>
{
    T Then(string target, IReadOnlyList<double> coefficients, double bias, FunctionalRuleState state = FunctionalRuleState.Frozen);

    T Then(string target, CoefficientInitMethod method, FunctionalRuleState state = FunctionalRuleState.Initialized);

    T Then(string target, ICoefficientInitPolicy policy, FunctionalRuleState state = FunctionalRuleState.Initialized);
}