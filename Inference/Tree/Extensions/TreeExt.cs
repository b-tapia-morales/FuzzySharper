using Inference.Tree.Abstractions;
using Inference.Tree.Comparers.Factory;

namespace Inference.Tree.Extensions;

public static class TreeExt
{
    extension<T>(T treeNode) where T : class, ITree<T>
    {
        public Stack<T> TraverseReverseLevelOrder()
        {
            var queue = new Queue<T>();
            queue.Enqueue(treeNode);
            var stack = new Stack<T>();
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
            var stack = new Stack<(T Node, int Depth)>();
            stack.Push((treeNode, 1));
            while (stack.TryPop(out var tuple))
            {
                var (node, depth) = tuple;
                maxDepth = Math.Max(node.MaxDepth(), maxDepth);
                foreach (var child in node.Children)
                    stack.Push((child, depth + 1));
            }

            return maxDepth;
        }

        public int NodeCount() =>
            treeNode.Children.Count == 0
                ? 0
                : 1 + treeNode.Children.Sum(child => child.NodeCount());

        public int RuleCount(bool countCompetingRules = false)
        {
            if (treeNode.Children.Count == 0)
                return countCompetingRules
                    ? treeNode.Rules.Count
                    : treeNode.Rules
                        .Select(r => r.Consequent!.Target)
                        .DistinctBy(v => v, StringComparer.OrdinalIgnoreCase)
                        .Count();
            return treeNode.Children.Sum(child => child.RuleCount(countCompetingRules));
        }

        public int ActiveRuleCount()
        {
            return treeNode.Children.Count == 0
                ? treeNode.Rules.Count(r => r.AdaptationState.LearningHistory.Count > 0)
                : treeNode.Children.Sum(child => child.ActiveRuleCount());
        }

        public int UncoveredRuleCount()
        {
            return treeNode.Children.Count == 0
                ? treeNode.Rules.Count(r => r.AdaptationState.LearningHistory.Count == 0)
                : treeNode.Children.Sum(child => child.UncoveredRuleCount());
        }

        public int CalculateMetric(TreeMetric metric) => metric switch
        {
            TreeMetric.NodeCount => treeNode.NodeCount(),
            TreeMetric.MaxDepth => treeNode.MaxDepth(),
            TreeMetric.RuleCount => treeNode.RuleCount(),
            TreeMetric.ActiveRuleCount => treeNode.ActiveRuleCount(),
            TreeMetric.UncoveredRuleCount => treeNode.UncoveredRuleCount(),
            _ => throw new ArgumentOutOfRangeException(nameof(metric), metric, null)
        };

        public void PrettyWriteTree(string indent = "", bool last = true)
        {
            Console.WriteLine($"{indent}+- {treeNode.Identifier}");
            indent += last ? "   " : "|  ";
            foreach (var (child, i) in treeNode.Children.Select((child, i) => (child, i)))
                child.PrettyWriteTree(indent, i == treeNode.Children.Count - 1);
        }

        public void WriteTree()
        {
            var stack = new Stack<T>();
            stack.Push(treeNode);
            while (stack.TryPop(out var node))
            {
                node.WriteNode();
                foreach (var child in node.Children)
                    stack.Push(child);
            }
        }

        public void WriteNode()
        {
            Console.WriteLine($"Variable name: {treeNode.Identifier}");
            if (!treeNode.IsLeaf())
                Console.WriteLine($"Associated rules:{Environment.NewLine}{string.Join(Environment.NewLine, treeNode.Rules)}");
            Console.WriteLine($"Proven: {treeNode.IsProven}");
        }
    }
}