namespace Knowledge.FactStorage.Exceptions;

public class InvalidKeyException : ArgumentException
{
    private const string Template =
        """
        An attempt has been made to use a key of the wrong type to access values.
        «{0}» expected a key of type «{1}», but «{2}» was received instead.
        """;

    public InvalidKeyException(string actualType, string storageType, string expectedType) :
        base(string.Format(Template, actualType, storageType, expectedType))
    {
    }

    public InvalidKeyException(string actualType, string storageType, string expectedType, Exception inner) :
        base(string.Format(Template, actualType, storageType, expectedType), inner)
    {
    }
}