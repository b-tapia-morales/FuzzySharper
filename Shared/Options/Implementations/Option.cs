using OneOf;
using Shared.Options.Exceptions;

namespace Shared.Options.Implementations;

[GenerateOneOf]
public partial class Option<T> : OneOfBase<T, Null>, IEquatable<Option<T>>, IEqualityComparer<Option<T>>
{
    public bool IsSome => IsT0;
    public bool IsNone => IsT1;

    public T Get => IsSome ? AsT0 : throw new MissingValueException(nameof(T));

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || obj is Option<T> other && Equals(other);

    public bool Equals(Option<T>? other)
        => other != null && (IsNone && other.IsNone || IsSome && other.IsSome && EqualityComparer<T>.Default.Equals(AsT0, other.AsT0));

    public bool Equals(Option<T>? x, Option<T>? y) =>
        ReferenceEquals(x, y) || x != null && x.Equals(y);

    public override int GetHashCode() =>
        IsNone ? 0 : AsT0.GetHashCode();

    public int GetHashCode(Option<T> obj) =>
        obj.GetHashCode();

    public override string ToString()
    {
        var value = IsSome ? AsT0.ToString() : string.Empty;
        return $"Option[{typeof(T).Name}] = {value}";
    }
}