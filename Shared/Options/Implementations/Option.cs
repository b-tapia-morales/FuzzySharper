using OneOf;
using Shared.Options.Abstractions;
using Shared.Options.Exceptions;

namespace Shared.Options.Implementations;

/// <summary>
/// <para>
/// A container that may or may not contain a value of type <c>T</c>.
/// If a value is present, <see cref="IsSome"/> returns <see langword="true"/>
/// and <see cref="Get"/> returns the contained value.
/// Otherwise, <see cref="IsNone"/> returns <see langword="true"/>
/// and <see cref="Get"/> throws <see cref="MissingValueException"/>.
/// </para>
/// <para>
/// Extension methods in <see cref="Shared.Options.Factory.OptionFactory">OptionFactory</see>
/// provide ways to create options.
/// <c>None</c> creates an empty option, and <c>Some</c> creates an option
/// containing the supplied non-null value.
/// <c>Maybe</c> creates a present option if the supplied reference is non-null,
/// or an empty option otherwise.
/// <c>SomeNullable</c> and <c>MaybeNullable</c> create options from nullable
/// value types.
/// </para>
/// <para>
/// Extension methods in <see cref="Shared.Options.Extensions.OptionExt">OptionExt</see>
/// provide operations that depend on whether a value is present,
/// such as <c>OrElse</c>, <c>IfPresent</c> and <c>IfEmpty</c>.
/// </para>
/// </summary>
/// <typeparam name="T">The type of the contained value when present.</typeparam>
/// <remarks>
/// <para>
/// Absence is represented by an instance of <see cref="Null"/>,
/// rather than a null reference.
/// </para>
/// <para>
/// Two options of the same value type are equal if both are empty,
/// or if both contain values considered equal by
/// <see cref="EqualityComparer{T}.Default"/>.
/// </para>
/// </remarks>
[GenerateOneOf]
public partial class Option<T> : OneOfBase<T, Null>, IOption, IEquatable<Option<T>>, IEqualityComparer<Option<T>>
{
    /// <summary>Returns <see langword="true"/> if there is a value present, otherwise <see langword="false"/>.</summary>
    public bool IsSome => IsT0;

    /// <summary>Returns <see langword="true"/> if the option is empty, otherwise <see langword="false"/>.</summary>
    public bool IsNone => IsT1;

    /// <summary>If a value is present, returns the value, otherwise throws <see cref="MissingValueException"/>.</summary>
    /// <exception cref="MissingValueException">The option is empty.</exception>
    public T Get => IsSome ? AsT0 : throw new MissingValueException(nameof(T));

    /// <summary>
    /// Indicates whether some other object is equal to this instance. The other object is considered equal if:
    /// <list type="bullet">
    /// <item>It is also an <c>Option</c></item>
    /// <item>It is non-null.</item>
    /// <item>Both instances are empty, or both contain values considered equal by <see cref="EqualityComparer{T}.Default"/>.</item>
    /// </list>
    /// </summary>
    /// <param name="obj">The object to test for equality.</param>
    /// <returns>
    /// <see langword="true"/> if the other object is equal to this instance; otherwise, false.
    /// </returns>
    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || obj is Option<T> other && Equals(other);

    /// <summary>
    /// Indicates whether another <c>Option</c> is equal to this instance. The other instance is considered equal if:
    /// <list type="bullet">
    /// <item>It is non-null.</item>
    /// <item>Both instances are empty, or both contain values considered equal by <see cref="EqualityComparer{T}.Default"/>.</item>
    /// </list>
    /// </summary>
    /// <param name="other">The object to test for equality.</param>
    /// <returns>
    /// <see langword="true"/> if the other <c>Option</c> is equal to this instance; otherwise, false.
    /// </returns>
    public bool Equals(Option<T>? other) =>
        other != null && (
            IsNone && other.IsNone ||
            IsSome && other.IsSome && EqualityComparer<T>.Default.Equals(AsT0, other.AsT0)
        );

    /// <summary>
    /// Indicates whether two instances are equal to each other. The two instances are considered equal if both are null,
    /// both are empty, or both contain values considered equal by <see cref="EqualityComparer{T}.Default"/>.
    /// If exactly one instance is null, they are unequal.
    /// </summary>
    /// <param name="x">The first instance</param>
    /// <param name="y">The second instance</param>
    /// <returns>
    /// <see langword="true"/> if the two instances are equal to each other; otherwise, false.
    /// </returns>
    public bool Equals(Option<T>? x, Option<T>? y) =>
        ReferenceEquals(x, y) || x != null && x.Equals(y);

    /// <summary>
    /// Returns a hash code for this instance, which evaluates to zero if it's empty, or the hash code of its underlying value
    /// if it's present.
    /// </summary>
    /// <returns>The hash code for this instance.</returns>
    public override int GetHashCode() =>
        IsNone ? 0 : AsT0.GetHashCode();

    /// <summary>
    /// Returns a hash code for the supplied instance, which evaluates to zero if it's empty, or the hash code of its underlying value
    /// if it's present.</summary>
    /// <param name="obj">The supplied instance.</param>
    /// <returns>The hash code for the supplied instance.</returns>
    public int GetHashCode(Option<T> obj) =>
        obj.GetHashCode();

    /// <summary>
    /// Returns a string that represents this instance using the format <c>Option[type name] = value</c>,
    /// with no text after the equals symbol when the value is absent.
    /// </summary>
    /// <returns>The string representation for this instance.</returns>
    public override string ToString()
    {
        var value = IsSome ? AsT0.ToString() : string.Empty;
        return $"Option[{typeof(T).Name}] = {value}";
    }

    /// <summary>Gets the immediate contained value as an object, boxing value types.</summary>
    /// <exception cref="MissingValueException">This option is empty.</exception>
    object IOption.Get => Get;
}