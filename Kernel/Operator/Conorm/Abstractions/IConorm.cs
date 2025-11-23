using Kernel.Number;
using Kernel.Operator.Conorm.Implementations.Canonical;
using Kernel.Operator.Conorm.Implementations.Parameterized;

namespace Kernel.Operator.Conorm.Abstractions;

/// <summary>
/// Represents a Triangular Conorm (T-Conorm), a binary operation commonly used in fuzzy logic
/// to model the union (<b>OR</b> operation) between two fuzzy sets.
/// The operation can be either strictly binary (see: <see cref="Implementations.Canonical.Conorm"/>) or
/// can be parameterized by a constant value <i>β</i> (see: <see cref="PConorm"/>).
/// </summary>
/// <remarks>
/// A T-Conorm is a commutative, associative, and monotonically increasing operation that satisfies the identity law:
/// <i>⊥(x, 0) = x</i> (<i>A ∪ ∅ = A</i>).
/// </remarks>
public interface IConorm
{
    /// <summary>
    /// Computes the union of two fuzzy numbers using the T-Conorm.
    /// </summary>
    /// <param name="x">The first <see cref="FuzzyNumber"/>.</param>
    /// <param name="y">The second <see cref="FuzzyNumber"/>.</param>
    /// <returns>
    /// A <see cref="FuzzyNumber"/> representing the result of the T-Conorm operation.
    /// </returns>
    /// <example>
    /// Examples of T-Conorms:
    /// <list type="bullet">
    /// <item><see cref="Implementations.Canonical.Conorm.Maximum">Minimum</see>: <i>⊥(x, y) = Max(x, y)</i>.</item>
    /// <item><see cref="Implementations.Canonical.Conorm.ProbabilisticSum">Product</see> <i>⊥(x, y) = x + y - x ⋅ y</i>.</item>
    /// <item><see cref="PIntersector.Hamacher">Hamacher</see>: <i>⊥₆(x, y) = (x + y + (β - 1) ⋅ x ⋅ y) / (1 + β ⋅ x ⋅ y)</i>.</item>
    /// </list>
    /// </example>
    /// <seealso cref="Conorm"/>
    /// <seealso cref="PConorm"/>
    FuzzyNumber Union(FuzzyNumber x, FuzzyNumber y);
}