using Kernel.Number;
using Shared.Intervals.Implementations;
using Shared.Options.Implementations;

namespace Kernel.Function.Abstractions;

/// <summary>
/// Represents a <b>Membership Function</b>, a fundamental concept in fuzzy logic used to define the
/// <i>membership degree</i> of elements in a <i>fuzzy set</i>.
/// A membership function maps elements from a <i>Universe of Discourse</i>
/// to the interval [0, μ<sub>Max</sub>], where μ<sub>Max</sub> ≤ 1,
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

    /// <summary>
    /// The <b>Universe of Discourse</b> is the interval of real values over which the membership function is
    /// semantically defined. Any portion of the function that lies outside this interval is interpreted as
    /// non-membership; that is, <c>μ(x) = 0</c>.
    /// </summary>
    /// <remarks>
    /// In fuzzy logic, a membership function does not define its Universe of Discourse independently.
    /// Instead, it's inherited from the linguistic variable to which the function belongs to; consequently, it
    /// cannot directly be specified the Universe of Discourse from the function itself.
    /// </remarks>
    Interval UniverseOfDiscourse { get; }

    /// <summary>
    /// The underlying membership function, represented as a <see cref="Func{T,TResult}" /> delegate,
    /// used to compute the membership degree <c>μ(x)</c> for any <c>x ∈ ℝ</c>.
    /// </summary>
    Func<double, double> PureFunction { get; }

    /// <summary>
    /// <para>
    /// Returns a clipped variant of <see cref="PureFunction"/> whose evaluation is restricted to the
    /// <see cref="UniverseOfDiscourse">Universe of Discourse</see>.
    /// </para>
    /// Formally, <c>∀x ∉ UoD ⇒ μ(x) = 0</c>; that is, any input value outside the
    /// Universe of Discourse evaluates to zero.
    /// </summary>
    Func<double, double> PureFunctionClipped { get; }

    #region FunctionProperties

    /// <summary>
    /// Determines whether the function is <b>vertically symmetric</b>.
    /// A function is vertically symmetric if <c>∀x ∈ ℝ | μ(c + x) = μ(c − x)</c>;
    /// that is, membership degrees are mirrored around a central point <c>c</c>.
    /// </summary>
    bool IsVerticallySymmetric { get; }

    /// <summary>
    /// Determines whether the membership function reaches a global maximum value.
    /// That is, <c>∃x ∈ ℝ | μ(x) = μMax</c>.
    /// </summary>
    bool HasGlobalMaximum { get; }

    /// <summary>
    /// Determines whether the membership function is <b>bounded on the left</b>.
    /// A function is bounded on the left if there exists a finite <c>x₀</c> such that
    /// <c>μ(x) = 0</c> for all <c>x &lt; x₀</c>.
    /// </summary>
    bool IsBoundedLeft { get; }

    /// <summary>
    /// Determines whether the membership function is <b>bounded on the right</b>.
    /// A function is bounded on the right if there exists a finite <c>x₁</c> such that
    /// <c>μ(x) = 0</c> for all <c>x &gt; x₁</c>.
    /// </summary>
    bool IsBoundedRight { get; }

    /// <summary>
    /// Determines whether the membership function floors to zero on the left.
    /// That is, Lim<sub> x→−∞ </sub>μ(x) = 0
    /// </summary>
    bool FloorsLeft { get; }

    /// <summary>
    /// Determines whether the membership function floors to zero on the right.
    /// That is, Lim<sub> x→+∞ </sub>μ(x) = 0
    /// </summary>
    bool FloorsRight { get; }

    /// <summary>
    /// Determines whether the membership function saturates on the left.
    /// That is, Lim<sub> x→−∞ </sub>μ(x) = μ<sub>max</sub>
    /// </summary>
    bool SaturatesLeft { get; }

    /// <summary>
    /// Determines whether the membership function saturates on the right.
    /// That is, Lim<sub> x→+∞ </sub>μ(x) = μ<sub>max</sub>
    /// </summary>
    bool SaturatesRight { get; }

    /// <summary>
    /// Determines whether the membership function converges to zero from both sides.
    /// That is, Lim<sub> x→±∞ </sub>μ(x) = 0.
    /// </summary>
    /// <seealso cref="IsBoundedLeft"/>
    /// <seealso cref="IsBoundedRight"/>
    bool IsZeroConvergent { get; }

    /// <summary>
    /// Determines whether the membership function is strictly unimodal.
    /// A function is strictly unimodal if <c>∃!x ∈ ℝ | μ(x) = μMax</c>; that is, it reaches its maximum value
    /// at <b>one and only one</b> point, and is strictly increasing and strictly decreasing before and after that point,
    /// respectively.
    /// </summary>
    bool IsStrictlyUnimodal { get; }

    #endregion

    #region MembershipFunctionProperties

    /// <summary>
    /// Determines whether the membership function is <b>Open Left</b>. A function is considered Open Left if
    /// and only if it <see cref="IsNormal">is normal</see>, <c>μ(x) → 1</c> as <c>x → −∞</c>, and
    /// <c>μ(x) → 0</c> as <c>x → +∞</c>.
    /// </summary>
    bool IsOpenLeft { get; }

    /// <summary>
    /// Determines whether the membership function is <b>Open Left</b>. A function is considered Open Left if
    /// and only if it <see cref="IsNormal">is normal</see>, <c>μ(x) → 1</c> as <c>x → +∞</c>,
    /// and <c>μ(x) → 0</c> as <c>x → -∞</c>.
    /// </summary>
    bool IsOpenRight { get; }

    /// <summary>
    /// Determines whether the membership function is <b>Closed</b>.
    /// A closed function is equivalent to a <see cref="IsZeroConvergent">zero-convergent</see> one.
    /// </summary>
    bool IsClosed { get; }

    /// <summary>
    /// Determines whether the membership function is <b>Normal</b>.
    /// A function is normal if <c>∃x ∈ ℝ | μ(x) = 1</c>; in other words, if there's <b>at least one</b> <i>x</i> value
    /// that reaches full membership.
    /// </summary>
    /// <remarks>
    /// A function that only <i>approaches</i> full membership asymptotically (that is, <c>sup μ(x) = 1</c>,
    /// but <c>∀x ∈ ℝ | μ(x) &lt; 1</c>) is <b>not</b> considered normal.
    /// In such cases, the maximum membership degree is never reached.
    /// </remarks>
    /// <seealso cref="Core"/>
    bool IsNormal { get; }

    /// <summary>
    /// Determines whether the membership function is <b>Prototypical</b>.
    /// A function is prototypical if <c>∃!x ∈ ℝ | μ(x) = 1</c>; in other words, if there's <b>one and only one</b>
    /// <i>x</i> value that reaches full membership.
    /// </summary>
    bool IsPrototypical { get; }

    #endregion

    #region FunctionIntervals

    /// <summary>
    /// Returns the leftmost <i>x</i> value of the <see cref="Peak">Peak interval</see>.
    /// <b>Special cases:</b>
    /// <list type="number">
    /// <item>
    /// If the function is <see cref="IsNormal">normal</see>: <c>x₀ = CoreLeft</c>.
    /// </item>
    /// <item>
    /// If the <see cref="Peak">Peak interval</see> is not <see cref="Interval.IsBoundedLeft">left bounded</see>:
    /// <c>x₀ = ∅</c>.
    /// </item>
    /// </list>
    /// </summary>
    /// <seealso cref="CoreLeft"/>
    Option<double> PeakLeft { get; }

    /// <summary>
    /// Returns the rightmost <i>x</i> value of the <see cref="Peak">Peak interval</see>.
    /// <b>Special cases:</b>
    /// <list type="number">
    /// <item>
    /// If the function is <see cref="IsNormal">normal</see>: <c>x₁ = CoreRight</c>.
    /// </item>
    /// <item>
    /// If the <see cref="Peak">Peak interval</see> is not <see cref="Interval.IsBoundedRight">right bounded</see>:
    /// <c>x₁ = ∅</c>.
    /// </item>
    /// </list>
    /// </summary>
    /// <seealso cref="CoreRight"/>
    Option<double> PeakRight { get; }

    /// <summary>
    /// Returns the <b>Peak interval</b>, defined as the interval <c>[a, b]</c> such that
    /// <c>μ(x) = μMax</c> for all <c>x ∈ [a, b]</c>.
    /// In other words, the region of the universe where the membership function reaches its maximum value.
    /// <b>Special cases:</b>
    /// <list type="number">
    /// <item>
    /// If the function does not have a <see cref="HasGlobalMaximum">global maximum</see>: Peak = ∅.
    /// </item>
    /// <item>
    /// If the function is <see cref="IsNormal">normal</see>: Peak = <see cref="Core"/>.
    /// </item>
    /// </list>
    /// </summary>
    Option<Interval> Peak { get; }

    /// <summary>
    /// Returns the leftmost <i>x</i> value of the <b>Clipped Peak interval</b>.
    /// The clipping behavior is defined as follows:
    /// If <c>PeakLeft &lt; UoD.LowerBound</c>, then <c>x₀ = UoD.LowerBound</c>; otherwise, <c>x₀ = PeakLeft</c>.
    /// </summary>
    Option<double> PeakLeftClipped { get; }

    /// <summary>
    /// Returns the rightmost <i>x</i> value of the <b>Clipped Peak interval</b>.
    /// The clipping behavior is defined as follows:
    /// If <c>PeakRight &gt; UoD.UpperBound</c>, then <c>x₁ = UoD.UpperBound</c>; otherwise, <c>x₁ = PeakRight</c>.
    /// </summary>
    /// <seealso cref="UniverseOfDiscourse"/>
    /// <seealso cref="PeakRight"/>
    Option<double> PeakRightClipped { get; }

    /// <summary>
    /// Returns the <b>Clipped Peak interval</b>, obtained by clipping the <see cref="Peak">Peak interval</see>
    /// to the bounds of the <see cref="UniverseOfDiscourse">Universe of Discourse</see>.
    /// </summary>
    /// <seealso cref="PeakLeftClipped"/>
    /// <seealso cref="PeakRightClipped"/>
    Option<Interval> PeakClipped { get; }

    #endregion

    #region MembershipFunctionIntervals

    /// <summary>
    /// Returns the leftmost <i>x</i> value of the <see cref="Support">Support</see>.
    /// If the function is <see cref="IsBoundedLeft">bounded on the left</see>, this is a finite value;
    /// otherwise, <c>x₀ = -∞</c>
    /// </summary>
    double SupportLeft { get; }

    /// <summary>
    /// Returns the rightmost <i>x</i> value of the <see cref="Support">Support</see>.
    /// If the function is <see cref="IsBoundedRight">bounded on the right</see>, this is a finite value;
    /// otherwise, <c>x₁ = +∞</c>
    /// </summary>
    double SupportRight { get; }

    /// <summary>
    /// Returns the <b>Support</b> of the membership function, defined as the set of all <i>x</i> values for which
    /// <c>μ(x) &gt; 0</c>. The support may be finite, unbounded on one of its sides, or unbounded on both.
    /// </summary>
    /// <seealso cref="SupportLeft"/>
    /// <seealso cref="SupportRight"/>
    Interval Support { get; }

    /// <summary>
    /// Returns the leftmost <i>x</i> value of the <see cref="EffectiveSupport">Effective Support</see>.
    /// The value is determined as follows:
    /// <list type="number">
    /// <item>
    /// If the function is <see cref="IsBoundedLeft">bounded on the left</see>: <c>x₀ = SupportLeft</c>.
    /// </item>
    /// <item>
    /// If the function <see cref="FloorsLeft">floors on the left</see>: <c>x₀ = inf { x ∈ ℝ | μ(x) → 0 }</c>;
    /// that is, the leftmost <i>x</i> value at which <c>μ(x)</c> approaches <c>0</c>.
    /// </item>
    /// <item>
    /// If the function <see cref="SaturatesLeft">saturates on the left</see>: <c>x₀ = inf { x ∈ ℝ | μ(x) → μMax }</c>;
    /// that is, the leftmost <i>x</i> value at which <c>μ(x)</c> reaches or begins to approach <c>μMax</c>.
    /// </item>
    /// </list>
    /// </summary>
    /// <seealso cref="SupportLeft"/>
    /// <seealso cref="Shared.Approx.DoubleApproxExt.extension(double).IsRoughlyZero">IsRoughlyZero</seealso>
    /// <seealso cref="Shared.Approx.DoubleApproxExt.extension(double).IsRoughlyOne">IsRoughlyOne</seealso>
    double EffectiveSupportLeft { get; }

    /// <summary>
    /// Returns the rightmost <i>x</i> value of the <see cref="EffectiveSupport">Effective Support</see>.
    /// The value is determined as follows:
    /// <list type="number">
    /// <item>
    /// If the function is <see cref="IsBoundedRight">bounded on the right</see>: <c>x₁ = SupportRight</c>.
    /// </item>
    /// <item>
    /// If the function <see cref="FloorsRight">floors on the right</see>: <c>x₁ = sup { x ∈ ℝ | μ(x) → 0 }</c>;
    /// that is, the rightmost <i>x</i> value at which <c>μ(x)</c> approaches <c>0</c>.
    /// </item>
    /// <item>
    /// If the function <see cref="SaturatesRight">saturates on the right</see>: <c>x₁ = sup { x ∈ ℝ | μ(x) → μMax }</c>;
    /// that is, the leftmost <i>x</i> value at which <c>μ(x)</c> reaches or begins to approach <c>μMax</c>.
    /// </item>
    /// </list>
    /// </summary>
    /// <seealso cref="SupportRight"/>
    /// <seealso cref="Shared.Approx.DoubleApproxExt.extension(double).IsRoughlyZero">IsRoughlyZero</seealso>
    /// <seealso cref="Shared.Approx.DoubleApproxExt.extension(double).IsRoughlyOne">IsRoughlyOne</seealso>
    double EffectiveSupportRight { get; }

    /// <summary>
    /// Returns the <b>Effective Support</b> of the membership function, defined as a finite interval that captures the
    /// region where the function most meaningfully contributes.
    /// The effective support is a bounded approximation of the <see cref="Support">Support</see>,
    /// determined by the function’s shape.
    /// </summary>
    /// <seealso cref="EffectiveSupportLeft"/>
    /// <seealso cref="EffectiveSupportRight"/>
    Interval EffectiveSupport { get; }

    /// <summary>
    /// Returns the leftmost <i>x</i> value of the <see cref="RestrictedSupport">Restricted Support</see>.
    /// If <c>EffectiveSupportLeft &lt; UoD.LowerBound</c>, then <c>x₀ = UoD.LowerBound</c>;
    /// otherwise, <c>x₀ = EffectiveSupportLeft</c>.
    /// </summary>
    /// <seealso cref="EffectiveSupportLeft"/>
    /// <seealso cref="UniverseOfDiscourse"/>
    /// <seealso cref="Interval.LowerBound"/>
    double RestrictedSupportLeft { get; }

    /// <summary>
    /// Returns the rightmost <i>x</i> value of the <see cref="RestrictedSupport">Restricted Support</see>.
    /// If <c>EffectiveSupportRight &gt; UoD.UpperBound</c>, then <c>x₁ = UoD.UpperBound</c>;
    /// otherwise, <c>x₁ = EffectiveSupportRight</c>.
    /// </summary>
    /// <seealso cref="EffectiveSupportRight"/>
    /// <seealso cref="UniverseOfDiscourse"/>
    /// <seealso cref="Interval.UpperBound"/>
    double RestrictedSupportRight { get; }

    /// <summary>
    /// Returns the <b>Restricted Support</b> of the membership function. The restricted support is obtained by
    /// restricting the <see cref="EffectiveSupport">Effective Support</see> to the
    /// <see cref="UniverseOfDiscourse">Universe of Discourse</see>.
    /// </summary>
    /// <seealso cref="RestrictedSupportLeft"/>
    /// <seealso cref="RestrictedSupportRight"/>
    Interval RestrictedSupport { get; }

    /// <summary>
    /// Returns the leftmost <i>x</i> value of the <see cref="Core">Core</see>.
    /// If the function is not <see cref="IsNormal">normal</see>: <c>x₀ = ∅</c>; otherwise, <c>x₀ = PeakLeft</c>.
    /// <seealso cref="PeakLeft"/>
    /// </summary>
    Option<double> CoreLeft { get; }

    /// <summary>
    /// Returns the rightmost <i>x</i> value of the <see cref="Core">Core</see>.
    /// If the function is not <see cref="IsNormal">normal</see>: <c>x₁ = ∅</c>; otherwise, <c>x₁ = PeakRight</c>.
    /// <seealso cref="PeakRight"/>
    /// </summary>
    Option<double> CoreRight { get; }

    /// <summary>
    /// Returns the <b>Core</b> of the membership function, defined as the interval of all <i>x</i> values for which
    /// <c>μ(x) = 1</c>.
    /// <seealso cref="CoreLeft"/>
    /// <seealso cref="CoreRight"/>
    /// </summary>
    Option<Interval> Core { get; }

    /// <summary>
    /// Returns the leftmost <i>x</i> value of the <see cref="Crossover"/>.
    /// If the function <see cref="SaturatesRight">saturates on the right</see>: <c>x₀ = −∞</c>;
    /// otherwise, it's the smallest <i>x</i> such that <c>μ(x) = μMax / 2</c>.
    /// </summary>
    double CrossoverLeft { get; }

    /// <summary>
    /// Returns the rightmost <i>x</i> value of the <see cref="Crossover"/>.
    /// If the function <see cref="SaturatesRight">saturates on the right</see>: <c>x₁ = +∞</c>;
    /// otherwise, it's the largest <i>x</i> such that <c>μ(x) = μMax / 2</c>.
    /// </summary>
    double CrossoverRight { get; }

    /// <summary>
    /// Returns the <b>Crossover</b> of the membership function.
    /// The crossover is the region where the function transitions between lower and higher degrees of truth
    /// relative to half its maximum value, defined as the interval <c>[a, b]</c> such that
    /// <c>μ(x) = μMax / 2</c> for all <c>x ∈ [a, b]</c>.
    /// </summary>
    /// <seealso cref="CrossoverLeft"/>
    /// <seealso cref="CrossoverRight"/>
    Interval Crossover { get; }

    #endregion

    #region AlphaCuts

    /// <summary>
    /// Returns the leftmost <i>x</i> value of the α-cut at the cut value <i>α</i>.
    /// This value is determined as follows:
    /// <list type="number">
    /// <item>
    /// If <c>α &gt; μMax</c>: <c>x₀ = ∅</c>
    /// </item>
    /// <item>
    /// If the function <see cref="SaturatesLeft">saturates on the left</see>: <c>x₀ = -∞</c>
    /// </item>
    /// <item>
    /// Otherwise, <c>x₀</c> is the smallest <i>x</i> such that <c>μ(x) ≥ α</c>.
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="alpha">
    /// The cut value <i>α</i>, represented as a <see cref="FuzzyNumber"/>.
    /// </param>
    /// <remarks>
    /// The cut value <i>α</i> may be provided as a <see cref="double"/> within the unit interval [0, 1],
    /// which is implicitly convertible to <see cref="FuzzyNumber"/>.
    /// </remarks>
    Option<double> AlphaCutLeft(FuzzyNumber alpha);

    /// <summary>
    /// Returns the rightmost <i>x</i> value of the α-cut at the cut value <i>α</i>.
    /// This value is determined as follows:
    /// <list type="number">
    /// <item>
    /// If <c>α &gt; μMax</c>: <c>x₁ = ∅</c>
    /// </item>
    /// <item>
    /// If the function <see cref="SaturatesRight">saturates on the right</see>: <c>x₁ = +∞</c>
    /// </item>
    /// <item>
    /// Otherwise, <c>x₁</c> is the largest <i>x</i> such that <c>μ(x) ≥ α</c>.
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="alpha">
    /// The cut value <i>α</i>, represented as a <see cref="FuzzyNumber"/>.
    /// </param>
    /// <remarks>
    /// The cut value <i>α</i> may be provided as a <see cref="double"/> within the unit interval [0, 1],
    /// which is implicitly convertible to <see cref="FuzzyNumber"/>.
    /// </remarks>
    Option<double> AlphaCutRight(FuzzyNumber alpha);

    /// <summary>
    /// <para>
    /// Returns the <b>α-cut</b> of the membership function at the cut value <i>α</i>.
    /// </para>
    /// The α-cut is defined as the interval <c>[a, b]</c> such that <c>μ(x) ≥ α</c> for all <c>x ∈ [a, b]</c>.
    /// </summary>
    /// <param name="alpha">
    /// The cut value <i>α</i>, represented as a <see cref="FuzzyNumber"/>.
    /// </param>
    /// <remarks>
    /// <b>Special case</b>: If <c>α &gt; μMax</c>, the α-cut is empty.
    /// <b>Implicit conversion</b>: The cut value <i>α</i> may be provided as a <see cref="double"/> within the
    /// unit interval [0, 1], which is implicitly convertible to <see cref="FuzzyNumber"/>.
    /// </remarks>
    Option<Interval> AlphaCut(FuzzyNumber alpha);

    /// <summary>
    /// <para>
    /// Returns the leftmost <i>x</i> value of the <b>Clipped α-cut</b> at the cut value <i>α</i>.
    /// </para>
    /// If <c>AlphaCutRight &lt; UoD.LowerBound</c>, the clipped α-cut is empty; otherwise, the value is the
    /// smallest <i>x</i> of the α-cut restricted to the <see cref="UniverseOfDiscourse">Universe of Discourse</see>.
    /// </summary>
    /// <param name="alpha">
    /// The cut value <i>α</i>, represented as a <see cref="FuzzyNumber"/>.
    /// </param>
    /// <remarks>
    /// The cut value <i>α</i> may be provided as a <see cref="double"/> within the unit interval [0, 1],
    /// which is implicitly convertible to <see cref="FuzzyNumber"/>.
    /// </remarks>
    /// <seealso cref="AlphaCutLeft"/>
    /// <seealso cref="AlphaCut"/>
    /// <seealso cref="UniverseOfDiscourse"/>
    /// <seealso cref="Interval.LowerBound"/>
    Option<double> AlphaCutLeftClipped(FuzzyNumber alpha);

    /// <summary>
    /// <para>
    /// Returns the rightmost <i>x</i> value of the <b>Clipped α-cut</b> at the cut value <i>α</i>.
    /// </para>
    /// If <c>AlphaCutLeft &gt; UoD.UpperBound</c>, the clipped α-cut is empty; otherwise, the value is the
    /// largest <i>x</i> of the α-cut restricted to the <see cref="UniverseOfDiscourse">Universe of Discourse</see>.
    /// </summary>
    /// <param name="alpha">
    /// The cut value <i>α</i>, represented as a <see cref="FuzzyNumber"/>.
    /// </param>
    /// <remarks>
    /// The cut value <i>α</i> may be provided as a <see cref="double"/> within the unit interval [0, 1],
    /// which is implicitly convertible to <see cref="FuzzyNumber"/>.
    /// </remarks>
    /// <seealso cref="AlphaCutRight"/>
    /// <seealso cref="AlphaCut"/>
    /// <seealso cref="UniverseOfDiscourse"/>
    /// <seealso cref="Interval.UpperBound"/>
    Option<double> AlphaCutRightClipped(FuzzyNumber alpha);

    /// <summary>
    /// <para>
    /// Returns the <b>Clipped α-cut</b> of the membership function at the cut value <i>α</i>.
    /// </para>
    /// The clipped α-cut is defined as the intersection of the α-cut with the
    /// <see cref="UniverseOfDiscourse">Universe of Discourse</see>.
    /// </summary>
    /// <param name="alpha">
    /// The cut value <i>α</i>, represented as a <see cref="FuzzyNumber"/>.
    /// </param>
    /// <remarks>
    /// <para>
    /// <b>Special case</b>: If the α-cut lies entirely outside the Universe of Discourse, the clipped α-cut is empty.
    /// </para>
    /// <b>Implicit conversion</b>: The cut value <i>α</i> may be provided as a <see cref="double"/> within the
    /// unit interval [0, 1], which is implicitly convertible to <see cref="FuzzyNumber"/>.
    /// </remarks>
    /// <seealso cref="AlphaCut"/>
    /// <seealso cref="UniverseOfDiscourse"/>
    Option<Interval> AlphaCutClipped(FuzzyNumber alpha);

    #endregion

    #region ImplicationMethods

    /// <summary>
    /// Returns a representation of the membership function obtained by applying the <b>Larsen Product</b> to the original
    /// membership function with the scaling factor <i>λ</i>.
    /// <para>
    /// The Larsen Product scales the membership degree at every point proportionally by the factor <i>λ</i>,
    /// producing a new membership function defined as: <c>μ′(x) = λ · μ(x)</c>.
    /// The resulting function preserves the original shape of the membership function while uniformly reducing its
    /// height.
    /// </para>
    /// <b>Special cases</b>:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// If <c>λ = 0</c> ⇒ the zero function.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// If <c>λ ≥ μMax</c> ⇒ the original membership function, unmodified.
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="lambda">
    /// The scaling factor <i>λ</i>, represented as a <see cref="FuzzyNumber"/>.
    /// </param>
    /// <remarks>
    /// The scaling factor <i>λ</i> may be provided as a <see cref="double"/> within the unit interval [0, 1],
    /// which is implicitly convertible to <see cref="FuzzyNumber"/>.
    /// </remarks>
    /// <returns>
    /// A <see cref="Func{Double, Double}"/> delegate that evaluates the scaled membership degree at any given <i>x</i>
    /// value.
    /// </returns>
    /// <seealso cref="PureFunction"/>
    Func<double, double> LarsenProduct(FuzzyNumber lambda);

    /// <summary>
    /// Returns a representation of the membership function obtained by applying the <b>Clipped Larsen Product</b>
    /// with the scaling factor <i>λ</i>.
    /// <para>
    /// This method evaluates the Larsen Product and restricts the resulting membership degrees to the
    /// <see cref="UniverseOfDiscourse">Universe of Discourse</see>.
    /// </para>
    /// </summary>
    /// <param name="lambda">
    /// The scaling factor <i>λ</i>, represented as a <see cref="FuzzyNumber"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Func{Double, Double}"/> delegate that evaluates the clipped, scaled membership degree at any given
    /// <i>x</i> value.
    /// </returns>
    /// <seealso cref="LarsenProduct"/>
    Func<double, double> LarsenProductClipped(FuzzyNumber lambda);

    /// <summary>
    /// Returns a representation of the membership function obtained by applying the
    /// <b>Mamdani Minimum</b> at the cut value <i>α</i>.
    /// <para>
    /// The Mamdani Minimum is defined as: <c>μ′(x) = min(μ(x), α)</c>.
    /// That is, the membership degree is limited from above by the cut value <i>α</i>, producing a
    /// horizontally truncated version of the original membership function.
    /// </para>
    /// <b>Special cases</b>:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// If <c>α = 0</c> ⇒ the zero function.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// If <c>α ≥ μMax</c> ⇒ the original membership function, unmodified.
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="alpha">
    /// The cut value <i>α</i>, represented as a <see cref="FuzzyNumber"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Func{Double, Double}"/> delegate that evaluates the horizontally
    /// truncated membership degree at a given <i>x</i>.
    /// </returns>
    /// <remarks>
    /// The cut value <i>α</i>> may be provided as a <see cref="double"/> within the unit interval [0, 1],
    /// which is implicitly convertible to <see cref="FuzzyNumber"/>.
    /// </remarks>
    /// <seealso cref="PureFunction"/>
    Func<double, double> MamdaniMinimum(FuzzyNumber alpha);

    /// <summary>
    /// <para>
    /// Returns a representation of the membership function obtained by applying the
    /// <b>Clipped Mamdani Minimum</b> at the cut value <i>α</i>.
    /// </para>
    /// This method evaluates the <see cref="MamdaniMinimum">Mamdani Minimum</see> and restricts the resulting
    /// membership degrees to the <see cref="UniverseOfDiscourse">Universe of Discourse</see>.
    /// </summary>
    /// <param name="alpha">
    /// The cut value <i>α</i>, represented as a <see cref="FuzzyNumber"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Func{Double, Double}"/> delegate that evaluates the clipped, horizontally truncated
    /// membership degree at any given <i>x</i> value.
    /// </returns>
    Func<double, double> MamdaniMinimumClipped(FuzzyNumber alpha);

    #endregion

    #region EvaluationMethods

    /// <summary>
    /// Returns the membership degree <c>μ(x)</c> of the given <i>x</i> value, represented as a
    /// <see cref="FuzzyNumber"/>.
    /// <para>
    /// That is, this method evaluates the membership function at <i>x</i> and returns the corresponding membership
    /// degree <c>μ(x)</c>.
    /// </para>
    /// This is equivalent to invoking the function returned by <see cref="PureFunction"/> with the same <i>x</i> value.
    /// </summary>
    /// <param name="x">
    /// The <i>x</i> value at which the membership degree is evaluated.
    /// </param>
    /// <returns>
    /// The membership degree <c>μ(x)</c>, represented as a <see cref="FuzzyNumber"/>.
    /// </returns>
    /// <seealso cref="PureFunction"/>
    FuzzyNumber MembershipDegree(double x);

    /// <summary>
    /// Returns the clipped membership degree <c>μ(x)</c> of the given <i>x</i> value,
    /// represented as a <see cref="FuzzyNumber"/>.
    /// <para>
    /// The clipped membership degree is obtained by evaluating the <see cref="MembershipDegree">Membership Degree</see>
    /// of <i>x</i> and restricting the result to the <see cref="UniverseOfDiscourse">Universe of Discourse</see>.
    /// </para>
    /// This is equivalent to invoking the function returned by <see cref="PureFunctionClipped"/> with the same
    /// <i>x</i> value.
    /// </summary>
    /// <param name="x">
    /// The <i>x</i> value at which the membership degree is evaluated.
    /// </param>
    /// <returns>
    /// The clipped membership degree <c>μ(x)</c>, represented as a <see cref="FuzzyNumber"/>.
    /// </returns>
    FuzzyNumber MembershipDegreeClipped(double x);

    #endregion

    /// <summary>
    /// <para>
    /// Returns a deep copy of the current membership function.
    /// </para>
    /// The returned instance is structurally independent of the original and preserves all of its properties and behavior.
    /// </summary>
    /// <returns>
    /// A deep copy of the membership function.
    /// </returns>
    IMembershipFunction DeepCopy();

    /// <summary>
    /// <para>
    /// Returns a deep copy of the current membership function with its name replaced by the specified value.
    /// </para>
    /// All other properties and behavior remain identical to the original.
    /// </summary>
    /// <param name="name">
    /// The name to assign to the copied membership function.
    /// </param>
    /// <returns>
    /// A deep copy of the membership function with the specified name.
    /// </returns>
    IMembershipFunction DeepCopy(string name);
}