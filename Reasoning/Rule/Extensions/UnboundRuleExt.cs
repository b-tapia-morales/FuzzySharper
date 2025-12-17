using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;

namespace Reasoning.Rule.Extensions;

public static class UnboundRuleExt
{
    extension<T>(T rule) where T : class, IContextFreeRule<T>
    {
        public T If(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
            rule.AddAntecedent(linguisticBase, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);

        public T IfNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
            rule.AddAntecedent(linguisticBase, variableName, Literal.IsNot, LinguisticHedge.ToValue(hedgeType), termName);

        public T And(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
            rule.AddConnective(linguisticBase, Connective.And, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);

        public T AndNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
            rule.AddConnective(linguisticBase, Connective.And, variableName, Literal.IsNot, LinguisticHedge.ToValue(hedgeType), termName);

        public T Or(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
            rule.AddConnective(linguisticBase, Connective.Or, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);

        public T OrNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
            rule.AddConnective(linguisticBase, Connective.Or, variableName, Literal.IsNot, LinguisticHedge.ToValue(hedgeType), termName);
    }
}