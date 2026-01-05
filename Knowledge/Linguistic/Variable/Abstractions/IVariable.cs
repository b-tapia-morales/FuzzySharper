using Kernel.Function.Abstractions;
using Kernel.Function.Comparer.Factory;
using Kernel.Number;
using Shared.Intervals.Implementations;
using Shared.Options.Implementations;

namespace Knowledge.Linguistic.Variable.Abstractions;

/// <summary>
/// <para>
/// Represents a <b>Linguistic Variable</b>, a fundamental concept in fuzzy logic that maps numerical values from a
/// real-valued domain (the <b>Universe of Discourse</b>) to qualitative <b>Linguistic Terms</b> (e.g.: <c>"Cold"</c>,
/// <c>"Warm"</c>, <c>"Hot"</c>), where each term is associated with a <see cref="IMembershipFunction">Membership Function</see>.
/// </para>
/// <b>Typical usages</b>:
/// <list type="number">
/// <item><description>Add terms using <c>Add…Function</c> helpers or <see cref="AddFunction"/>.</description></item>
/// <item><description>Validate coverage using <see cref="GetCoverage"/> and <see cref="FindGaps"/>.</description></item>
/// <item><description>Evaluate the membership degree of a crisp value across all linguistic terms using <see cref="EvaluateAll"/>.</description></item>
/// <item><description>Generate an evenly spaced grid of sample points over the domain with <see cref="SampleDomain"/>,
/// and detect excessive overlap using <see cref="ActiveFunctionCount"/>.</description></item>
/// </list>
/// </summary>
/// <remarks>
/// A linguistic variable is formally defined as a quintuple <i>(X, T(X), U, G, M)</i>:
/// <list type="bullet">
/// <item>
/// <b>X</b>: The <see cref="Name"/> of the variable.
/// </item>
/// <item>
/// <b>T(X)</b>: A finite set of <see cref="Terms">Linguistic Terms</see>.
/// </item>
/// <item>
/// <b>U</b>: The <see cref="UniverseOfDiscourse">Universe of Discourse</see>.
/// </item>
/// <item>
/// <b>M</b>: A finite set of <see cref="IMembershipFunction">Membership Functions</see>.
/// </item>
/// <item>
/// <b>G</b>: The syntactic rules used to modify or combine linguistic terms (e.g., "Very Hot" or "Somewhat Cold"),
/// which can be modeled through the use of
/// <see href="https://github.com/b-tapia-morales/FuzzySharper/blob/master/Reasoning/Proposition/Components/LinguisticHedge.cs">Linguistic Hedges</see> when constructing
/// <see href="https://github.com/b-tapia-morales/FuzzySharper/blob/master/Reasoning/Proposition/Implementations/FuzzyProposition.cs">Fuzzy Propositions</see>.
/// </item>
/// </list>
/// </remarks>
public interface IVariable
{
    /// <summary>
    /// The name of the variable <b>X</b>.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The universe of discourse <b>U</b>.
    /// </summary>
    Interval UniverseOfDiscourse { get; }

    /// <summary>
    /// The set of linguistic terms <b>T(X)</b> currently defined for the variable.
    /// </summary>
    IReadOnlySet<string> Terms { get; }

    /// <summary>
    /// The number of currently defined linguistic terms for the variable.
    /// </summary>
    uint TermCount { get; }

    /// <summary>
    /// Adds a trapezoidal membership function to the linguistic variable.
    /// </summary>
    /// <param name="name">
    /// The linguistic term associated with the function.
    /// </param>
    /// <param name="a">
    /// The left edge of the trapezoid.
    /// </param>
    /// <param name="b">
    /// The top left corner of the trapezoid.
    /// </param>
    /// <param name="c">
    /// The top right corner of the trapezoid.
    /// </param>
    /// <param name="d">
    /// The right edge of the trapezoid.
    /// </param>
    /// <param name="uMax">
    /// The maximum membership degree (plateau height) of the function. Defaults to <c>1</c>.
    /// </param>
    /// <remarks>
    /// The specified coordinates define the trapezoidal shape along the x-axis:
    /// <list type="bullet">
    /// <item>For <c>a &lt; x &lt; b</c>, the membership degree increases linearly from <c>0</c> to <c>μMax</c>.</item>
    /// <item>For <c>b ≤ x ≤ c</c>, the membership degree remains constant at <c>μMax</c>.</item>
    /// <item>For <c>c &lt; x &lt; d</c>, the membership degree decreases linearly from <c>μMax</c> to <c>0</c>.</item>
    /// <item>For <c>x ≤ a</c> or <c>x ≥ d</c>, the membership degree is <c>0</c>.</item>
    /// </list>
    /// The parameters must satisfy <c>a ≤ b ≤ c ≤ d</c>.
    /// </remarks>
    /// <returns>
    /// The current <see cref="IVariable"/> instance with the new linguistic term added.
    /// </returns>
    IVariable AddTrapezoidFunction(string name, double a, double b, double c, double d, double uMax = 1);

    /// <summary>
    /// Adds a left-open trapezoidal membership function to the linguistic variable.
    /// </summary>
    /// <param name="name">
    /// The linguistic term associated with the function.
    /// </param>
    /// <param name="a">
    /// The x-coordinate at which the membership degree begins to decrease from its maximum value to zero.
    /// </param>
    /// <param name="b">
    /// The x-coordinate at which the membership degree reaches zero.
    /// </param>
    /// <param name="uMax">
    /// The maximum membership degree of the function. Defaults to <c>1</c>.
    /// </param>
    /// <remarks>
    /// The specified coordinates define the trapezoidal shape along the x-axis:
    /// <list type="bullet">
    /// <item>For <c>x ≤ a</c>, the membership degree remains constant at <c>μMax</c>.</item>
    /// <item>For <c>a &lt; x &lt; b</c>, the membership degree decreases linearly from <c>μMax</c> to <c>0</c>.</item>
    /// <item>For <c>x ≥ b</c>, the membership degree remains constant at <c>0</c>.</item>
    /// </list>
    /// The plateau for <c>x ∈ (-∞, a]</c> is open-ended on the left side, while the segment over
    /// <c>(a, b)</c> defines a decreasing linear slope.
    /// </remarks>
    /// <returns>
    /// The current <see cref="IVariable"/> instance with the new linguistic term added.
    /// </returns>
    IVariable AddLeftTrapezoidFunction(string name, double a, double b, double uMax = 1);

    /// <summary>
    /// Adds a right-open trapezoidal membership function to the linguistic variable.
    /// </summary>
    /// <param name="name">
    /// The linguistic term associated with the function.
    /// </param>
    /// <param name="a">
    /// The x-coordinate at which the membership degree begins to increase from zero to its maximum value.
    /// </param>
    /// <param name="b">
    /// The x-coordinate at which the membership degree reaches its maximum value.
    /// </param>
    /// <param name="uMax">
    /// The maximum membership degree of the function. Defaults to <c>1</c>.
    /// </param>
    /// <remarks>
    /// The specified coordinates define the trapezoidal shape along the x-axis:
    /// <list type="bullet">
    /// <item>For <c>x ≤ a</c>, the membership degree remains constant at <c>0</c>.</item>
    /// <item>For <c>a &lt; x &lt; b</c>, the membership degree increases linearly from <c>μMax</c> to <c>0</c>.</item>
    /// <item>For <c>x ≥ b</c>, the membership degree remains constant at <c>μMax</c>.</item>
    /// </list>
    /// The plateau for <c>x ∈ [b, +∞)</c> is open-ended on the right side, while the segment over
    /// <c>(a, b)</c> defines an increasing linear slope.
    /// </remarks>
    /// <returns>
    /// The current <see cref="IVariable"/> instance with the new linguistic term added.
    /// </returns>
    IVariable AddRightTrapezoidFunction(string name, double a, double b, double uMax = 1);

    /// <summary>
    /// Adds a triangular membership function to the linguistic variable.
    /// </summary>
    /// <param name="name">
    /// The linguistic term associated with the function.
    /// </param>
    /// <param name="a">
    /// The left edge of the triangle.
    /// </param>
    /// <param name="b">
    /// The peak of the triangle.
    /// </param>
    /// <param name="c">
    /// The right edge of the triangle.
    /// </param>
    /// <param name="uMax">
    /// The maximum membership degree (peak height) of the function. Defaults to <c>1</c>.
    /// </param>
    /// <remarks>
    /// The specified coordinates define the triangular shape along the x-axis:
    /// <list type="bullet">
    /// <item>For <c>a &lt; x &lt; b</c>, the membership degree increases linearly from <c>0</c> to <c>μMax</c>.</item>
    /// <item>For <c>x = b</c>, the membership degree is <c>μMax</c>.</item>
    /// <item>For <c>b &lt; x &lt; c</c>, the membership degree decreases linearly from <c>μMax</c> to <c>0</c>.</item>
    /// </list>
    /// The Parameters must satisfy <c>a ≤ b ≤ c</c>.
    /// </remarks>
    /// <returns>
    /// The current <see cref="IVariable"/> instance with the new linguistic term added.
    /// </returns>
    IVariable AddTriangularFunction(string name, double a, double b, double c, double uMax = 1);

    /// <summary>
    /// Adds a singleton membership function to the linguistic variable.
    /// </summary>
    /// <param name="name">
    /// The linguistic term associated with the function.
    /// </param>
    /// <param name="center">
    /// The point in the universe of discourse where the membership degree reaches its maximum.
    /// </param>
    /// <param name="decimalPlaces">
    /// The number of decimal places used to approximate the singleton as a finite-width shape.
    /// </param>
    /// <param name="uMax">
    /// The maximum membership degree of the function. Defaults to <c>1</c>.
    /// </param>
    /// <remarks>
    /// The singleton function is approximated as a very narrow rectangular shape centered at <c>center</c>,
    /// whose range spans from <c>center − ε</c> to <c>center + ε</c>, where <c>ε = 10^(-decimalPlaces)</c>.
    /// </remarks>
    /// <returns>
    /// The current <see cref="IVariable"/> instance with the new linguistic term added,
    /// allowing fluent chaining.
    /// </returns>
    IVariable AddSingletonFunction(string name, double center, uint decimalPlaces = 4U, double uMax = 1);

    /// <summary>
    /// Adds a Gaussian membership function to the linguistic variable.
    /// </summary>
    /// <param name="name">
    /// The linguistic term associated with the function.
    /// </param>
    /// <param name="mu">
    /// The center <c>μ</c> where the membership degree reaches its maximum.
    /// </param>
    /// <param name="sigma">
    /// The standard deviation <c>σ</c>, which controls the width of the bell curve (must be positive).
    /// </param>
    /// <param name="uMax">
    /// The maximum membership degree of the function. Defaults to <c>1</c>.
    /// </param>
    /// <remarks>
    /// The Gaussian function is symmetric about <c>μ</c>, which defines the center of the bell.
    /// The parameter <c>σ</c> controls the spread of the curve along the x-axis:
    /// smaller values produce a narrower, sharper peak, while larger values produce a wider,
    /// flatter bell. The function approaches <c>0</c> asymptotically on both sides of <c>μ</c>.
    /// </remarks>
    /// <returns>
    /// The current <see cref="IVariable"/> instance with the new linguistic term added.
    /// </returns>
    IVariable AddGaussianFunction(string name, double mu, double sigma, double uMax = 1);

    /// <summary>
    /// Adds a generalized bell-shaped membership function to the linguistic variable.
    /// </summary>
    /// <param name="name">
    /// The linguistic term associated with the function.
    /// </param>
    /// <param name="a">
    /// Scale parameter controlling the half-width of the bell (must be positive).
    /// </param>
    /// <param name="b">
    /// Shape parameter controlling the slope of the bell's sides (must be positive).
    /// </param>
    /// <param name="c">
    /// The center of the bell, where the membership degree reaches its maximum.
    /// </param>
    /// <param name="uMax">
    /// The maximum membership degree of the function. Defaults to <c>1</c>.
    /// </param>
    /// <remarks>
    /// The generalized bell function is symmetric about <c>c</c>, which defines the center of the curve.
    /// The parameter <c>a</c> controls the half-width of the bell along the x-axis, while <c>b</c>
    /// controls the steepness of the slopes. Larger values of <c>b</c> produce steeper sides,
    /// making the bell closer to a rectangular shape, whereas smaller values yield smoother transitions.
    /// </remarks>
    /// <returns>
    /// The current <see cref="IVariable"/> instance with the new linguistic term added.
    /// </returns>
    IVariable AddGeneralizedBellFunction(string name, double a, double b, double c, double uMax = 1);

    /// <summary>
    /// Adds a logistic membership function to the linguistic variable.
    /// </summary>
    /// <param name="name">
    /// The linguistic term associated with the function.
    /// </param>
    /// <param name="a">
    /// The slope parameter controlling the steepness of the curve (must not be zero).
    /// Positive values produce increasing slopes; negative values produce decreasing slopes.
    /// </param>
    /// <param name="c">
    /// The function's midpoint at which <c>μ(x) = μMax / 2</c>.
    /// </param>
    /// <param name="uMax">
    /// The maximum membership degree approached asymptotically by the function. Defaults to <c>1</c>.
    /// </param>
    /// <remarks>
    /// The sigmoid function is monotonic. For <c>a &gt; 0</c>, it approaches <c>0</c> asymptotically as <c>x → −∞</c>
    /// and <c>μMax</c> as <c>x → +∞</c>. For <c>a &lt; 0</c>, this behavior is reversed.
    /// </remarks>
    /// <returns>
    /// The current <see cref="IVariable"/> instance with the new linguistic term added.
    /// </returns>
    IVariable AddLogisticFunction(string name, double a, double c, double uMax = 1);

    /// <summary>
    /// Adds an existing membership function to the linguistic variable,
    /// using the function's <see cref="Kernel.Function.Abstractions.IMembershipFunction.Name">Name</see> as the
    /// linguistic term.
    /// </summary>
    /// <param name="function">
    /// The membership function to add.
    /// </param>
    /// <returns>
    /// The current <see cref="IVariable"/> instance with the new linguistic term added.
    /// </returns>
    IVariable AddFunction(IMembershipFunction function);

    /// <summary>
    /// Adds a semantic mapping between a linguistic term and a membership function
    /// to the linguistic variable.
    /// </summary>
    /// <param name="function">
    /// The membership function whose <see cref="Kernel.Function.Abstractions.IMembershipFunction.Name">Name</see>
    /// defines the associated linguistic term.
    /// </param>
    void AddMapping(IMembershipFunction function);

    /// <summary>
    /// Determines whether a linguistic term is defined for this variable.
    /// </summary>
    /// <param name="term">
    /// The linguistic term to check.
    /// </param>
    /// <returns>
    /// <c>true</c> if the term is mapped to a membership function; otherwise, <c>false</c>.
    /// </returns>
    bool ContainsMapping(string term);

    /// <summary>
    /// Retrieves the membership function associated with a linguistic term.
    /// </summary>
    /// <param name="term">
    /// The linguistic term whose membership function is requested.
    /// </param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the membership function if the term exists;
    /// otherwise, an <i>empty</i> Option.
    /// </returns>
    Option<IMembershipFunction> GetMapping(string term);

    /// <summary>
    /// Computes the coverage interval of the linguistic variable, whose lower and upper bounds are the minimum
    /// <see cref="IMembershipFunction.RestrictedSupportLeft"/> and maximum
    /// <see cref="IMembershipFunction.RestrictedSupportRight"/>, respectively,
    /// across all defined membership functions.
    /// </summary>
    /// <returns>
    /// An <see cref="Option{Interval}"/> containing the coverage interval if at least one membership function is defined;
    /// otherwise, otherwise, an <i>empty</i> Option.
    /// </returns>
    /// <seealso cref="IMembershipFunction.RestrictedSupport"/>
    Option<Interval> GetCoverage();

    /// <summary>
    /// Identifies gaps in the universe of discourse where no membership function participates.
    /// </summary>
    /// <returns>A sequence of intervals where no membership function participates.</returns>
    IEnumerable<Interval> FindGaps();

    /// <summary>
    /// Generates an evenly spaced set of sample points over the meaningful domain of the linguistic variable.
    /// <b>Special cases:</b>
    /// <list type="number">
    /// <item>
    /// If the UoD <see cref="Interval.IsFullyBounded">is fully bounded</see>: Sampling is performed over its interval.
    /// </item>
    /// <item>
    /// If the UoD <see cref="Interval.IsUnbounded">is unbounded</see>: Sampling is performed over the interval spanned
    /// by the currently defined membership functions.
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="points">
    /// The number of points to generate. Defaults to <c>1000</c>.
    /// </param>
    /// <returns>
    /// A sequence of sampled x-values within the universe of discourse.
    /// </returns>
    /// <exception cref="ArgumentException">If <c>points = 0</c>.</exception>
    /// <remarks>
    /// For unbounded domains, sampling spans the effective coverage of the defined membership functions,
    /// consistent with <see cref="GetCoverage"/>.
    /// </remarks>
    IEnumerable<double> SampleDomain(uint points = 1000);

    /// <summary>
    /// Returns the linguistic terms ordered according to the specified criterion.
    /// </summary>
    /// <param name="method">
    /// The ordering strategy used to sort the terms. Defaults to <see cref="OrderingMethod.Centroid"/>.
    /// </param>
    /// <returns>
    /// A sequence of linguistic terms in sorted order.
    /// </returns>
    IEnumerable<string> GetSortedTerms(OrderingMethod method = OrderingMethod.Centroid);

    /// <summary>
    /// Evaluates how many membership functions are active at sampled points in the domain.
    /// </summary>
    /// <param name="points">
    /// The number of sampled points used to evaluate activity. Defaults to <c>1000</c>.
    /// </param>
    /// <param name="maxOverlapAllowed">
    /// The maximum number of simultaneously active functions allowed at a point. Defaults to <c>2</c>.
    /// </param>
    /// <returns>
    /// A sequence of tuples containing a sampled point and the list of active terms exceeding the allowed overlap at
    /// that point.
    /// </returns>
    /// <remarks>
    /// <b>Active function</b>: A function is considered active at a point <c>x</c> if <c>μ(x) &gt; 0</c>.
    /// <b>Sampling behavior</b>: Domain selection and sampling behavior are consistent with <see cref="SampleDomain"/>.
    /// </remarks>
    public IEnumerable<(double x, IList<string> Terms)> ActiveFunctionCount(uint points = 1000,
        uint maxOverlapAllowed = 2);

    /// <summary>
    /// Evaluates the membership degree of a crisp value across all linguistic terms.
    /// </summary>
    /// <param name="crispValue">
    /// The input value to evaluate.
    /// </param>
    /// <returns>
    /// A dictionary mapping each linguistic term to its corresponding membership degree.
    /// </returns>
    IDictionary<string, FuzzyNumber> EvaluateAll(double crispValue);
}