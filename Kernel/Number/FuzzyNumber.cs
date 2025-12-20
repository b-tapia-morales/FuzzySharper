using System.Globalization;
using Kernel.Operator.Conorm.Implementations.Canonical;
using Kernel.Operator.Negation.Implementations.Canonical;
using Kernel.Operator.Norm.Implementations.Canonical;
using Kernel.Operator.Residuum.Implementations;
using Shared.Approx;
using static Shared.Approx.DoubleApproxExt;

// ReSharper disable ArrangeThisQualifier
// ReSharper disable HeapView.PossibleBoxingAllocation

namespace Kernel.Number;

/// <summary>
/// <para>
/// Embodies the core mathematical concepts that define a membership degree, such as its truth value range
/// μ(x) ∈ [0, 1]; its basic operations - A ∧ B, A ∨ B, ¬A ⇒ 1 - A, and others.
/// </para>
/// <para>
/// In terms of its actual implementation, it's merely a wrapper class of a <see cref="double" /> value, on
/// which the aforementioned operators are applied.
/// </para>
/// </summary>
public readonly struct FuzzyNumber : IComparable<FuzzyNumber>, IComparer<FuzzyNumber>, IEquatable<FuzzyNumber>, IEqualityComparer<FuzzyNumber>
{
    /// <summary>
    /// Represents the smallest possible difference for which a comparison between two Fuzzy Numbers yields
    /// equality.
    /// <remarks>
    /// Two fuzzy numbers are considered to be equal if the absolute difference between their values is equal to or
    /// below this threshold.
    /// This constant accounts for potential numerical inaccuracies or rounding errors
    /// during comparisons.
    /// </remarks>
    /// </summary>
    public const double Epsilon = DefaultTolerance;

    /// <summary>
    /// Represents a Fuzzy Number's smallest possible value.
    /// </summary>
    /// <returns>A Fuzzy Number's smallest possible value.</returns>
    public static FuzzyNumber Min => 
        Of(0);

    /// <summary>
    /// Represents a Fuzzy Number's biggest possible value.
    /// </summary>
    /// <returns>A Fuzzy Number's biggest possible value.</returns>
    public static FuzzyNumber Max => 
        Of(1);

    /// <summary>
    /// The property for the <see cref="double" /> value on which the <see cref="FuzzyNumber" /> operates.
    /// </summary>
    public double Value { get; }

    private FuzzyNumber(double value) => 
        Value = value;

    /// <summary>
    /// <para>
    /// Creates an instance of a Fuzzy Number in a failsafe way.
    /// If the value provided as a parameter is within the range 0 &lt; μ(x) &lt; 1,
    /// the conversion from a <see cref="double" /> value to a <see cref="FuzzyNumber" /> is considered successful;
    /// otherwise, it defaults to either 0 or 1, and the conversion is considered unsuccessful.
    /// </para>
    /// <list type="number">
    /// <listheader>Cases:</listheader>
    /// <item>μ(x) &lt; 0 ⇒ 0; conversion is unsuccessful.</item>
    /// <item>μ(x) &gt; 1 ⇒ 1; conversion is unsuccessful.</item>
    /// <item>0 ≤ μ(x) ≤ 1 ⇒ The <see cref="double" /> value itself; conversion is successful.</item>
    /// <item>
    /// <b>Special case</b>: |μ(x)| &lt; <see cref="Epsilon" /> ∨ |1 - μ(x)| &lt; <see cref="Epsilon" /> ⇒
    /// Defaults to either 0 or 1 respectively; conversion is successful.
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="value">The <see cref="double" /> value.</param>
    /// <param name="number">An instance of a Fuzzy Number</param>
    /// <returns><see langword="true" /> if the conversion is successful; <see langword="false" />, otherwise.</returns>
    public static bool TryCreate(double value, out FuzzyNumber number)
    {
        try
        {
            number = Of(value);
            return true;
        }
        catch (ArgumentException)
        {
            number = value < 0 ? Math.Max(0, value) : Math.Min(1, value);
            return false;
        }
    }

    /// <summary>
    /// <para>
    /// Creates an instance of a Fuzzy Number.
    /// The <see cref="double" /> value must be in the range μ(x) ∈
    /// [0, 1]; otherwise, a <exception cref="ArgumentException" /> will be thrown.
    /// </para>
    /// <para>
    /// <b>Special case</b>: |0 - μ(x)| &lt; <see cref="Epsilon" /> ∨ |1 - μ(x)| &lt; <see cref="Epsilon" />
    /// ⇒ Defaults to either 0 or 1 respectively.
    /// </para>
    /// </summary>
    /// <param name="value">The <see cref="double" /> value.</param>
    /// <returns>An instance of a Fuzzy Number.</returns>
    /// <exception cref="ArgumentException">Thrown if the value is not in the range μ(X) ∈ [0, 1].</exception>
    public static FuzzyNumber Of(double value)
    {
        var boundedValue = value.SnapToBounds(0, 1);
        switch (boundedValue)
        {
            case < 0:
                throw new ArgumentException(
                    $"Value can't be lesser than 0 by a margin greater than ϵ = {DefaultTolerance} (Value provided was: {boundedValue}, margin was: {Math.Abs(boundedValue):f6}.)");
            case > 1:
                throw new ArgumentException(
                    $"Value can't be greater than 1 by a margin greater than ϵ = {DefaultTolerance} (Value provided was: {boundedValue}, margin was: {Math.Abs(1 - boundedValue):f6})");
        }
        return new FuzzyNumber(boundedValue);
    }

    /// <summary>
    /// Represents the negation operation NOT.
    /// The operation defaults to the <see cref="Negation.Standard"/> operator: <i>∼(x) = 1 - x</i>.
    /// </summary>
    /// <param name="x">A fuzzy number</param>
    /// <returns>The resulting fuzzy number after applying the NOT operator.</returns>
    /// <seealso cref="Negation"/>
    public static FuzzyNumber operator !(FuzzyNumber x) => 
        Negation.Standard.Complement(x);

    /// <summary>
    /// Represents the disjunction operation AND.
    /// The operation defaults to the <see cref="Norm.Minimum"/> operator: <i>T(x, y) = Min(x, y)</i>.
    /// </summary>
    /// <param name="x">A fuzzy number</param>
    /// <param name="y">A fuzzy number</param>
    /// <returns>The resulting fuzzy number after applying the NOT operator.</returns>
    /// <seealso cref="Norm"/>
    public static FuzzyNumber operator &(FuzzyNumber x, FuzzyNumber y) => 
        Norm.Minimum.Intersection(x, y);

    /// <summary>
    /// Represents the conjunction operation OR.
    /// The operation defaults to the <see cref="Conorm.Maximum"/> operator: <i>⊥(x, y) = Max(x, y)</i>.
    /// </summary>
    /// <param name="x">A fuzzy number</param>
    /// <param name="y">A fuzzy number</param>
    /// <returns>The resulting fuzzy number after applying the NOT operator.</returns>
    /// <seealso cref="Norm"/>
    public static FuzzyNumber operator |(FuzzyNumber x, FuzzyNumber y) => 
        Conorm.Maximum.Union(x, y);

    /// <summary>
    /// Represents the implication operation THEN.
    /// The operation defaults to the <see cref="Residuum.Godel"/> operator: <i>A => B = A ≤ B ⟹ 1; A > B ⟹ B</i>.
    /// </summary>
    /// <param name="a">A fuzzy number</param>
    /// <param name="b">A fuzzy number</param>
    /// <returns>The resulting fuzzy number after applying the NOT operator.</returns>
    /// <seealso cref="Residuum"/>
    public static FuzzyNumber operator >> (FuzzyNumber a, FuzzyNumber b) => 
        Residuum.Godel.Implication(a, b);

    /// <summary>
    /// Defines an implicit conversion from a <see cref="FuzzyNumber" /> to a <see cref="double" /> value.
    /// </summary>
    /// <param name="a">The <see cref="double" /> value.</param>
    /// <returns>The <see cref="FuzzyNumber" /> value.</returns>
    public static implicit operator double(FuzzyNumber a) =>
        a.Value;

    /// <summary>
    /// Defines an implicit conversion from a <see cref="double" /> value to a <see cref="FuzzyNumber" />.
    /// Note that this value must be in the range μ(x) ∈ [0, 1].
    /// </summary>
    /// <param name="a">The <see cref="double" /> value.</param>
    /// <returns>The <see cref="FuzzyNumber" /> value.</returns>
    public static implicit operator FuzzyNumber(double a) =>
        Of(a);

    /// <summary>
    /// <param>
    /// Compares two fuzzy numbers for equality based on a defined tolerance.
    /// </param>
    /// <param>
    /// Two fuzzy numbers, <paramref name="a"/> and <paramref name="b"/>, are considered equal if the
    /// absolute difference between them is below or equal to <see cref="Epsilon"/>.
    /// </param>
    /// </summary>
    /// <param name="a">A fuzzy number</param>
    /// <param name="b">A fuzzy number</param>
    /// <returns>
    /// <see langword="true"/> if the absolute difference between <paramref name="a"/> and
    /// <paramref name="b"/> is below or equal to <see cref="Epsilon"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(FuzzyNumber a, FuzzyNumber b) =>
        a.Value.RoughlyEquals(b.Value);

    /// <summary>
    /// <param>
    /// Compares two fuzzy numbers for inequality based on a defined tolerance.
    /// </param>
    /// <param>
    /// Two fuzzy numbers, <paramref name="a"/> and <paramref name="b"/>, are considered not equal if the
    /// absolute difference between them is strictly above <see cref="Epsilon"/>.
    /// </param>
    /// </summary>
    /// <param name="a">A fuzzy number</param>
    /// <param name="b">A fuzzy number</param>
    /// <returns>
    /// <see langword="true"/> if the absolute difference between <paramref name="a"/> and
    /// <paramref name="b"/> is strictly above <see cref="Epsilon"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(FuzzyNumber a, FuzzyNumber b) =>
        !a.Value.RoughlyEquals(b.Value);

    public static bool operator <(FuzzyNumber a, FuzzyNumber b) =>
        a.Value.IsRoughlyLesserThan(b.Value);

    public static bool operator <=(FuzzyNumber a, FuzzyNumber b) =>
        a.Value.IsRoughlyLesserOrEqualTo(b.Value);

    public static bool operator >(FuzzyNumber a, FuzzyNumber b) =>
        a.Value.IsRoughlyGreaterThan(b.Value);

    public static bool operator >=(FuzzyNumber a, FuzzyNumber b) =>
        a.Value.IsRoughlyGreaterOrEqualTo(b.Value);

    public override bool Equals(object? obj) =>
        obj is FuzzyNumber other && this == other;

    public bool Equals(FuzzyNumber other) =>
        this == other;

    public bool Equals(FuzzyNumber x, FuzzyNumber y) =>
        x.Equals(y);

    public override int GetHashCode() =>
        Value.GetHashCode();

    public int GetHashCode(FuzzyNumber obj) =>
        obj.GetHashCode();

    public int CompareTo(FuzzyNumber other) =>
        this == other ? 1 : this.Value.CompareTo(other.Value);

    public int Compare(FuzzyNumber x, FuzzyNumber y) =>
        x.CompareTo(y);

    public override string ToString() =>
        Value.ToString(CultureInfo.InvariantCulture);
}