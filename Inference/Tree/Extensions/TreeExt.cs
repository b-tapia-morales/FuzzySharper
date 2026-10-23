using Inference.Tree.Abstractions;
using Inference.Tree.Fuzzy.Comparers.Factory;
using Reasoning.Base.Extensions;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.FuzzySet.Components;
using Shared.Primitives.Implementation;

namespace Inference.Tree.Extensions;

public static class TreeExt
{
    extension<TSelf, TRule>(IGoalTree<TSelf, TRule> treeNode)
        where TSelf : class, IGoalTree<TSelf, TRule>
        where TRule : class, IRule
    {
        public Stack<IGoalTree<TSelf, TRule>> TraverseReverseLevelOrder()
        {
            var queue = new Queue<IGoalTree<TSelf, TRule>>();
            queue.Enqueue(treeNode);
            var stack = new Stack<IGoalTree<TSelf, TRule>>();
            while (queue.TryDequeue(out var node))
            {
                stack.Push(node);
                foreach (var child in node.Children)
                    queue.Enqueue(child);
            }

            return stack;
        }

        public int MaxDepth()
        {
            if (treeNode.Children.Count == 0)
                return 0;
            var maxDepth = 0;
            var stack = new Stack<(IGoalTree<TSelf, TRule> Node, int Depth)>();
            stack.Push((treeNode, 0));
            while (stack.TryPop(out var tuple))
            {
                var (node, depth) = tuple;
                maxDepth = Math.Max(depth, maxDepth);
                foreach (var child in node.Children)
                    stack.Push((child, depth + 1));
            }

            return maxDepth;
        }

        public int NodeCount() =>
            1 + treeNode.Children.Sum(child => child.NodeCount());

        public int RuleCount(bool countCompetingRules = false)
        {
            var localCount = countCompetingRules || treeNode.Rules.Any(rule => rule.Consequent is not FuzzySetConsequent)
                ? treeNode.Rules.Count
                : treeNode.Rules
                    .Select(rule => ((FuzzySetConsequent) rule.Consequent!).Proposition.Label)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Count();

            return localCount + treeNode.Children.Sum(child => child.RuleCount(countCompetingRules));
        }

        public int CoveredRuleCount() =>
            treeNode.Rules.Count(rule => rule.AdaptationState.LearningHistory.Count > 0) +
            treeNode.Children.Sum(child => child.CoveredRuleCount());

        public int UncoveredRuleCount() =>
            treeNode.Rules.Count(rule => rule.AdaptationState.LearningHistory.Count == 0) +
            treeNode.Children.Sum(child => child.UncoveredRuleCount());

        public int CalculateMetric(TreeMetric metric) => metric switch
        {
            TreeMetric.NodeCount => treeNode.NodeCount(),
            TreeMetric.MaxDepth => treeNode.MaxDepth(),
            TreeMetric.RuleCount => treeNode.RuleCount(),
            TreeMetric.CoveredRuleCount => treeNode.CoveredRuleCount(),
            TreeMetric.UncoveredRuleCount => treeNode.UncoveredRuleCount(),
            _ => throw new ArgumentOutOfRangeException(nameof(metric), metric, null)
        };

        public void WriteTree()
        {
            var stack = new Stack<IGoalTree<TSelf, TRule>>();
            stack.Push(treeNode);
            while (stack.TryPop(out var node))
            {
                node.WriteNode();
                foreach (var child in node.Children)
                    stack.Push(child);
            }
        }

        public void PrettyWriteTree() =>
            treeNode.PrettyWrite();

        public void WriteNode()
        {
            Console.WriteLine($"Variable name: {treeNode.Identifier}");
            if (!treeNode.IsLeaf())
                Console.WriteLine(
                    $"Associated rules:{Environment.NewLine}{string.Join(Environment.NewLine, treeNode.Rules)}");
            Console.WriteLine($"Proven: {treeNode.IsProven}");
        }

        private void PrettyWrite(string? indent = "", bool last = true)
        {
            Console.WriteLine($"{indent}+- {treeNode.Identifier}");
            indent += last ? "   " : "|  ";
            foreach (var (child, i) in treeNode.Children.Select((child, i) => (child, i)))
                child.PrettyWrite(indent, i == treeNode.Children.Count - 1);
        }

        public static TSelf BuildTree(StringOrType rootIdentifier, ICollection<TRule> rules, Func<StringOrType, TSelf> createNode)
        {
            return BuildTreeNode(rootIdentifier, rules.BuildDependencyGraph(), rules.BuildRuleDependencyMap());

            TSelf BuildTreeNode(StringOrType currentId, IReadOnlyDictionary<StringOrType, IReadOnlyList<StringOrType>> dependencyGraph,
                IReadOnlyDictionary<string, IReadOnlyList<TRule>> ruleDependencyMap)
            {
                var currentNode = createNode(currentId);

                // Boolean variable - Leaf node
                if (currentId.IsType)
                    return currentNode;

                // Fuzzy variable - Does not have rule dependencies
                if (!ruleDependencyMap.TryGetValue(currentId.AsString, out var associatedRules) || associatedRules.Count == 0)
                    return currentNode;

                currentNode.AddRules(associatedRules);

                // Fuzzy variable with rules but no derivational dependencies:
                // This variable does establish a dependency with other variables by using them as part of its premises,
                // but all of those variables need to be known as facts (boolean or crisp), thus they
                // require no further derivation. Therefore, this node has no children and becomes a leaf.
                if (!dependencyGraph.TryGetValue(currentId, out var childrenIds) || childrenIds.Count == 0)
                    return currentNode;

                // Fuzzy variable with rule AND derivational dependencies:
                foreach (var childId in childrenIds)
                {
                    var childNode = BuildTreeNode(childId, dependencyGraph, ruleDependencyMap);
                    currentNode.AddChild(childNode);
                }

                return currentNode;
            }
        }
    }
}
