using Shared.Options.Exceptions;
using Shared.Options.Implementations;

namespace Shared.Options.Factory;

/// <summary>Provides option creation, nullable-input conversion, and presence-checking extensions.</summary>
/// <remarks>
/// Present options created by these factories contain non-null values. To convert a nullable input,
/// select the non-nullable stored type and use <c>Maybe</c>, <c>SomeNullable</c>, or <c>MaybeNullable</c>.
/// </remarks>
public static class OptionFactory
{
    // Non-nullable reference or value types
    extension<T>(Option<T> option) where T : notnull
    {
        /// <summary>Creates a present option containing the supplied non-null value, including default values.</summary>
        /// <param name="value">The value to store; zero, false, and empty strings are accepted as present values.</param>
        /// <returns>An option containing the supplied value.</returns>
        /// <exception cref="ArgumentNullException">The supplied value is null.</exception>
        /// <exception cref="NullableTypeException">The stored type is a nullable value type.</exception>
        public static Option<T> Some(T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            CheckNullable<T>();
            return new Option<T>(value);
        }

        /// <summary>Creates an option with no contained value.</summary>
        /// <returns>An empty option of the selected stored type.</returns>
        /// <exception cref="NullableTypeException">The stored type is a nullable value type.</exception>
        public static Option<T> None()
        {
            CheckNullable<T>();
            return Null.GetInstance;
        }

        /// <summary>Reports whether this option is present and supplies its immediate value when it is.</summary>
        /// <param name="value">Receives the contained value when present, or the stored type's default value when empty.</param>
        /// <returns>True when a contained value was supplied; false when the option is empty.</returns>
        public bool IsSome(out T value)
        {
            value = default!;
            if (option.IsNone)
                return false;
            value = option.Get;
            return true;
        }

        /// <summary>Gets whether this option has no value.</summary>
        public bool IsNone => option.IsNone;
    }

    // Reference type that extracts the underlying value from a nullable reference type.
    extension<T>(Option<T> option) where T : class
    {
        /// <summary>Converts a nullable reference into a present option for a non-null reference, or an empty option for null.</summary>
        /// <param name="value">The reference whose presence determines the resulting option's state.</param>
        /// <returns>An option containing the supplied reference when non-null, or an empty option when null.</returns>
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
        /// <summary>Extracts the underlying value from a populated nullable value and stores it in a present option.</summary>
        /// <param name="value">The nullable input that must contain a value of the selected non-nullable stored type.</param>
        /// <returns>A present option containing the nullable input's underlying value.</returns>
        /// <exception cref="ArgumentNullException">The nullable input has no value.</exception>
        public static Option<T> SomeNullable(T? value) =>
            value.HasValue ? Option<T>.Some(value.Value) : throw new ArgumentNullException(nameof(value));

        /// <summary>Converts a nullable value into an option of its non-nullable underlying type.</summary>
        /// <param name="maybe">The nullable input whose presence determines the resulting option's state.</param>
        /// <returns>An option containing the underlying value when present, or an empty option when the input has no value.</returns>
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
