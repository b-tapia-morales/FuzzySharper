using Shared.Options.Exceptions;
using Shared.Options.Implementations;

namespace Shared.Options.Factory;

public static class OptionFactory
{
    extension<T>(Option<T> option)
    {
        public static Option<T> Some(T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new Option<T>(value);
        }

        public static Option<T> Maybe(T? value) =>
            value == null ? Null.GetInstance : new Option<T>(value);
        
        public static Option<T> None()
        {
            var underlyingType = GetNullableType<T>();
            return underlyingType != null ? throw new NullableTypeException(underlyingType) : Null.GetInstance;
        }
        
        public bool IsSome(out T value)
        {
            value = default!;
            if (option.IsNone)
                return false;
            value = option.Get;
            return true;
        }
    }

    extension<T>(Option<T> option) where T : struct
    {
        public static Option<T> SomeNullable(T? value) =>
            value.HasValue ? Option<T>.Some(value.Value) : throw new ArgumentNullException(nameof(value));

        public static Option<T> MaybeNullable(T? maybe) =>
            maybe.HasValue ? Option<T>.Maybe(maybe.Value) : Option<T>.None();
    }

    private static Type? GetNullableType<T>() => Nullable.GetUnderlyingType(typeof(T));
}