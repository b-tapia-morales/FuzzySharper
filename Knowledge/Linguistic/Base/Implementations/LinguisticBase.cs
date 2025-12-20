using Kernel.Function.Abstractions;
using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Linguistic.Base.Exceptions;
using Knowledge.Linguistic.Base.Extensions;
using Knowledge.Linguistic.Variable.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Knowledge.Linguistic.Base.Implementations;

public class LinguisticBase : ILinguisticBase
{
    public IDictionary<string, IVariable> LinguisticVariables { get; } = new Dictionary<string, IVariable>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Creates a new instance of a <see cref="ILinguisticBase"/>.
    /// </summary>
    /// <returns>A new instance of a <see cref="ILinguisticBase"/></returns>
    public static ILinguisticBase Create() => new LinguisticBase();

    /// <summary>
    /// Creates a new instance of a
    /// <see cref="ILinguisticBase"/> that contains all the linguistic variables provided as parameters.
    /// </summary>
    /// <param name="variables">A varying number of linguistic variables</param>
    /// <returns>The linguistic base itself containing the linguistic variables</returns>
    public static ILinguisticBase Create(params IEnumerable<IVariable> variables) => Create(variables.ToList());

    /// <summary>
    /// Creates a new instance of a
    /// <see cref="ILinguisticBase"/> that contains the collection of linguistic variables
    /// provided as a parameter.
    /// </summary>
    /// <param name="variables">A collection of linguistic variables</param>
    /// <returns>The linguistic base itself containing the collection of linguistic variables</returns>
    public static ILinguisticBase Create(ICollection<IVariable> variables)
    {
        var duplicates = variables.GroupBy(v => v.Name, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicates.Count > 0)
            throw new DuplicateVariableException(duplicates);

        var @base = new LinguisticBase();
        @base.AddAll(variables);
        return @base;
    }

    public bool ContainsVariable(string name) =>
        LinguisticVariables.ContainsKey(name);

    public Option<IVariable> GetVariable(string name) =>
        LinguisticVariables.TryGetValue(name, out var variable) ? Option<IVariable>.Some(variable) : Option<IVariable>.None();

    public bool ContainsFunction(string variableName, string termName) =>
        ContainsVariable(variableName) && LinguisticVariables[variableName].ContainsFunction(termName);

    public Option<IMembershipFunction> GetFunction(string variableName, string termName) =>
        LinguisticVariables.TryGetValue(variableName, out var variable) ? variable.GetFunction(termName) : Option<IMembershipFunction>.None();

    public void Add(IVariable variable)
    {
        if (!LinguisticVariables.TryAdd(variable.Name, variable))
            throw new DuplicateVariableException(variable.Name);
    }

    public void AddAll(params IEnumerable<IVariable> variables) => this.AddMultiple(variables);

    public void AddAll(ICollection<IVariable> variables) => this.AddRange(variables);

    public override string ToString() => $"{string.Join(Environment.NewLine, LinguisticVariables.Values)}";
}