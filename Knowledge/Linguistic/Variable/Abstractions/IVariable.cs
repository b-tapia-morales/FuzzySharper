using Kernel.Function.Abstractions;
using Kernel.Function.Comparer.Factory;
using Kernel.Number;
using Shared.Intervals.Implementations;
using Shared.Options.Implementations;

namespace Knowledge.Linguistic.Variable.Abstractions;

/// <summary>
/// Represents a <b>Linguistic Variable</b>, a fundamental concept in fuzzy logic that maps numerical values
/// (the <b>Universe of Discourse</b>) to qualitative linguistic terms.
/// </summary>
/// <remarks>
/// A linguistic variable is formally defined as a quintuple <i>(X, T(X), U, G, M)</i>:
/// <list type="bullet">
/// <item>
/// <b>X</b>: The <i>Name</i> of the variable, represented by the <see cref="Name"/> property (e.g., "Temperature").
/// </item>
/// <item>
/// <b>U</b>: The <i>Universe of Discourse</i>, represented by the interval defined by <see cref="LowerBound"/>
/// and <see cref="UpperBound"/>, which specifies the range of possible numerical values for the variable
/// (e.g.: [0, 100] for Temperature in Celsius).
/// </item>
/// <item>
/// <b>T(X)</b>: A finite set of <i>Linguistic Terms</i>, represented by the keys in <see cref="SemanticalMappings"/>
/// (e.g.: "Cold", "Warm", "Hot").
/// </item>
/// <item>
/// <b>M</b>: A finite set of <i>Membership Functions</i>,
/// represented by the values in <see cref="SemanticalMappings"/>.
/// Each membership function is associated with its corresponding term in <i>T(X)</i>.
/// </item>
/// <item>
/// <b>G</b>: The syntactic rules that allow for combining linguistic terms (e.g., "Very Hot" or "Somewhat Cold").
/// While <b>G</b> is not explicitly represented here, it can be modeled through the use of
/// <see cref="Proposition.Enums.LinguisticHedge">Linguistic Hedges</see> in conjunction with <see cref="IMembershipFunction">Membership Functions</see>.
/// </item>
/// </list>
/// <para>
/// This interface encapsulates the structure and semantics of linguistic variables,
/// supporting their use in fuzzy logic systems.
/// </para>
/// </remarks>
/// <seealso cref="Proposition.Enums.LinguisticHedge"/>
/// <seealso cref="Proposition.IProposition"/>
public interface IVariable
{
    /// <summary>
    /// The name of the variable <b>X</b>.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The semantical associations between each Linguistic Term (t ∈ <b>T(X)</b>),
    /// and its corresponding Membership Function (m ∈ <b>M</b>),
    /// represented as a Dictionary.
    /// </summary>
    IDictionary<string, IMembershipFunction> SemanticalMappings { get; }

    Interval UniverseOfDiscourse { get; }

    ISet<string> Terms { get; }

    IVariable AddTrapezoidFunction(string name, double a, double b, double c, double d, double uMax = 1);

    IVariable AddLeftTrapezoidFunction(string name, double a, double b, double uMax = 1);

    IVariable AddRightTrapezoidFunction(string name, double a, double b, double uMax = 1);

    IVariable AddTriangularFunction(string name, double a, double b, double c, double uMax = 1);

    IVariable AddSingletonFunction(string name, double center, uint decimalPlaces = 4U, double uMax = 1);

    IVariable AddGaussianFunction(string name, double mu, double sigma, double uMax = 1);

    IVariable AddGeneralizedBellFunction(string name, double a, double b, double c, double uMax = 1);

    IVariable AddSigmoidFunction(string name, double a, double c, double uMax = 1);

    IVariable AddFunction(IMembershipFunction function);

    bool ContainsFunction(string term);

    Option<IMembershipFunction> GetFunction(string term);

    Option<Interval> GetCoverage();

    IEnumerable<Interval> FindGaps();

    IEnumerable<double> SampleDomain(uint points = 1000);
    
    IEnumerable<string> GetSortedTerms(OrderingMethod method = OrderingMethod.Centroid);

    public IEnumerable<(double x, IList<string> Terms)> ActiveFunctionCount(uint points = 1000, uint maxAllowed = 2);

    IDictionary<string, FuzzyNumber> EvaluateAll(double crispValue);
}