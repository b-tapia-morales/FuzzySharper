using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Components;
using Shared.Primitives.Implementation;
using Utils.Graph;

namespace Reasoning.Base.Extensions;

public static class RulesExt
{
    extension<T>(ICollection<T> rules) where T : class, IRule
    {
        public IReadOnlySet<StringOrType> GetBaseVariables()
        {
            var conditionals = rules.Select(rule => rule.Conditional!.Identifier);
            var connectives = rules.Where(e => e.Connectives.Count > 0).SelectMany(e => e.Connectives)
                .Select(rule => rule.Identifier);
            var consequents = rules.GetInferredVariables().Select(e => (StringOrType) e);
            return conditionals.Concat(connectives).Except(consequents).ToHashSet();
        }

        public IReadOnlySet<string> GetInferredVariables() =>
            rules.Select(e => e.Consequent!).Select(rule => rule.Identifier)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

        public IReadOnlySet<StringOrType> GetAllVariables()
        {
            var conditionals = rules.Select(rule => rule.Conditional!.Identifier);
            var connectives = rules.Where(e => e.Connectives.Count > 0).SelectMany(e => e.Connectives)
                .Select(rule => rule.Identifier);
            var consequents = rules.Select(rule => (StringOrType) rule.Consequent!.Identifier);
            return conditionals.Concat(connectives).Concat(consequents).ToHashSet();
        }

        public IReadOnlySet<Type> GetBooleanVariables() =>
            rules.GetBaseVariables().Where(e => e.IsType).Select(e => e.AsType).ToHashSet();

        public IReadOnlySet<string> GetFuzzyVariables() =>
            rules.GetAllVariables().Where(e => e.IsString).Select(e => e.AsString).ToHashSet(StringComparer.OrdinalIgnoreCase);

        public IReadOnlySet<StringOrType> FindDependentVariables(string variableName)
        {
            var filteredRules = rules.FindByConclusion(variableName).ToList();
            if (filteredRules.Count == 0)
                return new HashSet<StringOrType>();
            var antecedents = filteredRules.Select(rule => rule.Conditional!.Identifier);
            var connectives = filteredRules.Where(e => e.Connectives.Count > 0).SelectMany(e => e.Connectives)
                .Select(rule => rule.Identifier);
            return antecedents.Concat(connectives).ToHashSet();
        }

        public IReadOnlyDictionary<string, IReadOnlyList<T>> BuildRuleDependencyMap()
        {
            var dict = new Dictionary<string, List<T>>(StringComparer.OrdinalIgnoreCase);
            foreach (var variable in rules.GetFuzzyVariables())
                dict[variable] = [..rules.FindByConclusion(variable)];

            return dict.ToDictionary(pair => pair.Key, IReadOnlyList<T> (pair) => pair.Value,
                StringComparer.OrdinalIgnoreCase);
        }

        public IReadOnlyDictionary<StringOrType, IReadOnlyList<StringOrType>> BuildDependencyGraph()
        {
            var consequents = rules.GetInferredVariables().ToDictionary(e => (StringOrType) e,
                e => new List<StringOrType>(rules.FindDependentVariables(e)));
            var antecedents = rules.GetBaseVariables().ToDictionary(e => e, _ => new List<StringOrType>());
            return consequents.Concat(antecedents).ToDictionary(e => e.Key, IReadOnlyList<StringOrType> (e) => e.Value);
        }

        public IEnumerable<T> FindByPremise(StringOrType variableName) =>
            [..rules.Where(e => e.PremiseContains(variableName))];

        public IEnumerable<T> FindByConclusion(string variableName) =>
            [..rules.Where(e => e.ConsequentContains(variableName))];

        public IEnumerable<T> FilterFacts(IWorkingMemory workingMemory)
        {
            var keys = workingMemory.NumericStorage.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
            return rules.Where(r => !keys.Contains(r.Consequent!.Identifier));
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
                      antecedent.Contains(rule.Consequent!.Identifier)))
                    yield return rule;
            }
        }

        public IEnumerable<T> GetEvaluable(IWorkingMemory memory) =>
            rules.Where(e => e.IsPremiseEvaluable(memory));

        public IEnumerable<T> GetActivated(uint iteration) =>
            rules.Where(r =>
                r.AdaptationState.LearningHistory.Count > 0 &&
                r.AdaptationState.LearningHistory.Last!.Value.Iteration == iteration);

        public IEnumerable<T> GetUnactivated(uint iteration) =>
            rules.Where(r =>
                r.AdaptationState.LearningHistory.Count == 0 ||
                r.AdaptationState.LearningHistory.Last!.Value.Iteration != iteration);

        public IEnumerable<T> GetDormant(IWorkingMemory memory, uint iteration)
        {
            var eligible = rules.GetEvaluable(memory);
            var activated = rules.GetActivated(iteration);
            return eligible.Except(activated);
        }

        public IEnumerable<T> GetNeverActivated() =>
            rules.Where(r => r.AdaptationState.LearningHistory.Count == 0);

        public IEnumerable<T> GetEverActivated() =>
            rules.Where(r => r.AdaptationState.LearningHistory.Count > 0);

        public void RecomputeAdaptation(AdaptationConfig config)
        {
            foreach (var rule in rules)
                rule.RecomputeAdaptation(config.MaxHistorySize, config.WeightAggregator);
        }

        public void ResetAdaptation()
        {
            foreach (var rule in rules)
                rule.ResetAdaptation();
        }

        public ICollection<T> ShallowCopy() =>
            [..rules];

        public ICollection<T> DeepCopy(LifecycleMode mode = LifecycleMode.NewInstance) =>
            [..rules.Select(r => (T) r.DeepCopy(mode))];
    }

}