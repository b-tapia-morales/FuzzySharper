using System.Diagnostics;
using Kernel.Function.Abstractions;
using Kernel.Function.Comparer.Factory;
using Kernel.Function.Implementations;
using Kernel.Number;
using Knowledge.Linguistic.Variable.Abstractions;
using Knowledge.Linguistic.Variable.Exceptions;
using Shared.Approx;
using Shared.Intervals.Extensions;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Knowledge.Linguistic.Variable.Implementations;

public sealed class LinguisticVariable : ILinguisticVariable
{
    private Dictionary<string, IMembershipFunction> SemanticalMappings { get; } = new(StringComparer.OrdinalIgnoreCase);

    public string Name { get; }

    public Interval UniverseOfDiscourse { get; }

    public IReadOnlySet<string> Terms =>
        SemanticalMappings.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);

    public uint TermCount =>
        (uint)SemanticalMappings.Count;

    private LinguisticVariable() =>
        throw new InvalidOperationException();

    private LinguisticVariable(string name) : this(name, Interval.Default)
    {
    }

    private LinguisticVariable(string name, Interval universe)
    {
        Name = name;
        UniverseOfDiscourse = universe;
    }

    public static ILinguisticVariable Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new LinguisticVariable(name);
    }

    public static ILinguisticVariable Create(string name, Interval universe)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new LinguisticVariable(name, universe);
    }

    public static ILinguisticVariable Create(string name, params IEnumerable<IMembershipFunction> functions) =>
        Create(name, Interval.Default, functions.ToList());

    public static ILinguisticVariable Create(string name, ICollection<IMembershipFunction> functions) =>
        Create(name, Interval.Default, functions);

    public static ILinguisticVariable Create(string name, Interval universe, params IEnumerable<IMembershipFunction> functions) =>
        Create(name, universe, functions.ToList());

    public static ILinguisticVariable Create(string name, Interval universe, ICollection<IMembershipFunction> functions)
    {
        ArgumentNullException.ThrowIfNull(functions);

        if (string.IsNullOrWhiteSpace(name))
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var emptyEntry = functions.FirstOrDefault(func => string.IsNullOrWhiteSpace(func.Name));
        if (emptyEntry != null)
            throw new EmptyEntryException();

        var collidingEntry = functions.GroupBy(func => func.Name, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1);
        if (collidingEntry != null)
            throw new DuplicatedEntryException(name, collidingEntry.Key);

        var outsideEntry = functions.FirstOrDefault(func => IsOutsideUniverse(func, universe));
        if (outsideEntry != null)
            throw new VariableRangeException(name, universe, outsideEntry.Name, outsideEntry.RestrictedSupport,
                nameof(outsideEntry));

        var variable = new LinguisticVariable(name, universe);
        foreach (var function in functions)
            variable.AddMapping(function);
        return variable;
    }

    public ILinguisticVariable AddTrapezoidFunction(string name, double a, double b, double c, double d,
        double uMax = 1) =>
        AddFunction(TrapezoidFunction.Create(name, a, b, c, d, UniverseOfDiscourse, uMax));

    public ILinguisticVariable AddLeftTrapezoidFunction(string name, double a, double b,
        double uMax = 1) =>
        AddFunction(LeftTrapezoidFunction.Create(name, a, b, UniverseOfDiscourse, uMax));

    public ILinguisticVariable AddRightTrapezoidFunction(string name, double a, double b,
        double uMax = 1) =>
        AddFunction(LeftTrapezoidFunction.Create(name, a, b, UniverseOfDiscourse, uMax));

    public ILinguisticVariable AddTriangularFunction(string name, double a, double b, double c,
        double uMax = 1) =>
        AddFunction(TriangleFunction.Create(name, a, b, c, UniverseOfDiscourse, uMax));

    public ILinguisticVariable AddSingletonFunction(string name, double center, uint decimalPlaces = 4U,
        double uMax = 1) =>
        AddFunction(SingletonFunction.Create(name, center, UniverseOfDiscourse, decimalPlaces, uMax));

    public ILinguisticVariable AddGaussianFunction(string name, double mu, double sigma,
        double uMax = 1) =>
        AddFunction(GaussianFunction.Create(name, mu, sigma, UniverseOfDiscourse, uMax));

    public ILinguisticVariable AddGeneralizedBellFunction(string name, double a, double b, double c,
        double uMax = 1) =>
        AddFunction(GeneralizedBellFunction.Create(name, a, b, c, UniverseOfDiscourse, uMax));

    public ILinguisticVariable AddLogisticFunction(string name, double a, double c, double uMax = 1) =>
        AddFunction(LogisticFunction.Create(name, a, c, UniverseOfDiscourse, uMax));

    public ILinguisticVariable AddFunction(IMembershipFunction function)
    {
        AddMapping(function);
        return this;
    }

    public void AddMapping(IMembershipFunction function)
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(function.Name));

        if (ContainsMapping(function.Name))
            throw new DuplicatedEntryException(Name, function.Name);
        if (IsOutsideUniverse(function, UniverseOfDiscourse))
            throw new VariableRangeException(Name, UniverseOfDiscourse, function.Name, function.RestrictedSupport,
                nameof(function));

        SemanticalMappings.Add(function.Name, function);
    }

    public bool ContainsMapping(string term) =>
        SemanticalMappings.ContainsKey(term);

    public Option<IMembershipFunction> GetMapping(string term) =>
        SemanticalMappings.TryGetValue(term, out var value)
            ? Option<IMembershipFunction>.Some(value)
            : Option<IMembershipFunction>.None();

    public Option<Interval> GetCoverage()
    {
        if (SemanticalMappings.Count == 0)
            return Option<Interval>.None();
        var ranges = SemanticalMappings.Values.Select(e => e.EffectiveSupport).ToList();
        return new Interval(ranges.MinBy(i => i.LowerBound).LowerBound, ranges.MaxBy(i => i.UpperBound).UpperBound);
    }

    public IEnumerable<Interval> FindGaps() =>
        SemanticalMappings.Count == 0
            ? []
            : SemanticalMappings.Values.Select(e => e.EffectiveSupport).FindGaps().ClipTo(UniverseOfDiscourse);

    public IEnumerable<double> SampleDomain(uint points = 1000)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(points, 1U);
        var isUoDBounded = UniverseOfDiscourse.IsFullyBounded;
        var coverageExists = GetCoverage().IsSome(out var interval);
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

        var n = checked((int)points - 1);
        var step = (x1 - x0) / n;
        return Enumerable.Range(0, n).Select(i => x0 + i * step);
    }

    public IEnumerable<string> GetSortedTerms(OrderingMethod method = OrderingMethod.Centroid) =>
        SemanticalMappings.Values
            .OrderBy(func => func, OrderingFactory.GetInstance(method))
            .Select(func => func.Name);

    public IEnumerable<(double x, IList<string> Terms)> ActiveFunctionCount(uint points = 1000,
        uint maxOverlapAllowed = 2)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxOverlapAllowed, 1U);
        var functions = SemanticalMappings.Values.ToList();
        foreach (var point in SampleDomain(points))
        {
            var overlap = functions.Where(func => func.EffectiveSupport.Contains(point)).Select(func => func.Name)
                .ToList();
            if (overlap.Count > maxOverlapAllowed)
                yield return (point, overlap);
        }
    }

    public IDictionary<string, FuzzyNumber> EvaluateAll(double crispValue) =>
        SemanticalMappings.ToDictionary(pair => pair.Key, pair => pair.Value.MembershipDegreeClipped(crispValue));

    public override string ToString() => $"""
                                          Linguistic Variable: {Name}
                                          {string.Join(Environment.NewLine, SemanticalMappings.Values)}
                                          """;

    private static bool IsOutsideUniverse(IMembershipFunction function, Interval universe)
    {
        if (!function.IsZeroConvergent)
            return false;
        var (lower, upper) = function.RestrictedSupport.ToTuple();
        var (min, max) = universe.ToTuple();
        return lower.IsRoughlyGreaterOrEqualTo(max) || upper.IsRoughlyLesserOrEqualTo(min);
    }
}