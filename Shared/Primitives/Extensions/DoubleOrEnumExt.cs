using Shared.Primitives.Implementation;

namespace Shared.Primitives.Extensions;

public static class DoubleOrEnumExt
{
    extension(DoubleOrEnum value)
    {
        public T AsTypedEnum<T>() where T : struct, Enum, IConvertible => (T) value.AsEnum;
    }
}