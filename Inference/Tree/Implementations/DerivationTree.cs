using Inference.Aggregator.Abstractions;
using Inference.Tree.Extensions;
using Inference.Defuzzifier.Abstractions;
using Inference.Tree.Abstractions;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Base.Extensions;
using Reasoning.Base.FuzzySet.Extensions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Inference.Tree.Implementations;

public class DerivationTree(StringOrType identifier) : ITree<DerivationTree>
{
    public StringOrType Identifier { get; } = identifier;
    public ICollection<IFuzzySetRule> Rules { get; } = new List<IFuzzySetRule>();
    public ICollection<DerivationTree> Children { get; } = new List<DerivationTree>();
    public bool IsProven { get; private set; }

    public bool IsLeaf() =>
        Children.Count == 0;

    public void AddRules(IEnumerable<IFuzzySetRule> rules)
    {
        foreach (var rule in rules)
            Rules.Add(rule);
    }

    public void AddChild(DerivationTree child) =>
        Children.Add(child);

    public void AddChildren(IEnumerable<DerivationTree> children)
    {
        foreach (var child in children)
            Children.Add(child);
    }

    public static DerivationTree BuildTree(StringOrType rootIdentifier, ICollection<IFuzzySetRule> rules)
    {
        return BuildTreeNode(rootIdentifier, rules.BuildDependencyGraph(), rules.BuildRuleDependencyMap());

        static DerivationTree BuildTreeNode(StringOrType currentId, IDictionary<StringOrType, List<StringOrType>> dependencyGraph,
            IDictionary<string, List<IFuzzySetRule>> ruleDependencyMap)
        {
            var currentNode = new DerivationTree(currentId);

            // Boolean variable - Leaf node
            if (currentId.IsType)
                return currentNode;

            // Fuzzy variable - Does not have rule dependencies
            if (!ruleDependencyMap.TryGetValue(currentId.AsString, out var rules) || rules.Count == 0)
                return currentNode;

            currentNode.AddRules(rules);

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

    public Option<double> InferFact(IWorkingMemory memory, IComparer<IFuzzySetRule> comparer,
        IDefuzzifier defuzzifier, IValueAggregator aggregator, IOperatorFamily operatorFamily, ImplicationMethod implicationMethod) =>
        InferFact(memory, comparer, defuzzifier, aggregator, operatorFamily, implicationMethod, Option<uint>.None());

    public Option<double> InferFact(IWorkingMemory memory, IComparer<IFuzzySetRule> comparer,
        IDefuzzifier defuzzifier, IValueAggregator aggregator, IOperatorFamily operatorFamily, ImplicationMethod implicationMethod, Option<uint> iteration)
    {
        var stack = this.TraverseReverseLevelOrder();
        while (stack.TryPop(out var node))
        {
            if (node.IsLeaf())
            {
                node.IsProven = memory.Contains(node.Identifier);
                continue;
            }

            var variableName = node.Identifier.AsString;
            var rules = node.Rules.FilterByResolutionMethod(variableName, comparer).ToList();
            if (rules.Count == 0)
            {
                node.IsProven = false;
                continue;
            }

            if (defuzzifier.Defuzzify(rules, memory, operatorFamily, aggregator, implicationMethod, out var activatedRules).IsSome(out var crispValue))
                memory.AddNumericFact(variableName, crispValue);

            AppendRecords(activatedRules, memory, operatorFamily, iteration);
            node.IsProven = true;
        }

        if (!memory.GetNumericFact(Identifier.AsString).IsSome(out var fact))
            return Option<double>.None();
        IsProven = true;
        return fact;
    }

    private static void AppendRecords(ICollection<IFuzzySetRule> rules, IWorkingMemory memory, IOperatorFamily family, Option<uint> iteration)
    {
        if (!iteration.IsSome(out var current))
            return;
        foreach (var rule in rules)
            rule.AdaptationState.Append(rule.EvaluateRuleWeight(memory, family).Get, current);
    }
}