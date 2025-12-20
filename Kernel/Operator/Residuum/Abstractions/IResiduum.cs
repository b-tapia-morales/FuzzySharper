using Kernel.Operator.Negation.Implementations.Canonical;
using Kernel.Number;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Residuum.Implementations;
using Kernel.Operator.Residuum.Implementations.Parameterized;

namespace Kernel.Operator.Residuum.Abstractions;

/// <summary>
/// Represents the <b>Residuum</b>, a binary operation commonly used in fuzzy logic to model the implication
/// (<b>THEN</b> operator), a conditional relationship between two fuzzy propositions.
/// The operation can be either strictly binary (see: <see cref="Residuum"/>) or
/// can be parameterized by a constant value <i>Ω</i> (see: <see cref="PResiduum"/>).
/// </summary>
/// <remarks>
/// Fuzzy Implications are generalizations of classical implication that can be defined as functions that map elements from
/// I: [0, 1] × [0, 1] → [0, 1]. These implications are categorized based on their construction:
/// <list type="bullet">
/// <item>
/// <p>
/// <b>R-Implications</b>: Derived from a <see cref="INorm">T-Norm</see> <i>T</i> such that
/// <i>I(x, y) = sup {z ∈ [0, 1] | T(x, z) ≤ y}</i>.
/// </p>
/// <p> R-Implications use the <see cref="Negation <i>∼(x) = 1 - x</i>.</p>
/// </item>
/// <item>
/// <p>
/// <b>S-Implications</b>: Derived from a <see cref="IConorm">T-Conorm</see> <i>⊥</i>,
/// where <i>I(x, y) = ⊥(∼x, y)</i>, and ∼x is the <see cref="INegation">Complement</see> of x.
/// </p>
/// <p>S-Implications use the <see cref="Negation <i>∼(x) = 1 - x</i>.</p>
/// </item>
/// <item>
/// <b>QL-Implications</b>: Derived from both a T-Norm <i>T</i> and a T-Conorm <i>⊥</i>, where I(x, y) = ⊥(∼x, ⊤(x, y))
/// </item>
/// </list>
/// <p>Note that the categorizations are not mutually exclusive, as a given implication can belong to more than one category.</p>
/// <p>
/// Fuzzy implications adhere to axioms that come from generalizations of classical logic, such as the dominance of falsity,
/// the neutrality of truth, the identity law, among others. The specific set of axioms being satisfied will depend on the
/// type of Implication.</p>
/// <p>
/// For a formal definition of these axioms, see Section 2, Definition 1, in the paper:
/// <see href="https://www.mdpi.com/1999-4893/16/12/569">
/// "A Survey of Fuzzy Implications: Properties, Classes, and Construction Methods" by Dubois et al. (2023)</see>.
/// </p>
/// <p>
/// For a detailed list of Fuzzy Implications, including their respective categories, and the axioms they adhere to,
/// refer to the material on page 102 in the lecture notes available at:
/// <see href="https://www.is.ovgu.de/is_media/Teaching/Vorlesung+Fuzzy+Sets/fs_part01_fuzzy_set_and_fuzzy_logic.pdf">
/// "Fuzzy Sets and Fuzzy Logic Lecture Notes" by the Otto von Guericke University Magdeburg</see>.
/// </p>
/// </remarks>
public interface IResiduum
{
    /// <summary>
    /// Evaluates the implication operation between two fuzzy numbers.
    /// </summary>
    /// <param name="x">The first <see cref="FuzzyNumber"/>.</param>
    /// <param name="y">The second <see cref="FuzzyNumber"/>.</param>
    /// <returns>The membership value of the implication.</returns>
    FuzzyNumber Implication(FuzzyNumber x, FuzzyNumber y);
}