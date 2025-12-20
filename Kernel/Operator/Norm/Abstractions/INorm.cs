using Kernel.Number;
using Kernel.Operator.Norm.Implementations.Canonical;
using Kernel.Operator.Norm.Implementations.Parameterized;

namespace Kernel.Operator.Norm.Abstractions;

/// <summary>
/// Represents the Triangular Norm (T-Norm), a binary operation commonly used in fuzzy logic
/// to model the intersection (<b>AND</b> operation) between two fuzzy sets.
/// The operation can be either strictly binary (see: <see cref="Implementations.Canonical.Norm"/>) or
/// can be parameterized by a constant value <i>α</i> (see: <see cref="PNorm"/>).
/// <remarks>
/// A T-Norm is a commutative, associative, and monotonically increasing operation that satisfies the identity law:
/// <i>T(x, 1) = x</i> (<i>A ∩ X = A</i>).
/// </remarks>
/// </summary>
public interface INorm
{
    /// <summary>
    /// Computes the intersection of two fuzzy numbers using the T-Norm.
    /// </summary>
    /// <param name="x">The first <see cref="FuzzyNumber"/>.</param>
    /// <param name="y">The second <see cref="FuzzyNumber"/>.</param>
    /// <returns>
    /// A <see cref="FuzzyNumber"/> representing the result of the T-Norm operation.
    /// </returns>
    /// <example>
    /// Examples of T-Norms:
    /// <list type="bullet">
    /// <item><see cref="Implementations.Canonical.Norm.Minimum">Minimum</see>: <i>T(x, y) = Min(x, y)</i>.</item>
    /// <item><see cref="Implementations.Canonical.Norm.Product">Product</see> <i>T(x, y) = x ⋅ y</i>.</item>
    /// <item><see cref="Implementations.Canonical.Norm.Minimum">Hamacher</see>: <i>Tₐ(x, y) = (x ⋅ y) / (α + (1 - α) ⋅ (x + y - x ⋅ y))</i>.</item>
    /// </list>
    /// </example>
    /// <seealso cref="Norm"/>
    /// <seealso cref="PNorm"/>
    FuzzyNumber Intersection(FuzzyNumber x, FuzzyNumber y);
}