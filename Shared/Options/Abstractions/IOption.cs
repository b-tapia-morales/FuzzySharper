using Shared.Options.Exceptions;

namespace Shared.Options.Abstractions;

/// <summary>
/// Represents an option independently of its contained value's type.
/// A present value of any type is exposed as an <see langword="object"/>, allowing options with
/// different value types to be accessed through the same interface.
/// </summary>
public interface IOption
{
    /// <summary>
    /// If a value is present, returns it as an <see langword="object"/>;
    /// otherwise, throws <see cref="MissingValueException"/>.
    /// </summary>
    /// <returns>The contained value as an object.</returns>
    /// <exception cref="MissingValueException">The option is empty.</exception>
    object Get { get; }

    /// <summary>
    /// Returns <see langword="true"/> if the option is empty; otherwise, <see langword="false"/>.
    /// </summary>
    /// <returns>Whether the option is empty.</returns>
    bool IsNone { get; }
}