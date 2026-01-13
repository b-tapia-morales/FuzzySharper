using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Proposition.Abstractions;
using Reasoning.Proposition.Components;

namespace Reasoning.Rule.Abstractions;

/// <summary>
/// <para>
/// Represents a context-free rule builder that provides a fluent API for constructing rule premises using linguistic
/// variables and terms resolved through linguistic bases supplied explicitly.
/// </para>
/// Each proposition is constructed by rule-building methods that explicitly require a linguistic base to resolve the
/// linguistic variables, allowing a single rule to combine propositions drawn from one or more linguistic domains.
/// </summary>
/// <typeparam name="T">
/// Represents the concrete rule type and enables fluent chaining while preserving access to all <see cref="IRule"/>
/// members and implementation-specific functionality.
/// </typeparam>
/// <example>
/// <code>
/// // IF Brightness IS Dark (camera vision)
/// // AND Brightness IS Uncomfortable (user ergonomics)
/// // AND Time of Day IS Night
/// rule
///     .If(CameraVision, "Brightness", "Dark")
///     .And(UserErgonomics, "Brightness", "Uncomfortable")
///     .And(Environment, "Time of Day", "Night")
/// // The rule conclusion follows here.
/// </code>
/// </example>
/// <seealso cref="IProposition"/>
/// <seealso cref="Connective"/>
/// <seealso cref="ILinguisticBase"/>
public interface IContextFreeRule<out T> : IRule where T : class, IContextFreeRule<T>
{
    /// <summary>
    /// Appends a proposition to the rule premise using the <see cref="Connective.If">If connective</see>, marking
    /// the start of the premise.
    /// </summary>
    /// <param name="linguisticBase">The resolution context for the specified linguistic variable.</param>
    /// <param name="variableName">The name of the linguistic variable.</param>
    /// <param name="termName">
    /// The name of the linguistic term whose meaning is defined by the specified linguistic variable.
    /// </param>
    /// <param name="hedgeType">The linguistic hedge applied to the proposition.</param>
    /// <returns>The rule instance after appending the proposition, allowing fluent chaining.</returns>
    T If(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    /// <summary>
    /// Appends a <b>negated</b> proposition to the rule premise using the <see cref="Connective.If">If connective</see>,
    /// marking the start of the premise.
    /// </summary>
    /// <param name="linguisticBase">The resolution context for the specified linguistic variable.</param>
    /// <param name="variableName">The name of the linguistic variable.</param>
    /// <param name="termName">
    /// The name of the linguistic term whose meaning is defined by the specified linguistic variable.
    /// </param>
    /// <param name="hedgeType">The linguistic hedge applied to the proposition.</param>
    /// <returns>The rule instance after appending the proposition, allowing fluent chaining.</returns>
    T IfNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    /// <summary>
    /// Appends a proposition to the rule premise using the <see cref="Connective.And">AND connective</see>, joining it
    /// with the preceding proposition.
    /// </summary>
    /// <param name="linguisticBase">The resolution context for the specified linguistic variable.</param>
    /// <param name="variableName">The name of the linguistic variable.</param>
    /// <param name="termName">
    /// The name of the linguistic term whose meaning is defined by the specified linguistic variable.
    /// </param>
    /// <param name="hedgeType">The linguistic hedge applied to the proposition.</param>
    /// <returns>The rule instance after appending the proposition, allowing fluent chaining.</returns>
    T And(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    /// <summary>
    /// Appends a <b>negated</b> proposition to the rule premise using the <see cref="Connective.And">AND connective</see>,
    /// joining it with the preceding proposition.
    /// </summary>
    /// <param name="linguisticBase">The resolution context for the specified linguistic variable.</param>
    /// <param name="variableName">The name of the linguistic variable.</param>
    /// <param name="termName">
    /// The name of the linguistic term whose meaning is defined by the specified linguistic variable.
    /// </param>
    /// <param name="hedgeType">The linguistic hedge applied to the proposition.</param>
    /// <returns>The rule instance after appending the proposition, allowing fluent chaining.</returns>
    T AndNot(ILinguisticBase linguisticBase, string variableName, string termName,
        HedgeType hedgeType = HedgeType.None);

    /// <summary>
    /// Appends a proposition to the rule premise using the <see cref="Connective.Or">OR connective</see>, joining it
    /// with the preceding proposition.
    /// </summary>
    /// <param name="linguisticBase">The resolution context for the specified linguistic variable.</param>
    /// <param name="variableName">The name of the linguistic variable.</param>
    /// <param name="termName">
    /// The name of the linguistic term whose meaning is defined by the specified linguistic variable.
    /// </param>
    /// <param name="hedgeType">The linguistic hedge applied to the proposition.</param>
    /// <returns>The rule instance after appending the proposition, allowing fluent chaining.</returns>
    T Or(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    /// <summary>
    /// Appends a <b>negated</b> proposition to the rule premise using the <see cref="Connective.Or">OR connective</see>,
    /// joining it with the preceding proposition.
    /// </summary>
    /// <param name="linguisticBase">The resolution context for the specified linguistic variable.</param>
    /// <param name="variableName">The name of the linguistic variable.</param>
    /// <param name="termName">
    /// The name of the linguistic term whose meaning is defined by the specified linguistic variable.
    /// </param>
    /// <param name="hedgeType">The linguistic hedge applied to the proposition.</param>
    /// <returns>The rule instance after appending the proposition, allowing fluent chaining.</returns>
    T OrNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);
}