using Knowledge.FactStorage.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Knowledge.FactStorage.Implementations;

public sealed class NumericStorage : IFactStorage<string, double>
{
    private Dictionary<string, double> Facts { get; }

    public IReadOnlySet<string> Keys =>
        new HashSet<string>(Facts.Keys, StringComparer.OrdinalIgnoreCase);

    public NumericStorage() : this(new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase))
    {
    }

    private NumericStorage(Dictionary<string, double> facts) =>
        Facts = new Dictionary<string, double>(facts, StringComparer.OrdinalIgnoreCase);

    public bool Contains(string key) =>
        Facts.ContainsKey(key);

    public Option<double> GetValue(string key) =>
        Contains(key) ? Facts[key] : Option<double>.None();

    public void AddValue(string key, double value) =>
        Facts[key] = value;

    public bool TryAddValue(string key, double value) => 
        Facts.TryAdd(key, value);

    public bool Remove(string key) =>
        Facts.Remove(key);

    public void Clear() =>
        Facts.Clear();

    public IFactStorage<string, double> DeepCopy() =>
        new NumericStorage(Facts);

    public string ToString(string? format, IFormatProvider? formatProvider) =>
        ToString();

    public override string ToString() =>
        string.Join(Environment.NewLine, Facts.Select(pair => $"{pair.Key}: {pair.Value}"));
}