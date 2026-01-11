using Kernel.Number;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Negation.Implementations.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Proposition.Components;
using Reasoning.Proposition.Implementations;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Reasoning.Proposition.Abstractions;

/// <summary>
/// Represents a propositional statement whose truth value can be evaluated with respect to a
/// <see cref="IWorkingMemory">working memory</see> or a directly supplied value.
/// </summary>
/// <remarks>
/// A proposition expresses a structured statement of truth composed of a subject (<see cref="Identifier"/>) and a
/// predicate (<see cref="Label"/>), and participates in a rule through its associated <see cref="Connective"/>.
/// For example, the statement <c>IF Water Is Cold</c> is composed of:
/// <list type="bullet">
/// <item><see cref="Connective"/>: <c>If</c></item>
/// <item><see cref="Identifier"/>: <c>Water</c></item>
/// <item><see cref="Literal"/>: <c>Is</c></item>
/// <item><see cref="Label"/>: <c>Cold</c></item>
/// </list>
/// Propositions may be either <see cref="FuzzyProposition">fuzzy </see> or
/// <see cref="BooleanProposition{T}">boolean</see>; fuzzy propositions use a <see cref="string"/> identifier,
/// while boolean propositions use a <see cref="Type"/> identifier. Evaluation produces a truth value in the form of a
/// <see cref="FuzzyNumber"/>.
/// </remarks>
public interface IProposition
{
    /// <summary>
    /// Gets the identifier of the proposition.
    /// </summary>
    StringOrType Identifier { get; }
    /// <summary>
    /// Gets the logical connective applied to the proposition.
    /// </summary>
    Connective Connective { get; }
    /// <summary>
    /// Gets the literal operator applied to the proposition.
    /// </summary>
    Literal Literal { get; }
    /// <summary>
    /// Gets the label associated with the proposition.
    /// </summary>
    string Label { get; }

    /// <summary>
    /// Determines whether the proposition can be evaluated with respect to the provided working memory.
    /// </summary>
    /// <param name="memory">The working memory used to resolve whether the proposition's evaluability.</param>
    /// <returns>
    /// <see langword="true"/> if the proposition can be evaluated; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsEvaluable(IWorkingMemory memory);

    /// <summary>
    /// Evaluates the proposition with respect to the provided working memory using the specified negation operator.
    /// </summary>
    /// <param name="memory">The working memory used to resolve the proposition value.</param>
    /// <param name="negation">The negation operator applied to the evaluated truth value.</param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the evaluated truth value if the proposition
    /// <see cref="IsEvaluable">is evaluable</see>; otherwise, an empty option.
    /// </returns>
    Option<FuzzyNumber> Evaluate(IWorkingMemory memory, INegation negation);

    /// <summary>
    /// Evaluates the proposition with respect to the provided working memory.
    /// </summary>
    /// <param name="memory">The working memory used to resolve the proposition value.</param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the evaluated truth value if the proposition is
    /// <see cref="IsEvaluable">is evaluable</see>; otherwise, an empty option.
    /// </returns>
    /// <remarks>
    /// This overload is functionally equivalent to <see cref="Evaluate(IWorkingMemory, INegation)"/> with
    /// the negation set to the <see cref="Negation.Standard">standard operator</see> by default.
    /// </remarks>
    Option<FuzzyNumber> Evaluate(IWorkingMemory memory) => Evaluate(memory, Negation.Standard);

    /// <summary>
    /// Evaluates the proposition using a directly supplied value and the provided negation operator.
    /// </summary>
    /// <param name="value">The value to evaluate the proposition against.</param>
    /// <param name="negation">The negation operator applied to the evaluated truth value.</param>
    /// <returns>
    /// The evaluated truth value.
    /// </returns>
    FuzzyNumber Evaluate(DoubleOrEnum value, INegation negation);

    /// <summary>
    /// Evaluates the proposition using a directly supplied value and the provided negation operator.
    /// </summary>
    /// <param name="value">The value to evaluate the proposition against.</param>
    /// <returns>
    /// The evaluated truth value.
    /// </returns>
    /// <remarks>
    /// This overload is functionally equivalent to <see cref="Evaluate(DoubleOrEnum, INegation)"/> with
    /// the negation set to the <see cref="Negation.Standard">standard operator</see> by default.
    /// </remarks>
    FuzzyNumber Evaluate(DoubleOrEnum value) => Evaluate(value, Negation.Standard);
    
    /// <summary>
    /// Creates a deep copy of the proposition.
    /// </summary>
    /// <returns>
    /// A new <see cref="IProposition"/> instance that is structurally identical to the current one.
    /// </returns>
    IProposition DeepCopy();
}