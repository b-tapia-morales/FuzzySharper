using Kernel.Function.Implementations;
using Kernel.Number;
using Shared.Intervals.Implementations;
using Shared.Options.Implementations;

namespace Kernel.Function.Abstractions;

/// <summary>
/// Represents a <b>Membership Function</b>, a fundamental concept in fuzzy logic used to define
/// the <i>Membership Degree</i> of elements in a <i>Fuzzy Set</i>.
/// A membership function maps elements from the <i>Universe of Discourse</i> to the unit interval [0, 1],
/// expressing the degree to which each element belongs to the fuzzy set.
/// </summary>
public interface IMembershipFunction
{
    /// <summary>
    /// The name of the function.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The maximum membership degree of the function.
    /// </summary>
    double UMax { get; }

    Interval UniverseOfDiscourse { get; }

    /// <summary>
    /// Returns the membership function itself, represented as a <see cref="Func{T,TResult}" /> delegate.
    /// This delegate can be invoked to calculate the membership degree <i>μ(X)</i> of any <i>x ∈ ℝ</i>.
    /// </summary>
    /// <value>The membership function, represented as a <see cref="Func{T,TResult}" /> delegate.</value>
    Func<double, double> PureFunction { get; }

    Func<double, double> PureFunctionClipped { get; }

    #region FunctionProperties

    /// <summary>
    /// Determines whether the function is <b>Symmetric</b>.
    /// A function is said to be symmetric if μ(x + c) = μ(x - c), ∀x ∈ X when evaluated around a center point c.
    /// </summary>
    /// <value>true if the function is Symmetric; otherwise, false</value>
    bool IsVerticallySymmetric { get; }

    bool HasGlobalMaximum { get; }

    bool IsBoundedLeft { get; }

    bool IsBoundedRight { get; }

    bool FloorsLeft { get; }

    bool FloorsRight { get; }

    bool SaturatesLeft { get; }

    bool SaturatesRight { get; }

    bool IsZeroConvergent { get; }

    bool IsStrictlyUnimodal { get; }

    #endregion

    #region MembershipFunctionProperties

    /// <summary>
    /// Determines whether the function is Open Left.
    /// A function is said to be open left if the membership degree for <i>x</i> values
    /// as <i>x</i> approaches -∞ and +∞ is 1 and 0 respectively.
    /// </summary>
    /// <value>true if the function is Open Left; otherwise, false</value>
    bool IsOpenLeft { get; }

    /// <summary>
    /// Determines whether the function is Open Right.
    /// A function is said to be open right if the membership degree for
    /// <i>x</i> values as <i>x</i> approaches -∞ and +∞ is 0 and 1 respectively.
    /// </summary>
    /// <value>true if the function is Open Right; otherwise, false</value>
    bool IsOpenRight { get; }

    /// <summary>
    /// Determines whether the function is <b>Closed</b>.
    /// A function is said to be closed if the membership degree for
    /// <i>x</i> values as <i>x</i> approaches both -∞ and +∞ is 0; in other words,
    /// if it is neither <see cref="IsOpenLeft">Open Left</see> nor <see cref="IsOpenRight">Open Right</see>.
    /// </summary>
    /// <value>true if the function is Closed; otherwise, false</value>
    bool IsClosed { get; }

    /// <summary>
    /// Determines whether the function is <b>Normal</b>.
    /// A function is said to be normal if ∃x ∈ X: μ(x) = 1; in other words,
    /// if there's at least one <i>x</i> value such that its membership degree equals to one.
    /// </summary>
    /// <value>true if the function is Normal; otherwise, false</value>
    bool IsNormal { get; }

    /// <summary>
    /// Determines whether the function is <b>Prototypical</b>.
    /// A function is commonly referred to as prototypical if ∃!x ∈ X: μ(x) = 1; in other words,
    /// if there's one and only one <i>x</i> value such that its membership degree equals to one.
    /// This <i>x</i> value is also referred to as the <b>Prototype</b> of the Set.
    /// </summary>
    /// <value>true if the function is Prototypical; otherwise, false</value>
    bool IsPrototypical { get; }

    #endregion

    #region FunctionIntervals

    /// <summary>
    /// <para>
    /// Returns the leftmost <i>x</i> value that belongs to the <see cref="Peak">Peak interval</see>.
    /// </para>
    /// <list type="bullet">
    /// <listheader>Special cases:</listheader>
    /// <item>
    /// <description>
    /// The function is either <see cref="MembershipFunction.SaturatesLeft">Monotonically Increasing</see>
    /// or <see cref="MembershipFunction.SaturatesRight">Monotonically Decreasing</see> ⇒ <i>x₀ = <c>null</c></i>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>The function is <see cref="IsNormal">Normal</see> ⇒ <i>x₀ = <see cref="CoreLeft"/></i>.</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <value>
    ///     The minimum value allowed for an <i>x</i> value that belongs to the Membership Function's peak interval.
    /// </value>
    Option<double> PeakLeft { get; }

    /// <summary>
    /// <para>
    /// Returns the rightmost <i>x</i> value that belongs to the <see cref="Peak">Peak interval</see>.
    /// </para>
    /// <list type="bullet">
    /// <listheader>Special cases:</listheader>
    /// <item>
    /// <description>
    /// The function is either <see cref="MembershipFunction.SaturatesLeft">Monotonically Increasing</see>
    /// or <see cref="MembershipFunction.SaturatesRight">Monotonically Decreasing</see> ⇒ <i>x₁ = <c>null</c></i>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>The function is <see cref="IsNormal">Normal</see> ⇒ <i>x₁ = <see cref="CoreRight"/></i>.</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <value>
    ///     The maximum value allowed for an <i>x</i> value that belongs to the Membership Function's peak interval.
    /// </value>
    Option<double> PeakRight { get; }

    /// <summary>
    /// Returns the leftmost and rightmost values of the <b>Peak interval</b>,
    /// represented by a <see cref="ValueTuple" />.
    /// The Peak interval is the region of the universe where the Membership Function
    /// reaches its maximum possible value.
    /// </summary>
    /// <value>The interval, represented as a <see cref="ValueTuple" />.</value>
    Option<Interval> Peak { get; }
    
    Option<double> PeakLeftClipped { get; }
    
    Option<double> PeakRightClipped { get; }
    
    Option<Interval> PeakClipped { get; }

    #endregion

    #region MembershipFunctionIntervals

    /// <summary>
    /// Returns the leftmost <i>x</i> value of the Membership Function's <see cref="Support">Support interval</see>.
    /// In case that the function is <see cref="IsOpenLeft">Open Left</see>,
    /// the value will be <see cref="Double.NegativeInfinity" />.
    /// </summary>
    /// <value>
    ///     The minimum value allowed for an <i>x</i> value that belongs to the Membership Function's support interval.
    /// </value>
    double SupportLeft { get; }

    /// <summary>
    /// Returns the rightmost <i>x</i> value of the Membership Function's <see cref="Support">Support interval</see>.
    /// In case that the function is <see cref="IsOpenRight">Open Right</see>,
    /// the value will be <see cref="Double.PositiveInfinity" />.
    /// </summary>
    /// <value>
    ///     The maximum value allowed for an <i>x</i> value that belongs to the Membership Function's support interval.
    /// </value>
    double SupportRight { get; }

    /// <summary>
    /// Returns the leftmost and rightmost values of the <b>Support interval</b>,
    /// represented by a <see cref="ValueTuple" />.
    /// The Support interval is the region of the universe that is characterized by nonzero membership: 0 &lt;  ≤ 1
    /// </summary>
    /// <value>The interval, represented as a <see cref="ValueTuple" />.</value>
    Interval Support { get; }

    double EffectiveSupportLeft { get; }

    double EffectiveSupportRight { get; }

    Interval EffectiveSupport { get; }

    double RestrictedSupportLeft { get; }

    double RestrictedSupportRight { get; }

    Interval RestrictedSupport { get; }

    /// <summary>
    /// <para>
    /// Returns the leftmost <i>x</i> value of the Membership Function's <see cref="Core">Core interval</see>.
    /// </para>
    /// <list type="bullet">
    /// <listheader>Special cases:</listheader>
    /// <item>
    /// <description>The function is not <see cref="IsNormal">Normal</see> ⇒ <i>x₀ = <c>null</c></i>.</description>
    /// </item>
    /// <item>
    /// <description>
    /// The function is either <see cref="MembershipFunction.SaturatesLeft">Monotonically Increasing</see>
    /// or <see cref="MembershipFunction.SaturatesRight">Monotonically Decreasing</see> ⇒ <i>x₀ = <c>null</c></i>.
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    /// <value>
    ///     The minimum value possible for an <i>x</i> value that belongs to the Membership Function's core interval.
    /// </value>
    Option<double> CoreLeft { get; }

    /// <summary>
    /// <para>
    /// Returns the rightmost <i>x</i> value of the Membership Function's <see cref="Core">Core interval</see>.
    /// </para>
    /// <list type="bullet">
    /// <listheader>Special cases:</listheader>
    /// <item>
    /// <description>The function is not <see cref="IsNormal">Normal</see> ⇒ <i>x₁ = <c>null</c></i>.</description>
    /// </item>
    /// <item>
    /// <description>
    /// The function is either <see cref="MembershipFunction.SaturatesLeft">Monotonically Increasing</see>
    /// or <see cref="MembershipFunction.SaturatesRight">Monotonically Decreasing</see> ⇒ <i>x₁ = <c>null</c></i>.
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    /// <value>
    ///     The maximum value possible for an <i>x</i> value that belongs to the Membership Function's core interval.
    /// </value>
    Option<double> CoreRight { get; }

    /// <summary>
    /// Returns the leftmost and rightmost values of the <b>Core interval</b>,
    /// represented by a <see cref="ValueTuple" />.
    /// The Core interval is the region of the universe that where the membership degree equals to one.
    /// </summary>
    /// <value>The interval, represented as a <see cref="ValueTuple" />.</value>
    Option<Interval> Core { get; }

    /// <summary>
    /// Retrieves the leftmost <i>x</i> value of the crossover points.
    /// <para>
    /// <list type="bullet">
    /// <listheader>Special cases:</listheader>
    /// <item>
    /// <description>0.5 > <see cref="UMax"/> ⇒ <i>x₀ = <c>null</c></i>.</description>
    /// </item>
    /// <item>
    /// <description>|0.5 - <see cref="UMax"/>| ≤ <see cref="FuzzyNumber.Epsilon">ε</see> ⇒ <i>x₀ = <see cref="PeakLeft"/></i>.</description>
    /// </item>
    /// <item>
    /// <description>
    /// The function is <see cref="MembershipFunction.SaturatesRight">Monotonically Decreasing</see> ⇒ <i>x₀ = <see cref="double.NegativeInfinity">-∞</see></i>.
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <value>
    ///     The leftmost <i>x</i> value itself if none of the special cases described above apply.
    /// </value>
    double CrossoverLeft { get; }

    /// <summary>
    /// Retrieves the rightmost <i>x</i> value of the crossover points.
    /// <para>
    /// <list type="bullet">
    /// <listheader>Special cases:</listheader>
    /// <item>
    /// <description>0.5 > <see cref="UMax"/> ⇒ <i>x₁ = <c>null</c></i>.</description>
    /// </item>
    /// <item>
    /// <description>|0.5 - <see cref="UMax"/>| ≤ <see cref="FuzzyNumber.Epsilon">ε</see> ⇒ <i>x₁ = <see cref="PeakRight"/></i>.</description>
    /// </item>
    /// <item>
    /// <description>
    /// The function is <see cref="MembershipFunction.SaturatesLeft">Monotonically Increasing</see> ⇒ <i>x₁ = <see cref="double.PositiveInfinity">+∞</see></i>.
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <value>
    ///     The rightmost <i>x</i> value itself if none of the special cases described above apply.
    /// </value>
    double CrossoverRight { get; }

    /// <summary>
    /// Returns the leftmost and rightmost values of the <b>Crossover interval</b>,
    /// represented by a <see cref="ValueTuple" />.
    /// The crossover interval is the region of the universe where ∀x ∈ Aₐ, μ(x) = 0.5;
    /// that is, all <i>x</i> values at which the membership degree μ(x) equals 0.5.
    /// The crossover is the point where the membership function transitions between
    /// lower and higher degrees of truth relative to 0.5.
    /// </summary>
    /// <value>The interval, represented as a <see cref="ValueTuple" />.</value>
    Interval Crossover { get; }

    #endregion

    #region AlphaCuts

    /// <summary>
    /// Retrieves the leftmost <i>x</i> value of the alpha-cut interval at the cut value α.
    /// <para>
    /// <list type="bullet">
    /// <listheader>Special cases:</listheader>
    /// <item>
    /// <description>α > <see cref="UMax"/> ⇒ <i>x₀ = <c>null</c></i>.</description>
    /// </item>
    /// <item>
    /// <description>|α - <see cref="UMax"/>| ≤ <see cref="FuzzyNumber.Epsilon">ε</see> ⇒ <i>x₀ = <see cref="PeakLeft"/></i>.</description>
    /// </item>
    /// <item>
    /// <description>
    /// The function is <see cref="MembershipFunction.SaturatesRight">Monotonically Decreasing</see> ⇒ <i>x₀ = <see cref="double.NegativeInfinity">-∞</see></i>.
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="alpha">
    ///     The cut value α, represented as a <see cref="FuzzyNumber"/>.
    /// </param>
    /// <returns>
    /// The leftmost <i>x</i> value itself if none of the special cases described above apply.
    /// </returns>
    /// <remarks>
    /// Since there's a defined <see cref="FuzzyNumber.implicit operator double(FuzzyNumber)">implicit conversion</see>
    /// to a <see cref="FuzzyNumber"/> for <c>double</c> values within the Unit Interval [0, 1],
    /// it is not strictly necessary to provide an α value as an instance of a Fuzzy Number.
    /// </remarks>
    Option<double> AlphaCutLeft(FuzzyNumber alpha);

    /// <summary>
    /// Retrieves the rightmost <i>x</i> value of the alpha-cut interval at the cut value α.
    /// <para>
    /// <list type="bullet">
    /// <listheader>Special cases:</listheader>
    /// <item>
    /// <description>α > <see cref="UMax"/> ⇒ <i>x₁ = <c>null</c></i>.</description>
    /// </item>
    /// <item>
    /// <description>|α - <see cref="UMax"/>| ≤ <see cref="FuzzyNumber.Epsilon">ε</see> ⇒ <i>x₁ = <see cref="PeakRight"/></i>.</description>
    /// </item>
    /// <item>
    /// <description>
    /// The function is <see cref="MembershipFunction.SaturatesLeft">Monotonically Increasing</see> ⇒ <i>x₁ = <see cref="double.PositiveInfinity">+∞</see></i>.
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="alpha">
    ///     The cut value α, represented as a <see cref="FuzzyNumber"/>.
    /// </param>
    /// <returns>
    /// The rightmost <i>x</i> value itself if none of the special cases described above apply.
    /// </returns>
    /// <remarks>
    /// Since there's a defined <see cref="FuzzyNumber.implicit operator double(FuzzyNumber)">implicit conversion</see>
    /// to a <see cref="FuzzyNumber"/> for <c>double</c> values within the Unit Interval [0, 1],
    /// it is not strictly necessary to provide an α value as an instance of a Fuzzy Number.
    /// </remarks>
    Option<double> AlphaCutRight(FuzzyNumber alpha);

    /// <summary>
    /// Returns the leftmost and rightmost values of the <b>Alpha-cut interval</b>,
    /// represented by a <see cref="ValueTuple" />.
    /// The alpha-cut is the region of the universe where ∀x ∈ Aₐ, μ(x) ≥ α;
    /// that is, all <i>x</i> values such that the membership degree μ(x) is equal to or greater than
    /// the specified membership degree α.
    /// </summary>
    /// <param name="alpha">
    ///     The cut value α, represented as a <see cref="FuzzyNumber"/>.
    /// </param>
    /// <returns>The interval, represented as a <see cref="ValueTuple" />.</returns>
    /// <remarks>
    /// Since there's a defined <see cref="FuzzyNumber.implicit operator double(FuzzyNumber)">implicit conversion</see>
    /// to a <see cref="FuzzyNumber"/> for <c>double</c> values within the Unit Interval [0, 1],
    /// it is not strictly necessary to provide an α value as an instance of a Fuzzy Number.
    /// </remarks>
    Option<Interval> AlphaCut(FuzzyNumber alpha);

    Option<double> AlphaCutLeftClipped(FuzzyNumber alpha);

    Option<double> AlphaCutRightClipped(FuzzyNumber alpha);

    Option<Interval> AlphaCutClipped(FuzzyNumber alpha);

    #endregion

    #region ImplicationMethods

    /// <summary>
    /// <para>
    /// Returns a new membership function generated by performing the Larsen Product
    /// over the original membership function with the scaling factor <i>λ</i>,
    /// represented by a <see cref="FuzzyNumber"/>.
    /// The resulting function is represented as a <see cref="Func{T,TResult}" /> delegate.
    /// </para>
    /// <para>
    /// The Larsen Product modifies the original membership function
    /// by scaling down each membership degree proportionally by the specified scaling factor <i>λ</i>,
    /// resulting in a new membership function: <c>μ'(x) = λ * μ(x)</c>.
    /// </para>
    /// <list type="bullet">
    /// <listheader>Special cases:</listheader>
    /// <item>
    /// <description><i>λ ≈ 0</i> ⇒ The zero function.</description>
    /// </item>
    /// <item>
    /// <description><i>λ ≈ μMax</i> ⇒ The original membership function, unmodified.</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="lambda">
    /// The cut value at which the horizontal cut is performed, represented as a <see cref="FuzzyNumber" />.
    /// </param>
    /// <returns>
    /// A new membership function represented as a <see cref="Func{T,TResult}" /> delegate,
    /// which performs a horizontal cut over the original membership function at the given cut value <i>λ</i>.
    /// </returns>
    /// <seealso cref="IMembershipFunction.PureFunction" />
    Func<double, double> LarsenProduct(FuzzyNumber lambda);

    Func<double, double> LarsenProductClipped(FuzzyNumber lambda);

    /// <summary>
    /// <para>
    /// Returns a new membership function generated by performing the Mamdani Minimum
    /// over the original membership function at the cut value <i>α</i>,
    /// represented by a <see cref="FuzzyNumber"/>.
    /// The resulting function is represented as a <see cref="Func{T,TResult}" /> delegate.
    /// </para>
    /// <para>
    /// The Mamdani Minimum performs a horizontal cut at the specified cut value <i>α</i>,
    /// generating a new function that either evaluates to the original membership function
    /// or the cut value itself depending on whether <i>x</i> is outside or inside the cut value's range,
    /// respectively.
    /// </para>
    /// <list type="bullet">
    /// <listheader>Special cases:</listheader>
    /// <item>
    /// <description><i>α ≈ 0</i> ⇒ The zero function.</description>
    /// </item>
    /// <item>
    /// <description><i>α ≈ μMax</i> ⇒ The original membership function, unmodified.</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="alpha">
    /// The cut value at which the horizontal cut is performed, represented as a <see cref="FuzzyNumber" />.
    /// </param>
    /// <returns>
    /// A new membership function represented as a <see cref="Func{T,TResult}" /> delegate,
    /// which performs a horizontal cut over the original membership function at the given cut value <i>α</i>.
    /// </returns>
    /// <seealso cref="IMembershipFunction.PureFunction" />
    Func<double, double> MamdaniMinimum(FuzzyNumber alpha);

    Func<double, double> MamdaniMinimumClipped(FuzzyNumber alpha);

    #endregion

    #region EvaluationMethods

    /// <summary>
    /// <para>
    /// Returns the membership degree, represented as a <see cref="FuzzyNumber" />, of the <i>x</i> value provided as a
    /// parameter.
    /// </para>
    /// <para>
    /// This method is equivalent to using the <see cref="Func{TResult}.Invoke" /> method on the resulting delegate
    /// of the <see cref="PureFunction" /> method.
    /// </para>
    /// </summary>
    /// <param name="x">The <i>x</i> value</param>
    /// <returns>The membership degree, represented as a <see cref="FuzzyNumber" /></returns>
    /// <seealso cref="PureFunction" />
    FuzzyNumber MembershipDegree(double x);
    
    FuzzyNumber MembershipDegreeClipped(double x);

    #endregion

    /// <summary>
    /// Creates a deep copy of the current membership function.
    /// </summary>
    /// <returns>
    /// A deep copy of the membership function with all of its properties being identical to the original.
    /// </returns>
    IMembershipFunction DeepCopy();

    /// <summary>
    /// Creates a deep copy of the current membership function
    /// while replacing its name with the one specified as a parameter.
    /// </summary>
    /// <param name="name">The new name for the cloned membership function.</param>
    /// <returns>
    /// A deep copy of the membership function with all of its properties being identical to the original,
    /// except for its name.
    /// </returns>
    IMembershipFunction DeepCopy(string name);
}