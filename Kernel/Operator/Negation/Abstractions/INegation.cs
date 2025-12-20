using Kernel.Number;
using Kernel.Operator.Negation.Implementations.Canonical;
using Kernel.Operator.Negation.Implementations.Parameterized;

namespace Kernel.Operator.Negation.Abstractions;

/// <summary>
/// Represents the Fuzzy Complement, a unary operator (the <b>NOT</b> operator) commonly used in fuzzy logic
/// to map a fuzzy number <i>x</i> to its "opposite" <i>∼x</i>,
/// where both values lie within the unit interval <i>[0, 1]</i>.
/// The operation can be either strictly unary (see: <see cref="Implementations.Canonical.Negation"/>) or
/// can be parameterized by a constant value <i>γ</i> (see: <see cref="PNegation"/>).
/// </summary>
/// <remarks>
/// The complement satisfies the following conditions:
/// <list type="bullet">
/// <item>∼(0) = 1</item>
/// <item>∼(1) = 0</item>
/// </list>
/// </remarks>
public interface INegation
{
    /// <summary>
    /// Computes the complement (negation) of a fuzzy number.
    /// </summary>
    /// <param name="x">The <see cref="FuzzyNumber"/> to negate.</param>
    /// <returns>
    /// A <see cref="FuzzyNumber"/> representing the complement of <paramref name="x"/>.
    /// </returns>
    /// <example>
    /// Examples of Complements:
    /// <list type="bullet">
    /// <item><see cref="Implementations.Canonical.Negation.Standard">Standard Negation</see>: <i>∼(x) = 1 - x</i>.</item>
    /// <item><see cref="Negation.RaisedCosine">Cosine Negation</see>: <i>∼(x) =  (1 / 2) ⋅ (1 + Cos(π ⋅ x))</i>.</item>
    /// <item><see cref="PNegator.Sugeno">Sugeno Negation</see>: <i>∼ᵧ(x) =  (1 - x) / (1 - γ ⋅ x)</i>.</item>
    /// </list>
    /// </example>
    FuzzyNumber Complement(FuzzyNumber x);
}