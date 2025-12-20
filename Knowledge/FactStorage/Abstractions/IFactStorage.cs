using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Knowledge.FactStorage.Abstractions;

public interface IFactStorage : IFormattable
{
    ISet<StringOrType> Keys();

    bool Contains(StringOrType key);

    Option<DoubleOrEnum> GetValue(StringOrType key);

    void AddValue(StringOrType key, DoubleOrEnum value);

    bool TryAddValue(StringOrType key, DoubleOrEnum value);

    bool Remove(StringOrType key);

    void AddRange(IEnumerable<(StringOrType, DoubleOrEnum)> tuples, bool replaceExisting = true)
    {
        foreach (var (key, value) in tuples)
        {
            if (Contains(key) && !replaceExisting)
                continue;
            AddValue(key, value);
        }
    }

    void AddRange(IEnumerable<KeyValuePair<StringOrType, DoubleOrEnum>> pairs, bool replaceExisting = true)
    {
        foreach (var (key, value) in pairs)
        {
            if (Contains(key) && !replaceExisting)
                continue;
            AddValue(key, value);
        }
    }

    void Clear();

    IFactStorage DeepCopy();
}