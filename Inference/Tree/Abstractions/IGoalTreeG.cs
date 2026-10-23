using Reasoning.Rule.Abstractions;

namespace Inference.Tree.Abstractions;

public interface IGoalTree<TSelf, TRule> : IGoalTree<TSelf>
    where TSelf : class, IGoalTree<TSelf, TRule>
    where TRule : class, IRule
{
    ICollection<TRule> Rules { get; }

    void AddRules(IEnumerable<TRule> rules);
}