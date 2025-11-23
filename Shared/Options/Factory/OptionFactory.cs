using Shared.Options.Exceptions;
using Shared.Options.Implementations;

namespace Shared.Options.Factory;

public static class OptionFactory
{
    extension<T>(Option<T> option) where T : class
    {
        public static Option<T> SomeRef(T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new Option<T>(value);
        }

        public static Option<T> MaybeRef(T? value) =>
            value == null ? Null.GetInstance : new Option<T>(value);

        public bool IsSomeRef(out T value)
        {
            value = null!;
            if (option.IsNone)
                return false;
            value = option.Get;
            return true;
        }
    }

    extension<T>(Option<T> option) where T : struct
    {
        public static Option<T> SomeVal(T value) =>
            new(value);

        public static Option<T> SomeVal(T? value) =>
            value.HasValue ? SomeVal(value.Value) : throw new ArgumentNullException(nameof(value));

        public static Option<T> MaybeVal(T value) =>
            new(value);

        public static Option<T> MaybeVal(T? maybe) =>
            maybe.HasValue ? MaybeVal(maybe.Value) : None<T>();

        public bool IsSomeVal(out T value)
        {
            value = default;
            if (option.IsNone)
                return false;
            value = option.Get;
            return true;
        }
    }

    public static Option<T> None<T>()
    {
        var underlyingType = GetNullableType<T>();
        return underlyingType != null ? throw new NullableTypeException(underlyingType) : Null.GetInstance;
    }

    private static Type? GetNullableType<T>() => Nullable.GetUnderlyingType(typeof(T));
}