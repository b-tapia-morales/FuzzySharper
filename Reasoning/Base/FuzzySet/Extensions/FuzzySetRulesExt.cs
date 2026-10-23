using Reasoning.Rule.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Components;

namespace Reasoning.Base.FuzzySet.Extensions;

public static class FuzzySetRulesExt
{
    extension(ICollection<IFuzzySetRule> rules)
    {
        public IEnumerable<IFuzzySetRule> FilterByResolutionMethod(string variableName, IComparer<IFuzzySetRule> ruleComparer) =>
            rules
                .Where(rule => string.Equals(rule.Consequent!.Identifier, variableName, StringComparison.OrdinalIgnoreCase))
                .GroupBy(rule => ((FuzzySetConsequent) rule.Consequent!).Proposition.Label, StringComparer.OrdinalIgnoreCase)
                .Select(grouping => (Function: grouping.Key, Rule: grouping.MaxBy(g => g, ruleComparer)))
                .Select(tuple => tuple.Rule)!;

    }
}