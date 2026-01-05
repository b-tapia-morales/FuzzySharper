using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Linguistic.Base.Exceptions;
using Reasoning.Proposition.Abstractions;
using Reasoning.Proposition.Components;
using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Exceptions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Components;
using Reasoning.Rule.FuzzySet.Exceptions;
using Shared.Options.Factory;

namespace Reasoning.Rule.FuzzySet.Extensions;

internal static class FuzzySetRuleExt
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

            List<IProposition> premise = [rule.Conditional, ..rule.Connectives];
            if (premise.Any(p => p is FuzzyProposition prop && string.Equals(prop.Identifier.AsString, variableName, StringComparison.OrdinalIgnoreCase)))
                throw new VariableOverlapException(variableName);

            if (!linguisticBase.GetVariable(variableName).IsSome(out var variable))
                throw new VariableNotFoundException(variableName);
            if (!variable.GetMapping(termName).IsSome(out var membershipFunction))
                throw new EntryNotFoundException(variableName, termName);

            rule.Consequent = new FuzzySetConsequent(new FuzzyProposition(variableName, Connective.Then, literal, linguisticHedge, membershipFunction));
            rule.IsFinalized = true;
            return rule;
        }
    }
}