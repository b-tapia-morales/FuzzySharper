using Shared.Options.Implementations;

namespace Knowledge.FactStorage.Abstractions;

/// <summary>
/// Represents a generic storage abstraction for <b>facts</b>, modeled as key–value pairs, supporting insertion,
/// retrieval, removal, and bulk operations.
/// </summary>
public interface IFactStorage<TKey, TValue> : IFormattable where TKey : notnull
{
    /// <summary>
    /// Gets the set of keys for all facts currently stored.
    /// </summary>
    IReadOnlySet<TKey> Keys { get; }

    /// <summary>
    /// Determines whether a fact identified by the specified key is present in the storage.
    /// </summary>
    /// <param name="key">The fact key.</param>
    /// <returns>
    /// <see langword="true"/> if a fact with the specified key is present; otherwise, <see langword="false"/>.
    /// </returns>
    bool Contains(TKey key);

    /// <summary>
    /// Retrieves the value associated with the specified key, if present.
    /// </summary>
    /// <param name="key">The fact key.</param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the associated value if present; otherwise, an empty option.
    /// </returns>
    Option<TValue> GetValue(TKey key);

    /// <summary>
    /// Adds or replaces the value associated with the specified key.
    /// </summary>
    /// <param name="key">The fact key.</param>
    /// <param name="value">The fact value.</param>
    void AddValue(TKey key, TValue value);

    /// <summary>
    /// Attempts to add a new fact to the storage without replacing an existing one.
    /// </summary>
    /// <param name="key">The fact key.</param>
    /// <param name="value">The fact value.</param>
    /// <returns>
    /// <see langword="true"/> if the fact was successfully added; otherwise, <see langword="false"/>, and the
    /// already existing fact is preserved.
    /// </returns>
    bool TryAddValue(TKey key, TValue value);

    /// <summary>
    /// Removes the fact associated with the specified key, if present.
    /// </summary>
    /// <param name="key">The fact key.</param>
    /// <returns>
    /// <see langword="true"/> if the fact was successfully found and removed; otherwise, <see langword="false"/>.
    /// </returns>
    bool Remove(TKey key);

    /// <summary>
    /// Adds multiple facts to the storage from a sequence of key–value tuples.
    /// </summary>
    /// <param name="tuples">The facts to add.</param>
    /// <param name="replaceExisting">
    /// Specifies whether existing facts with matching keys should be replaced. Defaults to <see langword="true"/>.
    /// </param>
    void AddRange(IEnumerable<(TKey, TValue)> tuples, bool replaceExisting = true)
    {
        foreach (var (key, value) in tuples)
        {
            if (Contains(key) && !replaceExisting)
                continue;
            AddValue(key, value);
        }
    }

    /// <summary>
    /// Adds multiple facts to the storage from a sequence of key–value pairs.
    /// </summary>
    /// <param name="pairs">The facts to add.</param>
    /// <param name="replaceExisting">
    /// Specifies whether existing facts with matching keys should be replaced. Defaults to <see langword="true"/>.
    /// </param>
    void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs, bool replaceExisting = true)
    {
        foreach (var (key, value) in pairs)
        {
            if (Contains(key) && !replaceExisting)
                continue;
            AddValue(key, value);
        }
    }

    /// <summary>
    /// Removes all facts from the storage.
    /// </summary>
    void Clear();

    /// <summary>
    /// Creates a deep copy of the current fact storage instance.
    /// </summary>
    /// <returns>
    /// A new <see cref="IFactStorage{TKey, TValue}"/> containing the same facts as the original instance.
    /// </returns>
    IFactStorage<TKey, TValue> DeepCopy();
}