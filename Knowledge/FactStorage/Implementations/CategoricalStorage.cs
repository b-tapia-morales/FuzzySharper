using System.Diagnostics;
using Knowledge.FactStorage.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Knowledge.FactStorage.Implementations;

public sealed class CategoricalStorage : IFactStorage<Type, Enum>
{
    private Dictionary<Type, Enum> Facts { get; }

    public IReadOnlySet<Type> Keys =>
        new HashSet<Type>(Facts.Keys);

    public CategoricalStorage() : this(new Dictionary<Type, Enum>())
    {
    }

    private CategoricalStorage(Dictionary<Type, Enum> facts) =>
        Facts = new Dictionary<Type, Enum>(facts);

    public bool Contains(Type key) =>
        Facts.ContainsKey(key);

    public Option<Enum> GetValue(Type key) =>
        Contains(key) ? Facts[key] : Option<Enum>.None();

    public void AddValue(Type key, Enum value)
    {
        Debug.Assert(Enum.IsDefined(key, value) && value.GetType() == key);
        Facts[key] = value;
    }

    public bool TryAddValue(Type key, Enum value)
    {
        Debug.Assert(Enum.IsDefined(key, value) && value.GetType() == key);
        return Facts.TryAdd(key, value);
    }

    public bool Remove(Type key) =>
        Facts.Remove(key);

    public void Clear() =>
        Facts.Clear();

    public IFactStorage<Type, Enum> DeepCopy() =>
        new CategoricalStorage(Facts);

    public string ToString(string? format, IFormatProvider? formatProvider) =>
        ToString();

    public override string ToString() =>
        string.Join(Environment.NewLine, Facts.Select(pair => $"{pair.Key.Name}: {pair.Value}"));
}