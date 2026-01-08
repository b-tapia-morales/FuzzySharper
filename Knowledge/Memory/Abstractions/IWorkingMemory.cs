using Knowledge.FactStorage.Abstractions;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;
using Utils.Csv;

namespace Knowledge.Memory.Abstractions;

/// <summary>
/// <para>
/// Represents a <b>Working Memory</b>, containing facts that are either provided externally or inferred through
/// rule evaluation during the reasoning process.
/// A working memory serves as the central repository of factual information used while evaluating fuzzy rules. Facts
/// may be asserted, updated, or removed over time, and may originate from user input, sensor data, or the
/// conclusions of other rules.
/// </para>
/// <para>
/// The working memory acts as a unified fact storage that orchestrates access to conceptually distinct <i>numeric</i>
/// and <i>categorical</i> facts, while exposing both a consolidated interface and dedicated, type-safe accessors
/// for each kind.
/// </para>
/// </summary>
public interface IWorkingMemory : IFactStorage<StringOrType, DoubleOrEnum>
{
    /// <summary>
    /// Provides access to numeric facts stored in the working memory.
    /// </summary>
    public IFactStorage<string, double> NumericStorage { get; }

    /// <summary>
    /// Provides access to categorical facts stored in the working memory.
    /// </summary>
    public IFactStorage<Type, Enum> CategoricalStorage { get; }

    /// <summary>
    /// The default strategy used to resolve conflicts for the insertion of facts with the same declared key.
    /// </summary>
    public EntryResolutionMethod Method { get; }

    /// <summary>
    /// Creates a deep copy of the working memory, including all currently stored facts.
    /// </summary>
    /// <returns>
    /// A new <see cref="IWorkingMemory"/> containing the same facts as the original instance.
    /// </returns>
    new IWorkingMemory DeepCopy();

    /// <summary>
    /// Determines whether a numeric fact with the specified key exists.
    /// <param name="key">The identifying key.</param>
    /// <returns>
    /// <see langword="true"/> if it contains a fact with the specified key; otherwise, <see langword="false"/>.
    /// </returns>
    /// </summary>
    bool ContainsNumericFact(string key);

    /// <summary>
    /// Retrieves the numeric fact associated with the specified key, if present.
    /// </summary>
    /// <param name="key">The key that identifies the fact.</param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the value if the fact exists; otherwise, an empty option.
    /// </returns>
    Option<double> GetNumericFact(string key);

    /// <summary>
    /// Adds a numeric fact to the working memory.
    /// <param name="key">The key that identifies the fact.</param>
    /// <param name="key">The value to associate with the specified key.</param>
    /// </summary>
    void AddNumericFact(string key, double value);

    /// <summary>
    /// Updates the value of a numeric fact identified by the specified key.
    /// </summary>
    /// <param name="key">
    /// The key that identifies the fact.
    /// </param>
    /// <param name="value">
    /// The value to associate with the specified key.
    /// </param>
    /// <remarks>
    /// If a fact with the specified key is already present, its value is replaced; otherwise, a new fact is added
    /// to the working memory.
    /// </remarks>
    void UpdateNumericFact(string key, double value);

    /// <summary>
    /// Removes the numeric fact associated with the specified key.
    /// <param name="key">The fact's key</param>
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the fact was successfully found and removed; otherwise, <see langword="false"/>.
    /// </returns>
    bool RemoveNumericFact(string key);

    /// <summary>
    /// Adds multiple numeric facts to the working memory using the specified conflict resolution strategy.
    /// </summary>
    /// <param name="method">
    /// The strategy used to resolve conflicts for the insertion of facts with the same declared key.
    /// </param>
    /// <param name="pairs">
    /// A collection of key–value pairs representing numeric facts to add.
    /// </param>
    void AddNumericFacts(EntryResolutionMethod method, IEnumerable<KeyValuePair<string, double>> pairs);

    /// <summary>
    /// Adds multiple numeric facts to the working memory.
    /// </summary>
    /// <param name="pairs">
    /// A collection of key–value pairs representing numeric facts to add.
    /// </param>
    /// <remarks>
    /// This method defaults to using the latest value for facts with the same declared key.
    /// </remarks>
    void AddNumericFacts(IEnumerable<KeyValuePair<string, double>> pairs);

    /// <summary>
    /// Adds multiple numeric facts to the working memory using the specified conflict resolution strategy.
    /// </summary>
    /// <param name="method">
    /// The strategy used to resolve conflicts for the insertion of facts with the same declared key.
    /// </param>
    /// <param name="pairs">
    /// A variable number of key–value tuples representing numeric facts to add.
    /// </param>
    void AddNumericFacts(EntryResolutionMethod method, params IEnumerable<(string Key, double Value)> pairs);

    /// <summary>
    /// Adds multiple numeric facts to the working memory using the specified conflict resolution strategy.
    /// </summary>
    /// <param name="pairs">
    /// A variable number of key–value tuples representing numeric facts to add.
    /// </param>
    /// <remarks>
    /// This method defaults to using the latest value for facts with the same declared key.
    /// </remarks>
    void AddNumericFacts(params IEnumerable<(string Key, double Value)> pairs);

    /// <summary>
    /// Reads numeric facts from a file and adds them to the working memory.
    /// </summary>
    /// <param name="folderPath">
    /// The path to the file containing the numeric facts.
    /// </param>
    /// <param name="hasHeader">
    /// Indicates whether the file contains a header row.
    /// </param>
    /// <param name="delimiter">
    /// The delimiter used to separate fields in the file.
    /// </param>
    /// <param name="method">
    /// The strategy used to resolve conflicts for the insertion of facts with the same declared key.
    /// </param>
    void ReadNumericFactsFromFile(string folderPath,
        bool hasHeader = false, DelimiterType delimiter = DelimiterType.Semicolon,
        EntryResolutionMethod method = EntryResolutionMethod.Replace);

    /// <summary>
    /// Determines whether a categorical fact of the specified type is present in the working memory.
    /// </summary>
    /// <typeparam name="T">
    /// The enum type identifying the fact.
    /// </typeparam>
    /// <returns>
    /// <see langword="true"/> if a fact of type <typeparamref name="T"/> exists; otherwise, <see langword="false"/>.
    /// </returns>
    bool ContainsCategoricalFact<T>() where T : struct, Enum, IConvertible;

    /// <summary>
    /// Retrieves the categorical fact associated with the specified enum type.
    /// </summary>
    /// <typeparam name="T">
    /// The enum type identifying the fact.
    /// </typeparam>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the categorical value if present; otherwise, an empty option.
    /// </returns>
    Option<T> GetCategoricalFact<T>() where T : struct, Enum, IConvertible;

    /// <summary>
    /// Adds a categorical fact to the working memory.
    /// </summary>
    /// <typeparam name="T">
    /// The enum type identifying the fact.
    /// </typeparam>
    /// <param name="value">
    /// The categorical value associated with the enum type <typeparamref name="T"/>.
    /// </param>
    void AddCategoricalFact<T>(T value) where T : struct, Enum, IConvertible;

    /// <summary>
    /// Updates the value of a categorical fact.
    /// </summary>
    /// <typeparam name="T">
    /// The enum type identifying the fact.
    /// </typeparam>
    /// <param name="value">
    /// The categorical value associated with the enum type <typeparamref name="T"/>.
    /// </param>
    /// <remarks>
    /// If a fact of the specified type is already present, its value is replaced; otherwise, a new fact is added
    /// to the working memory.
    /// </remarks>
    void UpdateCategoricalFact<T>(T value) where T : struct, Enum, IConvertible;

    /// <summary>
    /// Removes the categorical fact associated with the specified enum type.
    /// </summary>
    /// <typeparam name="T">
    /// The enum type identifying the fact.
    /// </typeparam>
    /// <returns>
    /// <see langword="true"/> if the fact was successfully found and removed; otherwise, <see langword="false"/>.
    /// </returns>
    bool RemoveCategoricalFact<T>() where T : struct, Enum, IConvertible;

    /// <summary>
    /// Adds multiple categorical facts to the working memory using the specified conflict resolution strategy.
    /// </summary>
    /// <param name="method">
    /// The strategy used to resolve conflicts for the insertion of facts with the same declared key.
    /// </param>
    /// <param name="values">
    /// A variable number of categorical values to add. Each value’s enum type acts as its key.
    /// </param>
    void AddCategoricalFacts(EntryResolutionMethod method, params IEnumerable<Enum> values);

    /// <summary>
    /// Adds multiple categorical facts to the working memory.
    /// </summary>
    /// <param name="values">
    /// A variable number of categorical values to add. Each value’s enum type acts as its key.
    /// </param>
    /// <remarks>
    /// This method defaults to using the latest value for facts with the same declared key.
    /// </remarks>
    void AddCategoricalFacts(params IEnumerable<Enum> values);

    /// <summary>
    /// Reads categorical facts from a file and adds them to the working memory.
    /// </summary>
    /// <param name="folderPath">
    /// The path to the file containing the categorical facts.
    /// </param>
    /// <param name="useFullyQualifiedName">
    /// Indicates whether enum types are identified using their fully qualified names.
    /// </param>
    /// <param name="hasHeader">
    /// Indicates whether the file contains a header row.
    /// </param>
    /// <param name="delimiter">
    /// The delimiter used to separate fields in the file.
    /// </param>
    /// <param name="method">
    /// The strategy used to resolve conflicts for the insertion of facts with the same declared key.
    /// </param>
    void ReadCategoricalFactsFromFile(string folderPath, bool useFullyQualifiedName = false, bool hasHeader = false,
        DelimiterType delimiter = DelimiterType.Semicolon,
        EntryResolutionMethod method = EntryResolutionMethod.Replace);

    IFactStorage<StringOrType, DoubleOrEnum> IFactStorage<StringOrType, DoubleOrEnum>.DeepCopy() =>
        DeepCopy();
}