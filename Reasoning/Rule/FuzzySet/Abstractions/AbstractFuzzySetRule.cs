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
using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Extensions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Reasoning.Rule.FuzzySet.Abstractions;

public abstract class AbstractFuzzySetRule : AbstractRule, IFuzzySetRule
{
    public bool IsEvaluable(IWorkingMemory memory)
    {
        this.Validate();
        var consequent = (FuzzySetConsequent) Consequent!;
        var proposition = consequent.Proposition;
        return IsPremiseEvaluable(memory) &&
               proposition.IsEvaluable(memory);
    }

    public Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory,
        INegation negation, INorm norm, IConorm conorm)
    {
        this.Validate();
        var numbers = new Queue<FuzzyNumber>(ApplyUnaryOperators(memory, negation));
        switch (numbers.Count)
        {
            case 0:
                return OptionFactory.None<FuzzyNumber>();
            case 1:
                return numbers.First();
        }

        var connectives = new Queue<Connective>(Connectives.Select(e => e.Connective));
        while (numbers.Count > 1)
        {
            var a = numbers.Dequeue();
            var b = numbers.Dequeue();
            var operation = connectives.Dequeue() == Connective.And ? norm.Intersection(a, b) : conorm.Union(a, b);
            numbers.Enqueue(operation);
        }

        return numbers.Dequeue();
    }

    public Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory, IOperatorFamily operatorFamily) =>
        EvaluatePremiseWeight(memory, operatorFamily.Negation, operatorFamily.Norm, operatorFamily.Conorm);

    public Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory) =>
        EvaluatePremiseWeight(memory, Negation.Standard, Norm.Minimum, Conorm.Maximum);

    public Option<FuzzyNumber> EvaluateConclusionWeight(IWorkingMemory memory)
    {
        this.Validate();
        var consequent = (FuzzySetConsequent) Consequent!;
        var proposition = consequent.Proposition;
        return !memory.GetNumericFact(Consequent!.Target).IsSomeVal(out var value)
            ? OptionFactory.None<FuzzyNumber>()
            : proposition.Evaluate(value);
    }

    public Option<FuzzyNumber> EvaluateRuleWeight(IWorkingMemory memory,
        INegation negation, INorm norm, IConorm conorm, IResiduum residuum) =>
        !IsEvaluable(memory)
            ? OptionFactory.None<FuzzyNumber>()
            : residuum.Implication(EvaluatePremiseWeight(memory, negation, norm, conorm).Get, EvaluateConclusionWeight(memory).Get);

    public Option<FuzzyNumber> EvaluateRuleWeight(IWorkingMemory memory, IOperatorFamily operatorFamily) =>
        EvaluateRuleWeight(memory, operatorFamily.Negation, operatorFamily.Norm, operatorFamily.Conorm, operatorFamily.Residuum);

    public Option<FuzzyNumber> EvaluateRuleWeight(IWorkingMemory memory) =>
        EvaluateRuleWeight(memory, Negation.Standard, Norm.Minimum, Conorm.Maximum, Residuum.Godel);

    /*protected T AddConsequent(ILinguisticBase linguisticBase, string variableName, Literal literal, LinguisticHedge linguisticHedge, string termName)
    {
        if (IsFinalized)
            throw new FinalizedRuleException();
        if (Conditional == null)
            throw new MissingAntecedentException();
        if (literal == Literal.IsNot)
            throw new NegatedConsequentException();

        if (!linguisticBase.GetVariable(variableName).IsSomeRef(out var variable))
            throw new VariableNotFoundException(variableName);
        if (!variable.GetFunction(termName).IsSomeRef(out var membershipFunction))
            throw new EntryNotFoundException(variableName, termName);

        Consequent = new FuzzySetConsequent(new FuzzyProposition(variableName, Connective.Then, literal, linguisticHedge, membershipFunction));
        IsFinalized = true;
        return (T) this;
    }*/
}