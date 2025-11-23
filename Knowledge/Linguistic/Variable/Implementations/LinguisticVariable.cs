using Kernel.Function.Abstractions;
using Knowledge.Linguistic.Variable.Abstractions;
using Knowledge.Linguistic.Variable.Extensions;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Knowledge.Linguistic.Variable.Implementations;

public class LinguisticVariable : IVariable
{
    public string Name { get; }
    public Interval UniverseOfDiscourse { get; }

    public IDictionary<string, IMembershipFunction> SemanticalMappings { get; } =
        new Dictionary<string, IMembershipFunction>(StringComparer.OrdinalIgnoreCase);

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

    public IVariable AddTrapezoidFunction(string name, double a, double b, double c, double d, double h = 1) =>
        VariableExt.AddTrapezoidFunction(this, name, a, b, c, d, h);

    public IVariable AddLeftTrapezoidFunction(string name, double a, double b, double h = 1) =>
        VariableExt.AddLeftTrapezoidFunction(this, name, a, b, h);

    public IVariable AddRightTrapezoidFunction(string name, double a, double b, double h = 1) =>
        VariableExt.AddRightTrapezoidFunction(this, name, a, b, h);

    public IVariable AddTriangularFunction(string name, double a, double b, double c, double h = 1) =>
        VariableExt.AddTriangularFunction(this, name, a, b, c, h);

    public IVariable AddGaussianFunction(string name, double m, double o, double h = 1) =>
        VariableExt.AddGaussianFunction(this, name, m, o, h);

    public IVariable AddCauchyFunction(string name, double a, double b, double c, double h = 1) =>
        this.AddGeneralizedBellFunction(name, a, b, c, h);

    public IVariable AddSigmoidFunction(string name, double a, double c, double h = 1) =>
        VariableExt.AddSigmoidFunction(this, name, a, c, h);

    public IVariable AddFunction(IMembershipFunction function) =>
        VariableExt.AddFunction(this, function);

    public bool ContainsFunction(string term) =>
        SemanticalMappings.ContainsKey(term);

    public Option<IMembershipFunction> GetFunction(string term) =>
        SemanticalMappings.TryGetValue(term, out var value) ? OptionFactory.SomeRef(value) : OptionFactory.None<IMembershipFunction>();

    public override string ToString() => $"""
                                          Linguistic Variable: {Name}
                                          {string.Join(Environment.NewLine, SemanticalMappings.Values)}
                                          """;
}