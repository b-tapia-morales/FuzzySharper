using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Rule.Abstractions;
using Shared.Primitives.Implementation;
using Utils.Graph;

namespace Reasoning.Rule.Extensions;

public static class RuleCollExt
{
    extension(ICollection<IRule> rules)
    {
        public ISet<StringOrType> GetBaseVariables()
        {
            var conditionals = rules.Select(rule => rule.Conditional!.Identifier);
            var connectives = rules.Where(e => e.Connectives.Count > 0).SelectMany(e => e.Connectives).Select(rule => rule.Identifier);
            var consequents = rules.GetInferredVariables().Select(e => (StringOrType) e);
            return conditionals.Concat(connectives).Except(consequents).ToHashSet();
        }

        public ISet<string> GetInferredVariables() =>
            rules.Select(e => e.Consequent!).Select(rule => rule.Variable).ToHashSet(StringComparer.OrdinalIgnoreCase);

        public ISet<StringOrType> GetAllVariables()
        {
            var conditionals = rules.Select(rule => rule.Conditional!.Identifier);
            var connectives = rules.Where(e => e.Connectives.Count > 0).SelectMany(e => e.Connectives).Select(rule => rule.Identifier);
            var consequents = rules.Select(rule => rule.Consequent!.Identifier);
            return conditionals.Concat(connectives).Concat(consequents).ToHashSet();
        }

        public ISet<Type> GetBooleanVariables() =>
            rules.GetBaseVariables().Where(e => e.IsType).Select(e => e.AsType).ToHashSet();

        public ISet<string> GetFuzzyVariables() =>
            rules.GetAllVariables().Where(e => e.IsString).Select(e => e.AsString).ToHashSet(StringComparer.OrdinalIgnoreCase);

        public ISet<StringOrType> FindDependentVariables(string variableName)
        {
            var filteredRules = rules.FindByConclusion(variableName).ToList();
            if (filteredRules.Count == 0)
                return new HashSet<StringOrType>();
            var antecedents = filteredRules.Select(rule => rule.Conditional!.Identifier);
            var connectives = filteredRules.Where(e => e.Connectives.Count > 0).SelectMany(e => e.Connectives).Select(rule => rule.Identifier);
            return antecedents.Concat(connectives).ToHashSet();
        }

        public IDictionary<string, List<IRule>> BuildRuleDependencyMap()
        {
            var dict = new Dictionary<string, List<IRule>>(StringComparer.OrdinalIgnoreCase);
            foreach (var variable in rules.GetFuzzyVariables())
                dict[variable] = rules.FindByConclusion(variable).ToList();

            return dict;
        }

        public IDictionary<StringOrType, List<StringOrType>> BuildDependencyGraph()
        {
            var consequents = rules.GetInferredVariables().ToDictionary(e => (StringOrType) e, e => new List<StringOrType>(rules.FindDependentVariables(e)));
            var antecedents = rules.GetBaseVariables().ToDictionary(e => e, _ => new List<StringOrType>());
            return consequents.Concat(antecedents).ToDictionary(e => e.Key, e => e.Value);
        }

        public IEnumerable<IRule> FilterByResolutionMethod(string variableName, IComparer<IRule> ruleComparer) =>
            rules
                .Where(rule => string.Equals(rule.Consequent!.Variable, variableName, StringComparison.OrdinalIgnoreCase))
                .GroupBy(rule => rule.Consequent!.Term)
                .Select(grouping => (Function: grouping.Key, Rule: grouping.MaxBy(g => g, ruleComparer)))
                .Select(tuple => tuple.Rule)!;

        public IEnumerable<IRule> FilterFacts(IWorkingMemory workingMemory)
        {
            var keys = workingMemory.NumericStorage.Keys().Select(e => e.AsString).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var rule in rules)
            {
                if (!keys.Contains(rule.Consequent!.Variable))
                    yield return rule;
            }
        }

        public IEnumerable<IRule> FilterCircularDependencies(string variableName)
        {
            var adjacencyList = rules.BuildDependencyGraph();
            var backEdges = GraphUtils.FindBackEdges(adjacencyList, variableName);
            var antecedent = backEdges.Select(e => e.From.AsString).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var conclusion = backEdges.Select(e => e.To).ToHashSet();
            foreach (var rule in rules)
            {
                if (!(conclusion.Contains(rule.Conditional!.Identifier) ||
                      rule.Connectives.Any(r => conclusion.Contains(r.Identifier)) ||
                      antecedent.Contains(rule.Consequent!.Variable)))
                    yield return rule;
            }
        }

        public IEnumerable<IRule> FindByPremise(StringOrType variableName) =>
            rules.Where(e => e.PremiseContains(variableName)).ToList();

        public IEnumerable<IRule> FindByConclusion(string variableName) =>
            rules.Where(e => e.ConsequentContains(variableName)).ToList();

        public IEnumerable<IRule> GetActivated(uint iteration) =>
            rules.Where(r => r.AdaptationState.LearningHistory.Count > 0 && r.AdaptationState.LearningHistory.Last!.Value.Iteration == iteration);

        public IEnumerable<IRule> GetUnactivated(uint iteration) =>
            rules.Where(r => r.AdaptationState.LearningHistory.Count == 0 || r.AdaptationState.LearningHistory.Last!.Value.Iteration != iteration);

        public IEnumerable<IRule> GetDormant(IWorkingMemory memory, uint iteration)
        {
            var eligible = rules.GetEvaluable(memory);
            var activated = rules.GetActivated(iteration);
            return eligible.Except(activated);
        }

        public IEnumerable<IRule> GetEvaluable(IWorkingMemory memory) =>
            rules.Where(e => e.IsPremiseEvaluable(memory));

        public IEnumerable<IRule> GetNeverActivated() =>
            rules.Where(r => r.AdaptationState.LearningHistory.Count == 0);

        public IEnumerable<IRule> GetEverActivated() =>
            rules.Where(r => r.AdaptationState.LearningHistory.Count > 0);

        public void UpdateLearning(AdaptationConfig config)
        {
            foreach (var rule in rules)
                rule.UpdateLearning(config.MaxHistorySize, config.WeightAggregator);
        }

        public void ResetLearning()
        {
            foreach (var rule in rules)
                rule.ResetLearning();
        }
    }
}