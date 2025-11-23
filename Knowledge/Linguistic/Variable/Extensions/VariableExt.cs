using Kernel.Function.Abstractions;
using Kernel.Function.Implementations;
using Knowledge.Linguistic.Variable.Abstractions;
using Knowledge.Linguistic.Variable.Exceptions;
using Knowledge.Linguistic.Variable.Implementations;
using Shared.Approx;
using Shared.Intervals.Implementations;

namespace Knowledge.Linguistic.Variable.Extensions;

public static class VariableExt
{
    public static IVariable Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new LinguisticVariable(name);
    }

    public static IVariable Create(string name, Interval universe)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new LinguisticVariable(name, universe);
    }

    public static IVariable Create(string name, params IEnumerable<IMembershipFunction> functions) =>
        Create(name, Interval.Default, functions.ToList());

    public static IVariable Create(string name, ICollection<IMembershipFunction> functions) =>
        Create(name, Interval.Default, functions);

    public static IVariable Create(string name, Interval universe, params IEnumerable<IMembershipFunction> functions) =>
        Create(name, universe, functions.ToList());

    public static IVariable Create(string name, Interval universe, ICollection<IMembershipFunction> functions)
    {
        ArgumentNullException.ThrowIfNull(functions);

        if (string.IsNullOrWhiteSpace(name))
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var emptyEntry = functions.FirstOrDefault(func => string.IsNullOrWhiteSpace(func.Name));
        if (emptyEntry != null)
            throw new EmptyEntryException();

        var collidingEntry = functions.GroupBy(func => func.Name.ToLowerInvariant()).FirstOrDefault(group => group.Count() > 1);
        if (collidingEntry != null)
            throw new DuplicatedEntryException(name, collidingEntry.Key);

        var outsideEntry = functions.FirstOrDefault(func => IsOutsideUniverse(func, universe));
        if (outsideEntry != null)
            throw new VariableRangeException(name, universe, outsideEntry.Name, outsideEntry.EffectiveSupport, nameof(outsideEntry));

        var variable = new LinguisticVariable(name, universe);
        foreach (var function in functions)
            variable.SemanticalMappings.Add(function.Name, function);
        return variable;
    }

    extension(IVariable variable)
    {
        public IVariable AddTrapezoidFunction(string name, double a, double b, double c,
            double d, double h = 1) =>
            variable.AddFunction(new TrapezoidFunction(name, a, b, c, d, variable.UniverseOfDiscourse, h));

        public IVariable AddLeftTrapezoidFunction(string name, double a, double b, double h = 1) =>
            variable.AddFunction(new LeftTrapezoidFunction(name, a, b, variable.UniverseOfDiscourse, h));

        public IVariable AddRightTrapezoidFunction(string name, double a, double b,
            double h = 1) =>
            variable.AddFunction(new LeftTrapezoidFunction(name, a, b, variable.UniverseOfDiscourse, h));

        public IVariable AddTriangularFunction(string name, double a, double b, double c,
            double h = 1) =>
            variable.AddFunction(new TriangleFunction(name, a, b, c, variable.UniverseOfDiscourse, h));

        public IVariable AddGaussianFunction(string name, double mu, double sigma, double h = 1) =>
            variable.AddFunction(new GaussianFunction(name, mu, sigma, variable.UniverseOfDiscourse, h));

        public IVariable AddGeneralizedBellFunction(string name, double a, double b, double c,
            double h = 1) =>
            variable.AddFunction(new GeneralizedBellFunction(name, a, b, c, variable.UniverseOfDiscourse, h));

        public IVariable AddSigmoidFunction(string name, double a, double c, double h = 1) =>
            variable.AddFunction(new LogisticFunction(name, a, c, variable.UniverseOfDiscourse, h));

        public IVariable AddFunction(IMembershipFunction function)
        {
            if (string.IsNullOrWhiteSpace(variable.Name))
                throw new EmptyEntryException();

            if (variable.ContainsFunction(function.Name))
                throw new DuplicatedEntryException(variable.Name, function.Name);

            if (IsOutsideUniverse(function, variable.UniverseOfDiscourse))
                throw new VariableRangeException(variable.Name, variable.UniverseOfDiscourse, function.Name, function.EffectiveSupport, nameof(function));

            variable.SemanticalMappings[function.Name] = function;
            return variable;
        }
    }

    private static bool IsOutsideUniverse(IMembershipFunction function, Interval universe)
    {
        if (!function.IsZeroConvergent)
            return false;
        var (lower, upper) = function.EffectiveSupport.ToTuple();
        var (min, max) = universe.ToTuple();
        return lower.IsRoughlyGreaterOrEqualTo(max) || upper.IsRoughlyLesserOrEqualTo(min);
    }
}