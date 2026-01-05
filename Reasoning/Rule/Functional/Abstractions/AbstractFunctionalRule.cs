using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Conorm.Implementations.Canonical;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Negation.Implementations.Canonical;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Norm.Implementations.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Components;
using Reasoning.Rule.Extensions;
using Reasoning.Rule.Functional.Components;
using Shared.Options.Factory;
using Shared.Options.Implementations;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Reasoning.Rule.Functional.Abstractions;

public abstract class AbstractFunctionalRule : AbstractRule, IFunctionalRule
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

    public override abstract IFunctionalRule DeepCopy(LifecycleMode mode = LifecycleMode.New);

    IRule IRule.DeepCopy(LifecycleMode mode) =>
        DeepCopy(mode);

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