using OneOf;
using Shared.Primitives.Exceptions;

namespace Shared.Primitives.Implementation;

[GenerateOneOf]
public partial class StringOrType : OneOfBase<string, Type>, IEquatable<StringOrType>
{
    public bool IsString => IsT0;
    public bool IsType => IsT1;

    public string AsString => IsString ? AsT0 : throw new TypeMismatchException(nameof(StringOrType), nameof(String), nameof(Type));

    public Type AsType => IsType ? AsT1 : throw new TypeMismatchException(nameof(StringOrType), nameof(Type), nameof(String));

    public override bool Equals(object? obj) =>
        obj is StringOrType other && Equals(other);

    public bool Equals(StringOrType? other)
    {
        if (ReferenceEquals(this, other))
            return true;
        if (other is null)
            return false;
        if (IsType && other.IsType)
            return EqualityComparer<Type>.Default.Equals(AsType, other.AsType);
        if (IsString && other.IsString)
            return string.Equals(AsString, other.AsString, StringComparison.OrdinalIgnoreCase);
        return false;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        Switch(str => hash.Add(str, StringComparer.OrdinalIgnoreCase), type => hash.Add(type));
        return hash.ToHashCode();
    }
}