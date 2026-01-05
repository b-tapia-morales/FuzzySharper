using Knowledge.FactStorage.Abstractions;
using Knowledge.FactStorage.Exceptions;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Knowledge.FactStorage.Implementations;

public sealed class NumericStorage : IFactStorage
{
    private Dictionary<string, double> Facts { get; }

    public IReadOnlySet<StringOrType> Keys => 
        new HashSet<StringOrType>(Facts.Keys.Select(e => (StringOrType) e));

    public NumericStorage() : this(new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase))
    {
    }

    private NumericStorage(Dictionary<string, double> facts) =>
        Facts = new Dictionary<string, double>(facts, StringComparer.OrdinalIgnoreCase);

    public bool Contains(StringOrType key) =>
        key.IsString ? Facts.ContainsKey(key.AsString) : throw new InvalidKeyException(GetType().Name, nameof(String), nameof(Type));

    public Option<DoubleOrEnum> GetValue(StringOrType key) =>
        Contains(key) ? Option<DoubleOrEnum>.Some(Facts[key.AsString]) : Option<DoubleOrEnum>.None();

    public void AddValue(StringOrType key, DoubleOrEnum value)
    {
        if (!key.IsString)
            throw new InvalidKeyException(GetType().Name, nameof(String), nameof(Type));
        if (!value.IsDouble)
            throw new InvalidValueException(GetType().Name, nameof(Double), nameof(Enum));
        Facts[key.AsString] = value.AsDouble;
    }

    public bool TryAddValue(StringOrType key, DoubleOrEnum value)
    {
        if (!key.IsString)
            throw new InvalidKeyException(GetType().Name, nameof(String), nameof(Type));
        if (!Contains(key))
            return false;
        AddValue(key, value);
        return true;
    }

    public bool Remove(StringOrType key) =>
        key.IsString ? Facts.Remove(key.AsString) : throw new InvalidKeyException(GetType().Name, nameof(String), nameof(Type));

    public void Clear() =>
        Facts.Clear();

    public IFactStorage DeepCopy() =>
        new NumericStorage(Facts);

    public string ToString(string? format, IFormatProvider? formatProvider) =>
        ToString();

    public override string ToString() =>
        string.Join(Environment.NewLine, Facts.Select(pair => $"{pair.Key}: {pair.Value}"));
}