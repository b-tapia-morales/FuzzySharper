using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Linguistic.Base.Exceptions;
using Reasoning.Proposition.Components;
using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Exceptions;
using Shared.Options.Factory;

namespace Reasoning.Rule.Extensions;

internal static class RuleExt
{
    extension<T>(T rule) where T : class, IRule
    {
        internal bool IsValid() =>
            rule is {Conditional: not null, Consequent: not null};

        internal void Validate()
        {
            if (!rule.IsValid())
                throw new InvalidRuleException();
        }

        public T AddAntecedent(ILinguisticBase linguisticBase, string variableName, Literal literal, LinguisticHedge linguisticHedge, string termName)
        {
            if (rule.IsFinalized)
                throw new FinalizedRuleException();
            if (rule.Conditional != null)
                throw new DuplicatedAntecedentException();

            if (!linguisticBase.GetVariable(variableName).IsSome(out var variable))
                throw new VariableNotFoundException(variableName);
            if (!variable.GetMapping(termName).IsSome(out var membershipFunction))
                throw new EntryNotFoundException(variableName, termName);

            rule.Conditional = new FuzzyProposition(variableName, Connective.If, literal, linguisticHedge, membershipFunction);
            return rule;
        }

        public T AddAntecedent<TEnum>(TEnum value, Literal literal) where TEnum : struct, Enum, IConvertible
        {
            if (rule.IsFinalized)
                throw new FinalizedRuleException();
            if (rule.Conditional != null)
                throw new DuplicatedAntecedentException();

            rule.Conditional = new BooleanProposition<TEnum>(value, Connective.If, literal);
            return rule;
        }

        public T AddConnective(ILinguisticBase linguisticBase, Connective connective, string variableName, Literal literal, LinguisticHedge linguisticHedge, string termName)
        {
            if (rule.IsFinalized)
                throw new FinalizedRuleException();
            if (rule.Conditional == null)
                throw new MissingAntecedentException();

            if (!linguisticBase.GetVariable(variableName).IsSome(out var variable))
                throw new VariableNotFoundException(variableName);
            if (!variable.GetMapping(termName).IsSome(out var membershipFunction))
                throw new EntryNotFoundException(variableName, termName);

            rule.Connectives.Add(new FuzzyProposition(variableName, connective, literal, linguisticHedge, membershipFunction));
            return rule;
        }

        public T AddConnective<TEnum>(TEnum value, Connective connective, Literal literal) where TEnum : struct, Enum, IConvertible
        {
            if (rule.IsFinalized)
                throw new FinalizedRuleException();
            if (rule.Conditional == null)
                throw new MissingAntecedentException();

            rule.Connectives.Add(new BooleanProposition<TEnum>(value, connective, literal));
            return rule;
        }
    }
}