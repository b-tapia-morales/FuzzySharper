using Inference.Tree.Abstractions;
using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Functional.Abstractions;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Inference.Tree.Functional.Abstractions;

/// <summary>Represents a goal tree that derives numeric facts using functional rules.</summary>
/// <typeparam name="T">The implementing goal-tree node type.</typeparam>
public interface ISugenoGoalTree<T> : IGoalTree<T, IFunctionalRule> where T : class, ISugenoGoalTree<T>
{
    /// <summary>
    /// Returns a known root fact or derives it by evaluating dependencies before their dependents.
    /// Successfully inferred values are added to the supplied working memory. Rules with unavailable inputs
    /// or zero premise activation do not participate in aggregation.
    /// </summary>
    /// <param name="memory">The facts used and extended during inference.</param>
    /// <param name="operatorFamily">The operators used to evaluate rule premises.</param>
    /// <returns>The root's numeric fact, or no value if it cannot be obtained or the root is categorical.</returns>
    Option<double> InferFact(IWorkingMemory memory, IOperatorFamily operatorFamily);

    /// <summary>Infers the root's numeric fact and optionally requests activation records for an iteration.</summary>
    /// <param name="memory">The facts used and extended during inference.</param>
    /// <param name="operatorFamily">The operators used to evaluate rule premises.</param>
    /// <param name="iteration">The iteration to record, or no value to disable recording.</param>
    /// <returns>The root's numeric fact, or no value if it cannot be obtained or the root is categorical.</returns>
    /// <exception cref="NotImplementedException">Recording is requested for a successfully evaluated node.</exception>
    /// <remarks>Activation recording is reserved for future work. Omit the iteration to perform inference without records.</remarks>
    Option<double> InferFact(IWorkingMemory memory, IOperatorFamily operatorFamily, Option<uint> iteration);

    /// <summary>Builds the root and its dependencies from the supplied functional rules.</summary>
    /// <param name="rootIdentifier">The identifier whose dependencies form the tree.</param>
    /// <param name="rules">The functional rules prepared for this inference request.</param>
    /// <returns>The root node with its associated rules and dependency nodes.</returns>
    /// <remarks>The dependency graph must be acyclic. Fact filtering and cycle pruning belong to inference preparation.</remarks>
    static abstract T BuildTree(StringOrType rootIdentifier, ICollection<IFunctionalRule> rules);
}
