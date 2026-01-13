using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Linguistic.Variable.Abstractions;
using Reasoning.Proposition.Abstractions;
using Reasoning.Proposition.Components;

namespace Reasoning.Rule.Abstractions;

/// <summary>
/// <para>
/// Represents a context-bound rule builder that owns a linguistic base and provides a fluent API for
/// constructing rule premises using linguistic variables and terms.
/// </para>
/// The associated linguistic base serves as the default resolution source for linguistic variables, allowing rules to
/// be expressed in a natural-language style such as <c>If("Water", "Cold")</c> without requiring an external base to
/// be supplied at each step.
/// </summary>
/// <typeparam name="T">
/// Represents the concrete rule type and enables fluent chaining while preserving access to all <see cref="IRule"/>
/// members and implementation-specific functionality.</typeparam>
/// <example>
/// <code>
/// // IF Temperature IS Very High AND Humidity IS High OR Pressure IS Slightly Low
///     rule
///         .If("Temperature", "High", HedgeType.Very)
///         .And("Humidity", "High")
///         .Or("Pressure", "Low", HedgeType.Slightly)
/// // The rule consequent follows here.
/// </code>
/// </example>
/// <seealso cref="IProposition"/>
/// <seealso cref="Connective"/>
/// <seealso cref="ILinguisticVariable"/>
/// <remarks>
/// The associated linguistic base defines the default resolution context for propositions constructed through this
/// interface. Concrete implementations may also provide additional methods that allow propositions to be resolved
/// against a linguistic base supplied explicitly, as defined by <see cref="IContextFreeRule{T}"/>.
/// </remarks>
public interface IContextBoundRule<out T> : IRule where T : class, IContextBoundRule<T>
{
    /// <summary>
    /// Gets the linguistic base used by this rule to resolve linguistic variables and their associated
    /// linguistic terms.
    /// </summary>
    ILinguisticBase LinguisticBase { get; }

    /// <summary>
    /// Appends a proposition to the rule premise using the <see cref="Connective.If">If connective</see>, marking
    /// the start of the premise.
    /// </summary>
    /// <param name="variableName">The name of the linguistic variable.</param>
    /// <param name="termName">
    /// The name of the linguistic term whose meaning is defined by the specified linguistic variable.
    /// </param>
    /// <param name="hedgeType">The linguistic hedge applied to the proposition.</param>
    /// <returns>The rule instance after appending the proposition, allowing fluent chaining.</returns>
    T If(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    /// <summary>
    /// Appends a <b>negated</b> proposition to the rule premise using the <see cref="Connective.If">If connective</see>,
    /// marking the start of the premise.
    /// </summary>
    /// <param name="variableName">The name of the linguistic variable.</param>
    /// <param name="termName">
    /// The name of the linguistic term whose meaning is defined by the specified linguistic variable.
    /// </param>
    /// <param name="hedgeType">The linguistic hedge applied to the proposition.</param>
    /// <returns>The rule instance after appending the proposition, allowing fluent chaining.</returns>
    T IfNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    /// <summary>
    /// Appends a proposition to the rule premise using the <see cref="Connective.And">AND connective</see>, joining it
    /// with the preceding proposition.
    /// </summary>
    /// <param name="variableName">The name of the linguistic variable.</param>
    /// <param name="termName">
    /// The name of the linguistic term whose meaning is defined by the specified linguistic variable.
    /// </param>
    /// <param name="hedgeType">The linguistic hedge applied to the proposition.</param>
    /// <returns>The rule instance after appending the proposition, allowing fluent chaining.</returns>
    T And(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    /// <summary>
    /// Appends a <b>negated</b> proposition to the rule premise using the <see cref="Connective.And">AND connective</see>,
    /// joining it with the preceding proposition.
    /// </summary>
    /// <param name="variableName">The name of the linguistic variable.</param>
    /// <param name="termName">
    /// The name of the linguistic term whose meaning is defined by the specified linguistic variable.
    /// </param>
    /// <param name="hedgeType">The linguistic hedge applied to the proposition.</param>
    /// <returns>The rule instance after appending the proposition, allowing fluent chaining.</returns>
    T AndNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    /// <summary>
    /// Appends a proposition to the rule premise using the <see cref="Connective.Or">OR connective</see>, joining it
    /// with the preceding proposition.
    /// </summary>
    /// <param name="variableName">The name of the linguistic variable.</param>
    /// <param name="termName">
    /// The name of the linguistic term whose meaning is defined by the specified linguistic variable.
    /// </param>
    /// <param name="hedgeType">The linguistic hedge applied to the proposition.</param>
    /// <returns>The rule instance after appending the proposition, allowing fluent chaining.</returns>
    T Or(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    /// <summary>
    /// Appends a <b>negated</b> proposition to the rule premise using the <see cref="Connective.Or">OR connective</see>,
    /// joining it with the preceding proposition.
    /// </summary>
    /// <param name="variableName">The name of the linguistic variable.</param>
    /// <param name="termName">
    /// The name of the linguistic term whose meaning is defined by the specified linguistic variable.
    /// </param>
    /// <param name="hedgeType">The linguistic hedge applied to the proposition.</param>
    /// <returns>The rule instance after appending the proposition, allowing fluent chaining.</returns>
    T OrNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None);
}