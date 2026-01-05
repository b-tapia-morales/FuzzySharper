using Knowledge.Memory.Abstractions;
using Reasoning.Base.Extensions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Components;
using Utils.Graph;

namespace Reasoning.Base.FuzzySet.Extensions;

public static class FuzzySetRulesExt
{
    extension(ICollection<IFuzzySetRule> rules)
    {
        public IEnumerable<IFuzzySetRule> FilterByResolutionMethod(string variableName, IComparer<IFuzzySetRule> ruleComparer) =>
            rules
                .Where(rule => string.Equals(rule.Consequent!.Target, variableName, StringComparison.OrdinalIgnoreCase))
                .GroupBy(rule => ((FuzzySetConsequent) rule.Consequent!).Proposition.Label, StringComparer.OrdinalIgnoreCase)
                .Select(grouping => (Function: grouping.Key, Rule: grouping.MaxBy(g => g, ruleComparer)))
                .Select(tuple => tuple.Rule)!;

        public IEnumerable<IFuzzySetRule> FilterFacts(IWorkingMemory workingMemory)
        {
            var keys = workingMemory.NumericStorage.Keys.Select(e => e.AsString).ToHashSet(StringComparer.OrdinalIgnoreCase);
            return rules.Where(r => !keys.Contains(r.Consequent!.Target));
        }

        public IEnumerable<IFuzzySetRule> FilterCircularDependencies(string variableName)
        {
            var adjacencyList = rules.BuildDependencyGraph();
            var backEdges = GraphUtils.FindBackEdges(adjacencyList, variableName);
            var antecedent = backEdges.Select(e => e.From.AsString).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var conclusion = backEdges.Select(e => e.To).ToHashSet();
            foreach (var rule in rules)
            {
                if (!(conclusion.Contains(rule.Conditional!.Identifier) ||
                      rule.Connectives.Any(r => conclusion.Contains(r.Identifier)) ||
                      antecedent.Contains(rule.Consequent!.Target)))
                    yield return rule;
            }
        }
    }
}