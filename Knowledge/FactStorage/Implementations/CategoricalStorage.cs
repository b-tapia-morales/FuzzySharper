using Knowledge.FactStorage.Abstractions;
using Knowledge.FactStorage.Exceptions;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Knowledge.FactStorage.Implementations;

public sealed class CategoricalStorage : IFactStorage
{
    private Dictionary<Type, Enum> Facts { get; }

    public IReadOnlySet<StringOrType> Keys =>
        new HashSet<StringOrType>(Facts.Keys.Select(e => (StringOrType) e));

    public CategoricalStorage() : this(new Dictionary<Type, Enum>())
    {
    }

    private CategoricalStorage(Dictionary<Type, Enum> facts) =>
        Facts = new Dictionary<Type, Enum>(facts);

    public bool Contains(StringOrType key) =>
        key.IsType ? Facts.ContainsKey(key.AsType) : throw new InvalidKeyException(GetType().Name, nameof(Type), nameof(String));

    public Option<DoubleOrEnum> GetValue(StringOrType key) =>
        Contains(key) ? Option<DoubleOrEnum>.Some(Facts[key.AsType]) : Option<DoubleOrEnum>.None();

    public void AddValue(StringOrType key, DoubleOrEnum value)
    {
        if (!key.IsType)
            throw new InvalidKeyException(GetType().Name, nameof(Type), nameof(String));
        if (!value.IsEnum)
            throw new InvalidValueException(GetType().Name, nameof(Enum), nameof(Double));
        Facts[key.AsType] = value.AsEnum;
    }

    public bool TryAddValue(StringOrType key, DoubleOrEnum value)
    {
        if (!key.IsType)
            throw new InvalidKeyException(GetType().Name, nameof(Type), nameof(String));
        if (!Contains(key))
            return false;
        AddValue(key, value);
        return true;
    }

    public bool Remove(StringOrType key) =>
        key.IsType ? Facts.Remove(key.AsType) : throw new InvalidKeyException(GetType().Name, nameof(Type), nameof(String));

    public void Clear() =>
        Facts.Clear();

    public IFactStorage DeepCopy() =>
        new CategoricalStorage(Facts);

    public string ToString(string? format, IFormatProvider? formatProvider) =>
        ToString();

    public override string ToString() =>
        string.Join(Environment.NewLine, Facts.Select(pair => $"{pair.Key.Name}: {pair.Value}"));
}