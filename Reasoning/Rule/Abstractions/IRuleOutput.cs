using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Functional.Abstractions;

namespace Reasoning.Rule.Abstractions;

/// <summary>
/// Represents the consequent of a rule (the <c>THEN</c> clause), defining the variable to which the rule output applies
/// when the rule fires. A rule output may be expressed either as a
/// <see cref="FuzzyProposition">fuzzy proposition</see> (<i>Mamdani-style</i>) or as a
/// <see cref="IFunctionalConsequent">functional consequent</see> (<i>Sugeno-style</i>).
/// </summary>
public interface IRuleOutput
{
    /// <summary>
    /// Gets the identifier of the variable to which this rule output applies.
    /// </summary>
    string Identifier { get; }
    
    /// <summary>
    /// Determines whether this rule output applies to the specified variable identifier.
    /// </summary>
    /// <param name="identifier">
    /// The identifier of the variable to probe against this rule output.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if this rule output applies to the specified variable; otherwise, <see langword="false"/>.
    /// </returns>
    bool Contains(string identifier) => 
        string.Equals(Identifier, identifier, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Creates a new instance that is structurally equivalent to this rule output.
    /// </summary>
    /// <returns>
    /// A new <see cref="IRuleOutput"/> instance that is structurally equivalent to the current one.
    /// </returns>
    IRuleOutput DeepCopy();
}