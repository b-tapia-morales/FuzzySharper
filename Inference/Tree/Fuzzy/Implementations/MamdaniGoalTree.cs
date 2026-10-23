using Inference.Aggregator.Abstractions;
using Inference.Defuzzifier.Abstractions;
using Inference.Tree.Abstractions;
using Inference.Tree.Extensions;
using Inference.Tree.Fuzzy.Abstractions;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Base.FuzzySet.Extensions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Inference.Tree.Fuzzy.Implementations;

public class MamdaniGoalTree(StringOrType identifier) : AbstractGoalTree<MamdaniGoalTree, IFuzzySetRule>(identifier), IMamdaniGoalTree<MamdaniGoalTree>
{
    public Option<double> InferFact(IWorkingMemory memory, IComparer<IFuzzySetRule> comparer,
        IDefuzzifier defuzzifier, IValueAggregator aggregator, IOperatorFamily operatorFamily,
        ImplicationMethod implicationMethod) =>
        InferFact(memory, comparer, defuzzifier, aggregator, operatorFamily, implicationMethod, Option<uint>.None());

    public Option<double> InferFact(IWorkingMemory memory, IComparer<IFuzzySetRule> comparer,
        IDefuzzifier defuzzifier, IValueAggregator aggregator, IOperatorFamily operatorFamily,
        ImplicationMethod implicationMethod, Option<uint> iteration)
    {
        var stack = this.TraverseReverseLevelOrder();
        while (stack.TryPop(out var node))
        {
            // Node is leaf -> node is proven only if it's known as a fact.
            if (node.IsLeaf())
            {
                node.IsProven = memory.Contains(node.Identifier);
                continue;
            }

            // Node has derivational dependencies, but no rules to infer its value -> node remains unproven.
            if (node.Rules.Count == 0)
            {
                node.IsProven = false;
                continue;
            }

            // Node has derivational dependencies AND rules to infer its value.
            // Resolve competing rules, then use surviving rules in the defuzzification process to infer the node's value.
            // The defuzzification process is unsuccessful -> node remains unproven.
            var variableName = node.Identifier.AsString;
            var rules = node.Rules.FilterByResolutionMethod(variableName, comparer).ToList();
            if (!defuzzifier.Defuzzify(rules, memory, operatorFamily, aggregator, implicationMethod, out var activatedRules).IsSome(out var crispValue))
                continue;

            // The node is proven -> mark it as a fact by adding its value.
            memory.AddNumericFact(variableName, crispValue);
            AppendRecords(activatedRules, memory, operatorFamily, iteration);
            node.IsProven = true;
        }

        // Unable to infer the node's value -> node remains unproven.
        if (!memory.GetNumericFact(Identifier.AsString).IsSome(out var fact))
            return Option<double>.None();
        IsProven = true;
        return fact;
    }

    public static MamdaniGoalTree BuildTree(StringOrType rootIdentifier, ICollection<IFuzzySetRule> rules) =>
        MamdaniGoalTree.BuildTree(rootIdentifier, rules, static id => new MamdaniGoalTree(id));

    private static void AppendRecords(ICollection<IFuzzySetRule> rules, IWorkingMemory memory, IOperatorFamily family, Option<uint> iteration)
    {
        if (!iteration.IsSome(out var current))
            return;
        foreach (var rule in rules)
            rule.AdaptationState.Append(rule.EvaluateRuleStrength(memory, family).Get, current);
    }

}