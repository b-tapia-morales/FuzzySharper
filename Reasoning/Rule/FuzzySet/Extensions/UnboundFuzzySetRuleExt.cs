using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.FuzzySet.Abstractions;

namespace Reasoning.Rule.FuzzySet.Extensions;

internal static class UnboundFuzzySetRuleExt
{
    extension<T>(T rule) where T : class, IFuzzySetRule, IContextFreeRule<T>
    {
        public T Then(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) => 
            rule.AddConsequent(linguisticBase, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);
    }
}