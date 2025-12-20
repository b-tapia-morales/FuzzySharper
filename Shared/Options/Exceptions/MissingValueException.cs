namespace Shared.Options.Exceptions;

public class MissingValueException : InvalidOperationException
{
    private const string Template =
        "An attempt has been made to unwrap an Option as a «{0}», but the underlying value is missing.";

    public MissingValueException(string expectedType) :
        base(string.Format(Template, expectedType))
    {
    }

    public MissingValueException(string expectedType, Exception inner) :
        base(string.Format(Template, expectedType), inner)
    {
    }
}