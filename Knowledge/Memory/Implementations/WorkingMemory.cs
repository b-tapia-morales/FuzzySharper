// ReSharper disable RedundantExplicitTupleComponentName

using Knowledge.Csv.Categorical;
using Knowledge.Csv.Numeric;
using Knowledge.FactStorage.Abstractions;
using Knowledge.FactStorage.Implementations;
using Knowledge.Memory.Abstractions;
using Knowledge.Memory.Exceptions;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;
using Utils.Csv;
using Utils.Types;

namespace Knowledge.Memory.Implementations;

public class WorkingMemory : IWorkingMemory
{
    public IFactStorage CategoricalStorage { get; } = new CategoricalStorage();
    public IFactStorage NumericStorage { get; } = new NumericStorage();
    public EntryResolutionMethod Method { get; }

    private WorkingMemory(EntryResolutionMethod method = EntryResolutionMethod.Replace) =>
        Method = method;

    private WorkingMemory(IEnumerable<(string Key, double Value)> numericalFacts, EntryResolutionMethod method = EntryResolutionMethod.Replace)
    {
        NumericStorage.AddRange(
            numericalFacts.Select(e => ((StringOrType) e.Key, (DoubleOrEnum) e.Value)),
            method == EntryResolutionMethod.Replace
        );
        Method = method;
    }

    private WorkingMemory(IEnumerable<KeyValuePair<string, double>> numericalFacts, EntryResolutionMethod method = EntryResolutionMethod.Replace)
    {
        NumericStorage.AddRange(
            numericalFacts.Select(e => new KeyValuePair<StringOrType, DoubleOrEnum>((StringOrType) e.Key, (DoubleOrEnum) e.Value)),
            method == EntryResolutionMethod.Replace);
        Method = method;
    }

    private WorkingMemory(IEnumerable<Enum> categoricalFacts, EntryResolutionMethod method = EntryResolutionMethod.Replace)
    {
        CategoricalStorage.AddRange(
            categoricalFacts.Select(e => ((StringOrType) e.GetType(), (DoubleOrEnum) e)),
            method == EntryResolutionMethod.Replace
        );
        Method = method;
    }

    private WorkingMemory(IEnumerable<(string Key, double Value)> numericalFacts, IEnumerable<Enum> categoricalFacts,
        EntryResolutionMethod method = EntryResolutionMethod.Replace)
    {
        NumericStorage.AddRange(
            numericalFacts.Select(e => ((StringOrType) e.Key, (DoubleOrEnum) e.Value)),
            method == EntryResolutionMethod.Replace
        );
        CategoricalStorage.AddRange(
            categoricalFacts.Select(e => ((StringOrType) e.GetType(), (DoubleOrEnum) e)),
            method == EntryResolutionMethod.Replace
        );
        Method = method;
    }

    private WorkingMemory(IEnumerable<KeyValuePair<string, double>> numericalFacts, IEnumerable<Enum> categoricalFacts,
        EntryResolutionMethod method = EntryResolutionMethod.Replace)
    {
        NumericStorage.AddRange(
            numericalFacts.Select(e => new KeyValuePair<StringOrType, DoubleOrEnum>((StringOrType) e.Key, (DoubleOrEnum) e.Value)),
            method == EntryResolutionMethod.Replace
        );
        CategoricalStorage.AddRange(
            categoricalFacts.Select(e => ((StringOrType) e.GetType(), (DoubleOrEnum) e)),
            method == EntryResolutionMethod.Replace
        );
        Method = method;
    }

    private WorkingMemory(IFactStorage categoricalStorage, IFactStorage numericalStorage, EntryResolutionMethod method)
    {
        CategoricalStorage = categoricalStorage;
        NumericStorage = numericalStorage;
        Method = method;
    }

    public static IWorkingMemory Create(EntryResolutionMethod method = EntryResolutionMethod.Replace) =>
        new WorkingMemory(method);

    public static IWorkingMemory Create(EntryResolutionMethod method, params IEnumerable<(string Key, double Value)> numericalFacts) =>
        new WorkingMemory(numericalFacts, method);

    public static IWorkingMemory Create(params IEnumerable<(string Key, double Value)> numericalFacts) =>
        new WorkingMemory(numericalFacts);

    public static IWorkingMemory Create(EntryResolutionMethod method, IEnumerable<KeyValuePair<string, double>> numericalFacts) =>
        new WorkingMemory(numericalFacts, method);

    public static IWorkingMemory Create(IEnumerable<KeyValuePair<string, double>> numericalFacts) =>
        new WorkingMemory(numericalFacts);

    public static IWorkingMemory Create(EntryResolutionMethod method, params IEnumerable<Enum> categoricalFacts) =>
        new WorkingMemory(categoricalFacts, method);

    public static IWorkingMemory Create(params IEnumerable<Enum> categoricalFacts) =>
        new WorkingMemory(categoricalFacts);

    public static IWorkingMemory Create(EntryResolutionMethod method, IEnumerable<(string Key, double Value)> numericalFacts, IEnumerable<Enum> categoricalFacts) =>
        new WorkingMemory(numericalFacts, categoricalFacts, method);

    public static IWorkingMemory Create(EntryResolutionMethod method, IEnumerable<KeyValuePair<string, double>> numericalFacts, IEnumerable<Enum> categoricalFacts) =>
        new WorkingMemory(numericalFacts, categoricalFacts, method);

    public bool ContainsNumericFact(string key) =>
        NumericStorage.Contains(key);

    public Option<double> GetNumericFact(string key) =>
        NumericStorage.GetValue(key).IsSome(out var value) ? Option<double>.Some((double) value) : Option<double>.None();

    public void AddNumericFact(string key, double value)
    {
        if (Method == EntryResolutionMethod.Preserve)
        {
            NumericStorage.TryAddValue(key, value);
            return;
        }

        NumericStorage.AddValue(key, value);
    }

    public void UpdateNumericFact(string key, double value) =>
        NumericStorage.AddValue(key, value);

    public bool RemoveNumericFact(string key) =>
        NumericStorage.Remove(key);

    public void AddNumericFacts(EntryResolutionMethod method, IEnumerable<KeyValuePair<string, double>> dictionary) =>
        NumericStorage.AddRange(dictionary.ToDictionary(pair => (StringOrType) pair.Key, pair => (DoubleOrEnum) pair.Value), method == EntryResolutionMethod.Replace);

    public void AddNumericFacts(IEnumerable<KeyValuePair<string, double>> dictionary) =>
        AddNumericFacts(EntryResolutionMethod.Replace, dictionary);

    public void AddNumericFacts(EntryResolutionMethod method, params IEnumerable<(string Key, double Value)> pairs) =>
        NumericStorage.AddRange(pairs
            .GroupBy(tuple => tuple.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(tuple => (StringOrType) tuple.Key, tuple => (DoubleOrEnum) (method == EntryResolutionMethod.Replace ? tuple.Last().Value : tuple.First().Value))
        );

    public void AddNumericFacts(params IEnumerable<(string Key, double Value)> pairs) =>
        AddNumericFacts(EntryResolutionMethod.Replace, pairs);

    public void ReadNumericFactsFromFile(string folderPath, bool hasHeader = false,
        DelimiterType delimiter = DelimiterType.Semicolon, EntryResolutionMethod method = EntryResolutionMethod.Replace)
    {
        var entries = CsvUtils.RetrieveRows<NumericFact, NumericMapping>(folderPath, hasHeader, delimiter)
            .GroupBy(tuple => tuple.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(tuple => tuple.Key, tuple => method == EntryResolutionMethod.Replace ? tuple.Last().Value : tuple.First().Value);
        AddNumericFacts(method, entries);
    }

    public bool ContainsCategoricalFact<T>() where T : struct, Enum, IConvertible =>
        CategoricalStorage.Contains(typeof(T));

    public Option<T> GetCategoricalFact<T>() where T : struct, Enum, IConvertible =>
        CategoricalStorage.GetValue(typeof(T)).IsSome(out var value) ? Option<T>.Some((T) value) : Option<T>.None();

    public void AddCategoricalFact<T>(T value) where T : struct, Enum, IConvertible
    {
        if (Method == EntryResolutionMethod.Preserve)
        {
            CategoricalStorage.TryAddValue(value.GetType(), value);
            return;
        }

        CategoricalStorage.AddValue(value.GetType(), value);
    }

    public void UpdateCategoricalFact<T>(T value) where T : struct, Enum, IConvertible =>
        CategoricalStorage.AddValue(value.GetType(), value);

    public bool RemoveCategoricalFact<T>() where T : struct, Enum, IConvertible =>
        CategoricalStorage.Remove(typeof(T));

    public void AddCategoricalFacts(EntryResolutionMethod method, params IEnumerable<Enum> values) =>
        CategoricalStorage.AddRange(values
            .GroupBy(value => value.GetType())
            .ToDictionary(tuple => (StringOrType) tuple.Key, tuple => (DoubleOrEnum) (method == EntryResolutionMethod.Replace ? tuple.Last() : tuple.First()))
        );

    public void AddCategoricalFacts(params IEnumerable<Enum> values) =>
        AddCategoricalFacts(EntryResolutionMethod.Replace, values);

    public void ReadCategoricalFactsFromFile(string folderPath, bool useFullyQualifiedName = false, bool hasHeader = false,
        DelimiterType delimiter = DelimiterType.Semicolon, EntryResolutionMethod method = EntryResolutionMethod.Replace)
    {
        var entries = CsvUtils.RetrieveRows<CategoricalFact, CategoricalMapping>(folderPath, hasHeader, delimiter)
            .Select(e => (Type: useFullyQualifiedName ? TypeExt.GetExactType(e.TypeName) : TypeExt.GetTypeByName(e.TypeName), ConstValue: e.ConstValue))
            .Where(tuple => tuple.Type.IsSome(out var type) && type.IsEnum)
            .Select(tuple => ParseEnum(tuple.Type.Get, tuple.ConstValue))
            .GroupBy(e => e.GetType())
            .Select(e => method == EntryResolutionMethod.Replace ? e.Last() : e.First());
        AddCategoricalFacts(method, entries);
    }

    public IWorkingMemory DeepCopy() =>
        new WorkingMemory(CategoricalStorage.DeepCopy(), NumericStorage.DeepCopy(), Method);

    public ISet<StringOrType> Keys() =>
        new HashSet<StringOrType>(NumericStorage.Keys().Union(CategoricalStorage.Keys()));

    public bool Contains(StringOrType key) =>
        key.IsString ? NumericStorage.Contains(key) : CategoricalStorage.Contains(key);

    public Option<DoubleOrEnum> GetValue(StringOrType key) =>
        key.IsString ? NumericStorage.GetValue(key) : CategoricalStorage.GetValue(key);

    public void AddValue(StringOrType key, DoubleOrEnum value)
    {
        switch (key.IsString)
        {
            case true when value.IsDouble:
                NumericStorage.AddValue(key, value);
                break;
            case false when value.IsEnum:
                CategoricalStorage.AddValue(key, value);
                break;
            default:
                throw new InvalidPairException(key.GetType().Name, value.GetType().Name);
        }
    }

    public bool TryAddValue(StringOrType key, DoubleOrEnum value) =>
        key.IsString switch
        {
            true when value.IsDouble => NumericStorage.TryAddValue(key, value),
            false when value.IsEnum => CategoricalStorage.TryAddValue(key, value),
            _ => throw new InvalidPairException(key.GetType().Name, value.GetType().Name)
        };

    public bool Remove(StringOrType key) =>
        key.IsString ? NumericStorage.Remove(key) : CategoricalStorage.Remove(key);

    public void Clear()
    {
        NumericStorage.Clear();
        CategoricalStorage.Clear();
    }

    IFactStorage IFactStorage.DeepCopy() =>
        DeepCopy();

    public string ToString(string? format, IFormatProvider? formatProvider) =>
        ToString();

    public override string ToString() =>
        $"{CategoricalStorage}{Environment.NewLine}{NumericStorage}";

    public static Enum ParseEnum(Type type, string constValue) =>
        type.IsEnum ? (Enum) Enum.Parse(type, constValue, ignoreCase: true) : throw new ArgumentException("Provided type must be an enum", nameof(type));
}