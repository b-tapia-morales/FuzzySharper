using OneOf;
using Shared.Primitives.Exceptions;

namespace Shared.Primitives.Implementation;

[GenerateOneOf]
public partial class DoubleOrEnum : OneOfBase<double, Enum>
{
    public bool IsDouble => IsT0;
    public bool IsEnum => IsT1;

    public double AsDouble => IsDouble ? AsT0 : throw new TypeMismatchException(nameof(DoubleOrEnum), nameof(Double), nameof(Enum));

    public Enum AsEnum => IsEnum ? AsT1 : throw new TypeMismatchException(nameof(DoubleOrEnum), nameof(Enum), nameof(Double));
}