using System.Diagnostics;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Conorm.Implementations.Canonical;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Negation.Implementations.Canonical;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Norm.Implementations.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Extensions;
using Reasoning.Rule.Functional.Components;
using Reasoning.Rule.Functional.Exceptions;
using Reasoning.Rule.Functional.Implementations;
using Reasoning.Rule.Functional.Policy.Abstractions;
using Reasoning.Rule.Functional.Policy.Factory;
using Shared.Options.Factory;
using Shared.Options.Implementations;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Reasoning.Rule.Functional.Abstractions;

public class AbstractFunctionalRule : AbstractRule, IFunctionalRule
{
    public FunctionalRuleState State { get; set; }

    public Option<double> EvaluateConsequentValue(IWorkingMemory memory)
    {
        this.Validate();
        var consequent = (IFunctionalConsequent) Consequent!;
        return consequent.Evaluate(memory);
    }

    public Option<double> EvaluateRuleOutput(IWorkingMemory memory, INegation negation, INorm norm, IConorm conorm)
    {
        if (!IsPremiseEvaluable(memory))
            return Option<double>.None();
        var premiseWeight = EvaluatePremiseWeight(memory, negation, norm, conorm).Get;
        var consequentValue = EvaluateConsequentValue(memory).Get;
        return premiseWeight * consequentValue;
    }

    public Option<double> EvaluateRuleOutput(IWorkingMemory memory, IOperatorFamily family) =>
        EvaluateRuleOutput(memory, family.Negation, family.Norm, family.Conorm);

    public Option<double> EvaluateRuleOutput(IWorkingMemory memory) =>
        EvaluateRuleOutput(memory, Negation.Standard, Norm.Minimum, Conorm.Maximum);

    protected bool MemberwiseEquals(IFunctionalRule? other)
    {
        this.Validate();
        return ReferenceEquals(this, other) ||
               other != null &&
               Equals(Conditional, other.Conditional) &&
               Connectives.SequenceEqual(other.Connectives) &&
               Equals(Consequent, other.Consequent);
    }

    protected int MemberwiseHashCode()
    {
        this.Validate();
        return HashCode.Combine(Conditional, Connectives, Consequent);
    }
}

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

        var coefficientDict = policy.Initialize(this);

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