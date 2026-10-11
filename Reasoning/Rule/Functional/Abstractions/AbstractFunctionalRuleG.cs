using System.Diagnostics;
using Reasoning.Rule.Functional.Components;
using Reasoning.Rule.Functional.Exceptions;
using Reasoning.Rule.Functional.Implementations;
using Reasoning.Rule.Functional.Policy.Abstractions;
using Reasoning.Rule.Functional.Policy.Factory;

namespace Reasoning.Rule.Functional.Abstractions;

public abstract class AbstractFunctionalRule<T> : AbstractFunctionalRule, IFunctionalRule<T> where T : AbstractFunctionalRule<T>
{
    public T Then(string target, IReadOnlyDictionary<string, double> coefficientDict, double bias, FunctionalRuleState state = FunctionalRuleState.Frozen)
    {
        var dict = coefficientDict.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase);
        var set = PremiseVariables.Where(v => v.IsString).Select(v => v.AsString).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingKeys = set.Except(dict.Keys, StringComparer.OrdinalIgnoreCase).ToList();
        var unexpectedKeys = dict.Keys.Except(set, StringComparer.OrdinalIgnoreCase).ToList();
        if (missingKeys.Count > 0 || unexpectedKeys.Count > 0)
            throw new VariableMismatchException(missingKeys, unexpectedKeys);

        var normalizedDict = set.ToDictionary(k => k, k => coefficientDict[k], StringComparer.OrdinalIgnoreCase);

        FinalizeConsequent(target, normalizedDict, state, bias);
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

    private void FinalizeConsequent(string target, IReadOnlyDictionary<string, double> coefficientDict, FunctionalRuleState state, double bias = 0)
    {
        Consequent = new FunctionalConsequent
        {
            Identifier = target,
            CoefficientDict = coefficientDict,
            Arity = (uint) coefficientDict.Count,
            Bias = bias
        };

        State = state;
        IsFinalized = true;
    }
}