using Kernel.Number;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Conorm.Implementations.Canonical;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Negation.Implementations.Canonical;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Norm.Implementations.Canonical;
using Kernel.Operator.Residuum.Abstractions;
using Kernel.Operator.Residuum.Implementations;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Components;
using Reasoning.Rule.Extensions;
using Reasoning.Rule.FuzzySet.Components;
using Shared.Options.Factory;
using Shared.Options.Implementations;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Reasoning.Rule.FuzzySet.Abstractions;

public abstract class AbstractFuzzySetRule : AbstractRule, IFuzzySetRule
{
    public bool IsEvaluable(IWorkingMemory memory)
    {
        this.Validate();
        var consequent = (FuzzySetConsequent) Consequent!;
        return IsPremiseEvaluable(memory) &&
               consequent.IsEvaluable(memory);
    }

    public Option<FuzzyNumber> EvaluateConclusionWeight(IWorkingMemory memory)
    {
        this.Validate();
        var consequent = (FuzzySetConsequent) Consequent!;
        return !memory.GetNumericFact(consequent.Target).IsSome(out var value)
            ? Option<FuzzyNumber>.None()
            : consequent.Evaluate(value);
    }

    public Option<FuzzyNumber> EvaluateRuleWeight(IWorkingMemory memory,
        INegation negation, INorm norm, IConorm conorm, IResiduum residuum) =>
        !IsEvaluable(memory)
            ? Option<FuzzyNumber>.None()
            : residuum.Implication(EvaluatePremiseWeight(memory, negation, norm, conorm).Get, EvaluateConclusionWeight(memory).Get);

    public Option<FuzzyNumber> EvaluateRuleWeight(IWorkingMemory memory, IOperatorFamily operatorFamily) =>
        EvaluateRuleWeight(memory, operatorFamily.Negation, operatorFamily.Norm, operatorFamily.Conorm, operatorFamily.Residuum);

    public Option<FuzzyNumber> EvaluateRuleWeight(IWorkingMemory memory) =>
        EvaluateRuleWeight(memory, Negation.Standard, Norm.Minimum, Conorm.Maximum, Residuum.Godel);

    public abstract override IFuzzySetRule DeepCopy(LifecycleMode mode = LifecycleMode.New);

    IRule IRule.DeepCopy(LifecycleMode mode) => 
        DeepCopy(mode);

    protected bool MemberwiseEquals(IFuzzySetRule? other)
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