using System.Diagnostics;
using Reasoning.Rule.Functional.Components;
using Reasoning.Rule.Functional.Exceptions;
using Reasoning.Rule.Functional.Implementations;
using Reasoning.Rule.Functional.Policy.Abstractions;
using Reasoning.Rule.Functional.Policy.Factory;

namespace Reasoning.Rule.Functional.Abstractions;

public abstract class AbstractFunctionalRule<T> : AbstractFunctionalRule, IFunctionalRule<T> where T : AbstractFunctionalRule<T>
{
    public T Then(string target, IReadOnlyList<double> coefficients, double bias, FunctionalRuleState state = FunctionalRuleState.Frozen)
    {
        var variables = PremiseVariables.Where(v => v.IsString).Select(v => v.AsString).ToList();
        if (coefficients.Count != variables.Count)
            throw new CoefficientArityMismatchException(variables.Count, coefficients.Count);

        var coefficientDict = variables.Zip(coefficients, (v, c) => (Variable: v, Coefficient: c)).ToDictionary(t => t.Variable, t => t.Coefficient);

        FinalizeConsequent(target, coefficientDict, state);
        return (T) this;
    }

    public T Then(string target, CoefficientInitMethod method, FunctionalRuleState state = FunctionalRuleState.Initialized) =>
        Then(target, CoefficientInitFactory.GetInstance(method), state);

    public T Then(string target, ICoefficientInitPolicy policy, FunctionalRuleState state = FunctionalRuleState.Initialized)
    {
        var variables = PremiseVariables.Where(v => v.IsString).Select(v => v.AsString).ToList();

        var coefficientDict = policy.InitializeFromRule(this);

        Debug.Assert(coefficientDict.Count == variables.Count);

        FinalizeConsequent(target, coefficientDict, state);
        return (T) this;
    }

    private void FinalizeConsequent(string target, IReadOnlyDictionary<string, double> coefficientDict, FunctionalRuleState state)
    {
        Consequent = new FunctionalConsequent
        {
            Target = target,
            CoefficientDict = coefficientDict,
            Arity = (uint) coefficientDict.Count
        };

        State = state;
        IsFinalized = true;
    }
}