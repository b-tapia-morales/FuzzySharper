namespace Shared.Options.Exceptions;

/// <summary>The exception thrown when an empty option's contained value is requested.</summary>
public class MissingValueException : InvalidOperationException
{
    private const string Template =
        "An attempt has been made to unwrap an Option as a «{0}», but the underlying value is missing.";

    /// <summary>Creates a missing-value exception whose message identifies the expected value type.</summary>
    /// <param name="expectedType">The expected type's name or label to include in the message.</param>
    public MissingValueException(string expectedType) :
        base(string.Format(Template, expectedType))
    {
    }

    /// <summary>Creates a missing-value exception with an expected type label and an underlying exception.</summary>
    /// <param name="expectedType">The expected type's name or label to include in the message.</param>
    /// <param name="inner">The exception retained as the cause of this exception.</param>
    public MissingValueException(string expectedType, Exception inner) :
        base(string.Format(Template, expectedType), inner)
    {
    }
}
