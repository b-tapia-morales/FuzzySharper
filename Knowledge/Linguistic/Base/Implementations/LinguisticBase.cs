using Kernel.Function.Abstractions;
using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Linguistic.Base.Exceptions;
using Knowledge.Linguistic.Variable.Abstractions;
using Knowledge.Linguistic.Variable.Exceptions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Knowledge.Linguistic.Base.Implementations;

public class LinguisticBase : ILinguisticBase
{
    private Dictionary<string, IVariable> VariableRegistry { get; } = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlySet<string> Variables =>
        VariableRegistry.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);

    public uint VariableCount =>
        (uint) VariableRegistry.Count;

    /// <summary>
    /// Creates a new instance of a <see cref="ILinguisticBase"/>.
    /// </summary>
    /// <returns>A new instance of a <see cref="ILinguisticBase"/></returns>
    public static ILinguisticBase Create() =>
        new LinguisticBase();

    /// <summary>
    /// Creates a new instance of a
    /// <see cref="ILinguisticBase"/> that contains all the linguistic variables provided as parameters.
    /// </summary>
    /// <param name="variables">A varying number of linguistic variables</param>
    /// <returns>The linguistic base itself containing the linguistic variables</returns>
    public static ILinguisticBase Create(params IEnumerable<IVariable> variables) =>
        Create(variables.ToList());

    /// <summary>
    /// Creates a new instance of a
    /// <see cref="ILinguisticBase"/> that contains the collection of linguistic variables
    /// provided as a parameter.
    /// </summary>
    /// <param name="variables">A collection of linguistic variables</param>
    /// <returns>The linguistic base itself containing the collection of linguistic variables</returns>
    public static ILinguisticBase Create(ICollection<IVariable> variables)
    {
        var collidingVariable = variables.GroupBy(e => e.Name, StringComparer.OrdinalIgnoreCase).FirstOrDefault(group => group.Count() > 1);
        if (collidingVariable != null)
            throw new DuplicatedEntryInBatchException(collidingVariable.Key);

        var duplicateVariable = variables.GroupBy(v => v.Name, StringComparer.OrdinalIgnoreCase).FirstOrDefault(g => g.Count() > 1);
        if (duplicateVariable != null)
            throw new DuplicateVariableException(duplicateVariable.Key);

        var @base = new LinguisticBase();
        @base.AddAll(variables);
        return @base;
    }

    public bool ContainsVariable(string name) =>
        VariableRegistry.ContainsKey(name);

    public Option<IVariable> GetVariable(string name) =>
        VariableRegistry.TryGetValue(name, out var variable) ? Option<IVariable>.Some(variable) : Option<IVariable>.None();

    public bool ContainsMapping(string variableName, string termName) =>
        VariableRegistry.TryGetValue(variableName, out var variable) && variable.ContainsMapping(termName);

    public Option<IMembershipFunction> GetMapping(string variableName, string termName) =>
        VariableRegistry.TryGetValue(variableName, out var variable) ? variable.GetMapping(termName) : Option<IMembershipFunction>.None();

    public void Add(IVariable variable)
    {
        if (variable.TermCount == 0)
            throw new UndefinedVariableException(nameof(variable));
        if (!VariableRegistry.TryAdd(variable.Name, variable))
            throw new DuplicateVariableException(variable.Name);
    }

    public void AddAll(params IEnumerable<IVariable> variables) =>
        AddAll(variables.ToList());

    public void AddAll(ICollection<IVariable> variables)
    {
        ArgumentNullException.ThrowIfNull(variables);

        var undefinedVariable = variables.FirstOrDefault(e => e.TermCount == 0);
        if (undefinedVariable != null)
            throw new UndefinedVariableException(nameof(undefinedVariable));

        var collidingVariable = variables.GroupBy(e => e.Name, StringComparer.OrdinalIgnoreCase).FirstOrDefault(group => group.Count() > 1);
        if (collidingVariable != null)
            throw new DuplicatedEntryInBatchException(collidingVariable.Key);

        foreach (var variable in variables)
            VariableRegistry.Add(variable.Name, variable);
    }

    public override string ToString() => $"{string.Join(Environment.NewLine, VariableRegistry.Values)}";
}