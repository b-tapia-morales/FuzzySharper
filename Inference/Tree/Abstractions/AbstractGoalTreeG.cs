using Reasoning.Rule.Abstractions;
using Shared.Primitives.Implementation;

namespace Inference.Tree.Abstractions;

public abstract class AbstractGoalTree<TSelf, TRule>(StringOrType identifier) : AbstractGoalTree<TSelf>(identifier)
    where TSelf : AbstractGoalTree<TSelf, TRule>
    where TRule : class, IRule
{
    public ICollection<TRule> Rules { get; } = new List<TRule>();

    public void AddRules(IEnumerable<TRule> rules)
    {
        foreach (var rule in rules)
            Rules.Add(rule);
    }
}
