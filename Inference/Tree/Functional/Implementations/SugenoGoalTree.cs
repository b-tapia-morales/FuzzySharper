using Inference.Tree.Abstractions;
using Inference.Tree.Extensions;
using Inference.Tree.Functional.Abstractions;
using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Base.Functional.Extensions;
using Reasoning.Rule.Functional.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Inference.Tree.Functional.Implementations;

/// <summary>Derives numeric facts by aggregating functional rules in dependency order.</summary>
/// <param name="identifier">The numeric or categorical identifier represented by this node.</param>
public class SugenoGoalTree(StringOrType identifier)
    : AbstractGoalTree<SugenoGoalTree, IFunctionalRule>(identifier), ISugenoGoalTree<SugenoGoalTree>
{
    /// <inheritdoc />
    public Option<double> InferFact(IWorkingMemory memory, IOperatorFamily operatorFamily) =>
        InferFact(memory, operatorFamily, Option<uint>.None());

    /// <inheritdoc />
    public Option<double> InferFact(IWorkingMemory memory, IOperatorFamily operatorFamily, Option<uint> iteration)
    {
        if (Identifier.IsString && memory.GetNumericFact(Identifier.AsString).IsSome(out var knownFact))
        {
            IsProven = true;
            return knownFact;
        }

        var stack = this.TraverseReverseLevelOrder();
        while (stack.TryPop(out var node))
        {
            node.IsProven = memory.Contains(node.Identifier);
            if (node.IsProven || node.Identifier.IsType || node.Rules.Count == 0)
                continue;

            var target = node.Identifier.AsString;
            if (!node.Rules.EvaluateTarget(target, memory, operatorFamily).IsSome(out var value))
                continue;

            if (iteration.IsSome(out var current))
                AppendRecords(node.Rules, memory, operatorFamily, current);

            memory.AddNumericFact(target, value);
            node.IsProven = true;
        }

        return Identifier.IsString ? memory.GetNumericFact(Identifier.AsString) : Option<double>.None();
    }

    /// <inheritdoc />
    public static SugenoGoalTree BuildTree(StringOrType rootIdentifier, ICollection<IFunctionalRule> rules) =>
        SugenoGoalTree.BuildTree(rootIdentifier, rules, static id => new SugenoGoalTree(id));

    // Future records must use the raw premise weights of rules that contributed to successful aggregation.
    // Missing inputs and zero activation are not recorded; the evaluator should expose its actual contributions.
    private static void AppendRecords(ICollection<IFunctionalRule> rules, IWorkingMemory memory,
        IOperatorFamily operatorFamily, uint iteration) =>
        throw new NotImplementedException("Recording contributing Sugeno rules is not implemented.");
}
