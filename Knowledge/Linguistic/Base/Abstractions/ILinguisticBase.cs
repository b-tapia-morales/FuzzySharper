using Kernel.Function.Abstractions;
using Knowledge.Linguistic.Variable.Abstractions;
using Shared.Options.Implementations;

namespace Knowledge.Linguistic.Base.Abstractions;

/// <summary>
/// Represents a <b>Linguistic Base</b>, a centralized repository that stores and manages
/// a collection of <see cref="ILinguisticVariable">Linguistic Variables</see> and their associated
/// semantic mappings.
/// </summary>
public interface ILinguisticBase
{
    /// <summary>
    /// Gets the set of names of all currently registered linguistic variables.
    /// </summary>
    IReadOnlySet<string> Variables { get; }
    
    /// <summary>
    /// Gets the total number of registered linguistic variables.
    /// </summary>
    uint VariableCount { get; }
    
    /// <summary>
    /// Determines whether the base contains a linguistic variable with the specified name.
    /// </summary>
    /// <param name="name">
    /// The name of the linguistic variable.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if a variable with the given name exists in the base; otherwise, <see langword="false"/>.
    /// </returns>
    bool ContainsVariable(string name);

    /// <summary>
    /// Retrieves the linguistic variable with the specified name.
    /// </summary>
    /// <param name="name">
    /// The name of the linguistic variable.
    /// </param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the requested variable if it exists; otherwise, an <i>empty</i> Option..
    /// </returns>
    Option<ILinguisticVariable> GetVariable(string name);
    
    /// <summary>
    /// Determines whether the base contains a linguistic variable with the specified name and, if so, whether that
    /// variable contains a semantic mapping for the specified linguistic term.
    /// </summary>
    /// <param name="variableName">
    /// The name of the linguistic variable.
    /// </param>
    /// <param name="termName">
    /// The name of the linguistic term.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the variable exists and defines the specified term; otherwise, <see langword="false"/>.
    /// </returns>
    bool ContainsMapping(string variableName, string termName);

    /// <summary>
    /// Retrieves the membership function associated with the specified linguistic term,
    /// provided that the base contains a linguistic variable with the specified name and
    /// that variable contains a semantic mapping for the specified term.
    /// </summary>
    /// <param name="variableName">
    /// The name of the linguistic variable.
    /// </param>
    /// <param name="termName">
    /// The name of the linguistic term whose membership function is to be retrieved.
    /// </param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the membership function associated with the specified linguistic term
    /// if both conditions are met; otherwise, an <i>empty</i> Option.
    /// </returns>
    Option<IMembershipFunction> GetMapping(string variableName, string termName);

    /// <summary>
    /// Adds a linguistic variable to the base.
    /// </summary>
    /// <param name="variable">
    /// The linguistic variable to add.
    /// </param>
    void Add(ILinguisticVariable variable);

    /// <summary>
    /// Adds a collection of linguistic variables to the base.
    /// </summary>
    /// <param name="variables">
    /// The variables to add.
    /// </param>
    void AddAll(ICollection<ILinguisticVariable> variables);

    /// <summary>
    /// Adds zero or more linguistic variables to the base.
    /// </summary>
    /// <param name="variables">
    /// The variables to add.
    /// </param>
    void AddAll(params IEnumerable<ILinguisticVariable> variables);
}