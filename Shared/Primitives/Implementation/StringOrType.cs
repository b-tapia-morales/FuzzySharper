using OneOf;
using Shared.Primitives.Exceptions;

namespace Shared.Primitives.Implementation;

[GenerateOneOf]
public partial class StringOrType : OneOfBase<string, Type>, IEquatable<StringOrType>, IEqualityComparer<StringOrType>
{
    public bool IsString => IsT0;
    public bool IsType => IsT1;

    public string AsString => IsString ? AsT0 : throw new TypeMismatchException(nameof(StringOrType), nameof(String), nameof(Type));

    public Type AsType => IsType ? AsT1 : throw new TypeMismatchException(nameof(StringOrType), nameof(Type), nameof(String));

    public override bool Equals(object? obj) =>
        obj is StringOrType other && Equals(this, other);

    public bool Equals(StringOrType? other) =>
        Equals(this, other);

    public bool Equals(StringOrType? x, StringOrType? y)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (x is null || y is null)
            return false;
        if (x.IsType && y.IsType)
            return EqualityComparer<Type>.Default.Equals(x.AsType, y.AsType);
        if (x.IsString && y.IsString)
            return string.Equals(x.AsString, y.AsString, StringComparison.OrdinalIgnoreCase);
        return false;
    }

    public override int GetHashCode() =>
        GetHashCode(this);

    public int GetHashCode(StringOrType obj) =>
        obj.IsType ? obj.AsType.GetHashCode() : obj.AsString.ToLower().GetHashCode();
}