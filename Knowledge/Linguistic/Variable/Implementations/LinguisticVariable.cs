using Kernel.Function.Abstractions;
using Kernel.Function.Comparer.Factory;
using Kernel.Number;
using Knowledge.Linguistic.Variable.Abstractions;
using Knowledge.Linguistic.Variable.Extensions;
using Shared.Intervals.Extensions;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Knowledge.Linguistic.Variable.Implementations;

public class LinguisticVariable : IVariable
{
    public string Name { get; }

    public IDictionary<string, IMembershipFunction> SemanticalMappings { get; } =
        new Dictionary<string, IMembershipFunction>(StringComparer.OrdinalIgnoreCase);

    public Interval UniverseOfDiscourse { get; }

    public ISet<string> Terms =>
        SemanticalMappings.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);

    private LinguisticVariable() =>
        throw new InvalidOperationException();

    internal LinguisticVariable(string name) : this(name, Interval.Default)
    {
    }

    internal LinguisticVariable(string name, Interval universe)
    {
        Name = name;
        UniverseOfDiscourse = universe;
    }

    public static IVariable Create(string name) =>
        VariableExt.Create(name);

    public static IVariable Create(string name, Interval universe) =>
        VariableExt.Create(name, universe);

    public static IVariable Create(string name, double lower, double upper) =>
        VariableExt.Create(name, new Interval(lower, upper));

    public static IVariable Create(string name, Interval universe, ICollection<IMembershipFunction> functions) =>
        VariableExt.Create(name, universe, functions);

    public static IVariable Create(string name, double lower, double upper, ICollection<IMembershipFunction> functions) =>
        VariableExt.Create(name, new Interval(lower, upper), functions);

    public static IVariable Create(string name, Interval universe, params IEnumerable<IMembershipFunction> functions) =>
        VariableExt.Create(name, universe, functions.ToList());

    public static IVariable Create(string name, double lower, double upper, params IEnumerable<IMembershipFunction> functions) =>
        VariableExt.Create(name, new Interval(lower, upper), functions.ToList());

    public static IVariable Create(string name, ICollection<IMembershipFunction> functions) =>
        VariableExt.Create(name, Interval.Default, functions);

    public static IVariable Create(string name, params IEnumerable<IMembershipFunction> functions) =>
        VariableExt.Create(name, Interval.Default, functions.ToList());

    public IVariable AddTrapezoidFunction(string name, double a, double b, double c, double d, double uMax = 1) =>
        VariableExt.AddTrapezoidFunction(this, name, a, b, c, d, uMax);

    public IVariable AddLeftTrapezoidFunction(string name, double a, double b, double uMax = 1) =>
        VariableExt.AddLeftTrapezoidFunction(this, name, a, b, uMax);

    public IVariable AddRightTrapezoidFunction(string name, double a, double b, double uMax = 1) =>
        VariableExt.AddRightTrapezoidFunction(this, name, a, b, uMax);

    public IVariable AddTriangularFunction(string name, double a, double b, double c, double uMax = 1) =>
        VariableExt.AddTriangularFunction(this, name, a, b, c, uMax);

    public IVariable AddSingletonFunction(string name, double center, uint decimalPlaces = 4U, double uMax = 1) =>
        VariableExt.AddSingletonFunction(this, name, center, decimalPlaces, uMax);

    public IVariable AddGaussianFunction(string name, double mu, double sigma, double uMax = 1) =>
        VariableExt.AddGaussianFunction(this, name, mu, sigma, uMax);

    public IVariable AddGeneralizedBellFunction(string name, double a, double b, double c, double uMax = 1) =>
        VariableExt.AddGeneralizedBellFunction(this, name, a, b, c, uMax);

    public IVariable AddSigmoidFunction(string name, double a, double c, double uMax = 1) =>
        VariableExt.AddSigmoidFunction(this, name, a, c, uMax);

    public IVariable AddFunction(IMembershipFunction function) =>
        VariableExt.AddFunction(this, function);

    public bool ContainsFunction(string term) =>
        SemanticalMappings.ContainsKey(term);

    public Option<IMembershipFunction> GetFunction(string term) =>
        SemanticalMappings.TryGetValue(term, out var value) ? OptionFactory.SomeRef(value) : OptionFactory.None<IMembershipFunction>();

    public Option<Interval> GetCoverage()
    {
        if (SemanticalMappings.Count == 0)
            return OptionFactory.None<Interval>();
        var ranges = SemanticalMappings.Values.Select(e => e.EffectiveSupport).ToList();
        return new Interval(ranges.MinBy(i => i.LowerBound).LowerBound, ranges.MaxBy(i => i.UpperBound).UpperBound);
    }

    public IEnumerable<Interval> FindGaps() =>
        SemanticalMappings.Count == 0
            ? []
            : SemanticalMappings.Values.Select(e => e.EffectiveSupport).FindGaps().ClipTo(UniverseOfDiscourse);

    public IEnumerable<double> SampleDomain(uint points = 1000)
    {
        if (points == 0)
            throw new ArgumentException("Cannot draw samples from zero points", nameof(points));

        var isUoDBounded = UniverseOfDiscourse.IsFullyBounded;
        var coverageExists = GetCoverage().IsSomeVal(out var interval);
        if (!(isUoDBounded || coverageExists))
            return [];

        var (x0, x1) = isUoDBounded ? UniverseOfDiscourse.ToTuple() : interval.ToTuple();
        switch (points)
        {
            case 1:
                return [x0];
            case 2:
                return [x0, x1];
        }

        var n = (int) points - 1;
        var step = (x1 - x0) / n;
        return Enumerable.Range(0, n).Select(i => x0 + i * step);
    }

    public IEnumerable<string> GetSortedTerms(OrderingMethod method = OrderingMethod.Centroid) =>
        SemanticalMappings.Values
            .OrderBy(func => func, OrderingFactory.GetInstance(method))
            .Select(func => func.Name);

    public IEnumerable<(double x, IList<string> Terms)> ActiveFunctionCount(uint points = 1000, uint maxAllowed = 2)
    {
        var functions = SemanticalMappings.Values.ToList();
        foreach (var point in SampleDomain(points))
        {
            var overlap = functions.Where(func => func.EffectiveSupport.Contains(point)).Select(func => func.Name).ToList();
            if (overlap.Count > maxAllowed)
                yield return (point, overlap);
        }
    }

    public IDictionary<string, FuzzyNumber> EvaluateAll(double crispValue) =>
        SemanticalMappings.ToDictionary(pair => pair.Key, pair => pair.Value.MembershipDegreeClipped(crispValue));

    public override string ToString() => $"""
                                          Linguistic Variable: {Name}
                                          {string.Join(Environment.NewLine, SemanticalMappings.Values)}
                                          """;
}