namespace Knowledge.FactStorage.Exceptions;

public class InvalidValueException : ArgumentException
{
    private const string Template =
        """
        An attempt has been made to add a value of the wrong type.
        «{0}» expected a key of type «{1}», but «{2}» was received instead.
        """;

    public InvalidValueException(string actualType, string storageType, string expectedType) :
        base(string.Format(Template, actualType, storageType, expectedType))
    {
    }

    public InvalidValueException(string actualType, string storageType, string expectedType, Exception inner) :
        base(string.Format(Template, actualType, storageType, expectedType), inner)
    {
    }
}