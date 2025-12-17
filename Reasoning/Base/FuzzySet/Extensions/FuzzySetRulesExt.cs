using Knowledge.Memory.Abstractions;
using Reasoning.Base.Extensions;
using Reasoning.Rule.FuzzySet;
using Reasoning.Rule.FuzzySet.Abstractions;
using Utils.Graph;

namespace Reasoning.Base.FuzzySet.Extensions;

public static class FuzzySetRulesExt
{
    extension<T>(ICollection<T> rules) where T : class, IFuzzySetRule
    {
        public IEnumerable<T> FilterByResolutionMethod(string variableName, IComparer<IFuzzySetRule> ruleComparer) =>
            rules
                .Where(rule => string.Equals(rule.Consequent!.Target, variableName, StringComparison.OrdinalIgnoreCase))
                .GroupBy(rule => ((FuzzySetConsequent) rule.Consequent!).Proposition.Term)
                .Select(grouping => (Function: grouping.Key, Rule: grouping.MaxBy(g => g, ruleComparer)))
                .Select(tuple => tuple.Rule)!;

        public IEnumerable<T> FilterFacts(IWorkingMemory workingMemory)
        {
            var keys = workingMemory.NumericStorage.Keys().Select(e => e.AsString).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var rule in rules)
            {
                if (!keys.Contains(rule.Consequent!.Target))
                    yield return rule;
            }
        }

        public IEnumerable<T> FilterCircularDependencies(string variableName)
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