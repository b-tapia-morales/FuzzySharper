using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Extensions;
using Reasoning.Rule.FuzzySet.Abstractions;

namespace Reasoning.Rule.FuzzySet.Extensions;

public static class BoundFuzzySetRuleExt
{
    extension<T>(T rule) where T : class, IFuzzySetRule, IContextBoundRule<T>
    {
        public T Then(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None)
        {
            rule.ValidateConflictingVariable(linguisticBase, variableName);
            return rule.AddConsequent(linguisticBase, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);
        }

        public T Then(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
            rule.AddConsequent(rule.LinguisticBase, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);
    }
}