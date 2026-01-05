using Shared.Options.Exceptions;
using Shared.Options.Implementations;

namespace Shared.Options.Factory;

public static class OptionFactory
{
    // Non-nullable reference or value types
    extension<T>(Option<T> option) where T : notnull
    {
        public static Option<T> Some(T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            CheckNullable<T>();
            return new Option<T>(value);
        }

        public static Option<T> None()
        {
            CheckNullable<T>();
            return Null.GetInstance;
        }

        public bool IsSome(out T value)
        {
            value = default!;
            if (option.IsNone)
                return false;
            value = option.Get;
            return true;
        }

        public bool IsNone => option.IsNone;
    }

    // Reference type that extracts the underlying value from a nullable reference type.
    extension<T>(Option<T> option) where T : class
    {
        public static Option<T> Maybe(T? value)
        {
            CheckNullable<T>();
            return value == null ? Null.GetInstance : new Option<T>(value);
        }
    }

    // Value type that extracts the underlying value from a nullable value type.
    // T is always a value type, never a nullable value type.
    extension<T>(Option<T> option) where T : struct
    {
        public static Option<T> SomeNullable(T? value) =>
            value.HasValue ? Option<T>.Some(value.Value) : throw new ArgumentNullException(nameof(value));

        public static Option<T> MaybeNullable(T? maybe) =>
            maybe.HasValue ? Option<T>.Some(maybe.Value) : Option<T>.None();
    }

    private static void CheckNullable<T>()
    {
        var nullableType = Nullable.GetUnderlyingType(typeof(T));
        var isNullable = nullableType != null;
        if (isNullable)
            throw new NullableTypeException(nullableType!);
    }
}