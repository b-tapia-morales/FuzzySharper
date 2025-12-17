using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Linguistic.Base.Exceptions;
using Reasoning.Proposition.Components;
using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Exceptions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Shared.Options.Factory;

namespace Reasoning.Rule.FuzzySet.Extensions;

public static class FuzzySetRuleExt
{
    extension<T>(T rule) where T : class, IFuzzySetRule
    {
        public T AddConsequent(ILinguisticBase linguisticBase, string variableName, Literal literal, LinguisticHedge linguisticHedge, string termName)
        {
            if (rule.IsFinalized)
                throw new FinalizedRuleException();
            if (rule.Conditional == null)
                throw new MissingAntecedentException();
            if (literal == Literal.IsNot)
                throw new NegatedConsequentException();

            if (!linguisticBase.GetVariable(variableName).IsSomeRef(out var variable))
                throw new VariableNotFoundException(variableName);
            if (!variable.GetFunction(termName).IsSomeRef(out var membershipFunction))
                throw new EntryNotFoundException(variableName, termName);

            rule.Consequent = new FuzzySetConsequent(new FuzzyProposition(variableName, Connective.Then, literal, linguisticHedge, membershipFunction));
            rule.IsFinalized = true;
            return rule;
        }
    }
}