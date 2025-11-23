using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Shared.Deferred;

public sealed class DeferredValue<T>(Func<T> compute, Action resolvePrerequisites) where T : struct
{
    private Option<T> _cachedValue = OptionFactory.None<T>();

    public bool HasValue => _cachedValue.IsSome;

    public T Value
    {
        get
        {
            if (_cachedValue.IsSomeVal(out var value))
                return value;

            resolvePrerequisites();

            var result = compute();
            _cachedValue = OptionFactory.SomeVal(result);

            return result;
        }
    }
}