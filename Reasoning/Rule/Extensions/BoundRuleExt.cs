using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Exceptions;

namespace Reasoning.Rule.Extensions;

public static class BoundRuleExt
{
    extension<T>(T rule) where T : class, IContextBoundRule<T>
    {
        internal void ValidateConflictingVariable(ILinguisticBase otherBase, string variableName)
        {
            if (rule.LinguisticBase.ContainsVariable(variableName) && otherBase.ContainsVariable(variableName))
                throw new ConflictingVariableException(variableName);
        }

        public T If(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None)
        {
            rule.ValidateConflictingVariable(linguisticBase, variableName);
            return rule.AddAntecedent(linguisticBase, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);
        }

        public T If(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
            rule.AddAntecedent(rule.LinguisticBase, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);

        public T IfNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None)
        {
            rule.ValidateConflictingVariable(linguisticBase, variableName);
            return rule.AddAntecedent(linguisticBase, variableName, Literal.IsNot, LinguisticHedge.ToValue(hedgeType), termName);
        }

        public T IfNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
            rule.AddAntecedent(rule.LinguisticBase, variableName, Literal.IsNot, LinguisticHedge.ToValue(hedgeType), termName);

        public T And(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None)
        {
            rule.ValidateConflictingVariable(linguisticBase, variableName);
            return rule.AddConnective(linguisticBase, Connective.And, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);
        }

        public T And(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
            rule.AddConnective(rule.LinguisticBase, Connective.And, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);

        public T AndNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None)
        {
            rule.ValidateConflictingVariable(linguisticBase, variableName);
            return rule.AddConnective(linguisticBase, Connective.And, variableName, Literal.IsNot, LinguisticHedge.ToValue(hedgeType), termName);
        }

        public T AndNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
            rule.AddConnective(rule.LinguisticBase, Connective.And, variableName, Literal.IsNot, LinguisticHedge.ToValue(hedgeType), termName);

        public T Or(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None)
        {
            rule.ValidateConflictingVariable(linguisticBase, variableName);
            return rule.AddConnective(linguisticBase, Connective.Or, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);
        }

        public T Or(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
            rule.AddConnective(rule.LinguisticBase, Connective.Or, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);

        public T OrNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None)
        {
            rule.ValidateConflictingVariable(linguisticBase, variableName);
            return rule.AddConnective(linguisticBase, Connective.Or, variableName, Literal.IsNot, LinguisticHedge.ToValue(hedgeType), termName);
        }

        public T OrNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
            rule.AddConnective(rule.LinguisticBase, Connective.Or, variableName, Literal.IsNot, LinguisticHedge.ToValue(hedgeType), termName);
    }
}