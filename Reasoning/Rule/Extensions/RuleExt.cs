using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Linguistic.Base.Exceptions;
using Reasoning.Proposition.Components;
using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Exceptions;
using Shared.Options.Factory;

namespace Reasoning.Rule.Extensions;

public static class RuleExt
{
    extension(IFuzzySetRule rule)
    {
        internal bool IsValid() =>
            rule is {Conditional: not null, Consequent: not null};

        internal void Validate()
        {
            if (!rule.IsValid())
                throw new InvalidRuleException();
        }

        public IFuzzySetRule AddAntecedent(ILinguisticBase linguisticBase, string variableName, Literal literal, LinguisticHedge linguisticHedge, string termName)
        {
            if (rule.IsFinalized)
                throw new FinalizedRuleException();
            if (rule.Conditional != null)
                throw new DuplicatedAntecedentException();

            if (!linguisticBase.GetVariable(variableName).IsSomeRef(out var variable))
                throw new VariableNotFoundException(variableName);
            if (!variable.GetFunction(termName).IsSomeRef(out var membershipFunction))
                throw new EntryNotFoundException(variableName, termName);

            rule.Conditional = new FuzzyProposition(variableName, Connective.If, literal, linguisticHedge, membershipFunction);
            return rule;
        }

        public IFuzzySetRule AddAntecedent<T>(T value, Literal literal) where T : struct, Enum, IConvertible
        {
            if (rule.IsFinalized)
                throw new FinalizedRuleException();
            if (rule.Conditional != null)
                throw new DuplicatedAntecedentException();

            rule.Conditional = new BooleanProposition<T>(value, Connective.If, literal);
            return rule;
        }

        public IFuzzySetRule AddConnective(ILinguisticBase linguisticBase, Connective connective, string variableName, Literal literal, LinguisticHedge linguisticHedge, string termName)
        {
            if (rule.IsFinalized)
                throw new FinalizedRuleException();
            if (rule.Conditional == null)
                throw new MissingAntecedentException();

            if (!linguisticBase.GetVariable(variableName).IsSomeRef(out var variable))
                throw new VariableNotFoundException(variableName);
            if (!variable.GetFunction(termName).IsSomeRef(out var membershipFunction))
                throw new EntryNotFoundException(variableName, termName);

            rule.Connectives.Add(new FuzzyProposition(variableName, connective, literal, linguisticHedge, membershipFunction));
            return rule;
        }

        public IFuzzySetRule AddConnective<T>(T value, Connective connective, Literal literal) where T : struct, Enum, IConvertible
        {
            if (rule.IsFinalized)
                throw new FinalizedRuleException();
            if (rule.Conditional == null)
                throw new MissingAntecedentException();

            rule.Connectives.Add(new BooleanProposition<T>(value, connective, literal));
            return rule;
        }

        public IFuzzySetRule AddConsequent(ILinguisticBase linguisticBase, string variableName, Literal literal, LinguisticHedge linguisticHedge, string termName)
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

            rule.Consequent = new FuzzyProposition(variableName, Connective.Then, literal, linguisticHedge, membershipFunction);
            rule.IsFinalized = true;
            return rule;
        }
    }
}