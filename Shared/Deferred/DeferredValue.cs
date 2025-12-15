using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Shared.Deferred;

public sealed class DeferredValue<T>(Func<T> compute, Action resolvePrerequisites)
{
    private Option<T> _cachedValue = OptionFactory.None<T>();

    public DeferredValue(T value) : this(() => value, () => { })
    {
        _cachedValue = value;
    }

    public T Value
    {
        get
        {
            if (_cachedValue.IsSome)
                return _cachedValue.Get;
            Compute();
            return _cachedValue.Get;
        }
    }

    public bool HasValue =>
        _cachedValue.IsSome;

    public void Invalidate() =>
        _cachedValue = OptionFactory.None<T>();

    public void Compute()
    {
        if (_cachedValue.IsSome)
            return;

        resolvePrerequisites();
        _cachedValue = compute();
    }

    public void Recompute()
    {
        Invalidate();
        Compute();
    }

    public void OverrideValue(T value) =>
        _cachedValue = value;

    public bool TryGetValue(out T value)
    {
        value = _cachedValue.IsSome ? _cachedValue.Get : default!;
        return _cachedValue.IsSome;
    }
}