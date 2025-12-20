using Knowledge.FactStorage.Implementations;

namespace Knowledge.Memory.Exceptions;

public class InvalidPairException: ArgumentException
{
    private const string Template =
        """
        An attempt has been made to add a key-value pair of incompatible types.
        «{0}» expects a key-value pair of type «{1}» : «{2}».
        «{3}» expects a key-value pair of type «{4}» : «{5}».
        The actual provided type was: «{6}» : «{7}».
        """;

    public InvalidPairException(string keyType, string valueType) :
        base(string.Format(Template, nameof(CategoricalStorage), nameof(Type), nameof(Enum),
            nameof(NumericStorage), nameof(String), nameof(Double),
            keyType, valueType))
    {
    }

    public InvalidPairException(string keyType, string valueType, Exception inner) :
        base(string.Format(Template, nameof(CategoricalStorage), nameof(Type), nameof(Enum),
            nameof(NumericStorage), nameof(String), nameof(Double),
            keyType, valueType), inner)
    {
    }
}