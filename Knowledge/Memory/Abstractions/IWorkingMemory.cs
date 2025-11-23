using Knowledge.FactStorage.Abstractions;
using Shared.Options.Implementations;
using Utils.Csv;

namespace Knowledge.Memory.Abstractions;

/// <summary>
/// Contains the facts that are provided by the user or inferred from other rules.
/// </summary>
public interface IWorkingMemory : IFactStorage
{
    public IFactStorage NumericStorage { get; }
    public IFactStorage CategoricalStorage { get; }

    /// <summary>
    /// The method that resolves conflicting entries with the same declared key.
    /// </summary>
    public EntryResolutionMethod Method { get; }

    new IWorkingMemory DeepCopy();

    bool ContainsNumericFact(string key);

    Option<double> GetNumericFact(string key);

    void AddNumericFact(string key, double value);

    void UpdateNumericFact(string key, double value);

    bool RemoveNumericFact(string key);

    void AddNumericFacts(EntryResolutionMethod method, IEnumerable<KeyValuePair<string, double>> dictionary);

    void AddNumericFacts(IEnumerable<KeyValuePair<string, double>> dictionary);

    void AddNumericFacts(EntryResolutionMethod method, params IEnumerable<(string Key, double Value)> pairs);

    void AddNumericFacts(params IEnumerable<(string Key, double Value)> pairs);

    void ReadNumericFactsFromFile(string folderPath,
        bool hasHeader = false, DelimiterType delimiter = DelimiterType.Semicolon, EntryResolutionMethod method = EntryResolutionMethod.Replace);

    bool ContainsCategoricalFact<T>() where T : struct, Enum, IConvertible;

    Option<T> GetCategoricalFact<T>() where T : struct, Enum, IConvertible;

    void AddCategoricalFact<T>(T value) where T : struct, Enum, IConvertible;

    void UpdateCategoricalFact<T>(T value) where T : struct, Enum, IConvertible;

    bool RemoveCategoricalFact<T>() where T : struct, Enum, IConvertible;

    void AddCategoricalFacts(EntryResolutionMethod method, params IEnumerable<Enum> values);

    void AddCategoricalFacts(params IEnumerable<Enum> values);

    void ReadCategoricalFactsFromFile(string folderPath, bool useFullyQualifiedName = false, bool hasHeader = false,
        DelimiterType delimiter = DelimiterType.Semicolon, EntryResolutionMethod method = EntryResolutionMethod.Replace);
}