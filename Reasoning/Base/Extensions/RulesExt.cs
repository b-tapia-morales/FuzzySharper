using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Rule.Abstractions;
using Shared.Primitives.Implementation;

namespace Reasoning.Base.Extensions;

public static class RulesExt
{
    extension<T>(ICollection<T> rules) where T : class, IRule
    {
        public ISet<StringOrType> GetBaseVariables()
        {
            var conditionals = rules.Select(rule => rule.Conditional!.Identifier);
            var connectives = rules.Where(e => e.Connectives.Count > 0).SelectMany(e => e.Connectives).Select(rule => rule.Identifier);
            var consequents = rules.GetInferredVariables().Select(e => (StringOrType) e);
            return conditionals.Concat(connectives).Except(consequents).ToHashSet();
        }

        public ISet<string> GetInferredVariables() =>
            rules.Select(e => e.Consequent!).Select(rule => rule.Target).ToHashSet(StringComparer.OrdinalIgnoreCase);

        public ISet<StringOrType> GetAllVariables()
        {
            var conditionals = rules.Select(rule => rule.Conditional!.Identifier);
            var connectives = rules.Where(e => e.Connectives.Count > 0).SelectMany(e => e.Connectives).Select(rule => rule.Identifier);
            var consequents = rules.Select(rule => (StringOrType) rule.Consequent!.Target);
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

        public IDictionary<string, List<T>> BuildRuleDependencyMap()
        {
            var dict = new Dictionary<string, List<T>>(StringComparer.OrdinalIgnoreCase);
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

        public IEnumerable<T> FindByPremise(StringOrType variableName) =>
            rules.Where(e => e.PremiseContains(variableName)).ToList();

        public IEnumerable<T> FindByConclusion(string variableName) =>
            rules.Where(e => e.ConsequentContains(variableName)).ToList();

        public IEnumerable<T> GetActivated(uint iteration) =>
            rules.Where(r => r.AdaptationState.LearningHistory.Count > 0 && r.AdaptationState.LearningHistory.Last!.Value.Iteration == iteration);

        public IEnumerable<T> GetUnactivated(uint iteration) =>
            rules.Where(r => r.AdaptationState.LearningHistory.Count == 0 || r.AdaptationState.LearningHistory.Last!.Value.Iteration != iteration);

        public IEnumerable<T> GetDormant(IWorkingMemory memory, uint iteration)
        {
            var eligible = rules.GetEvaluable(memory);
            var activated = rules.GetActivated(iteration);
            return eligible.Except(activated);
        }

        public IEnumerable<T> GetEvaluable(IWorkingMemory memory) =>
            rules.Where(e => e.IsPremiseEvaluable(memory));

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
    }
}