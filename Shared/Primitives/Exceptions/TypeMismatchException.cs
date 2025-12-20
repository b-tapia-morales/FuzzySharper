namespace Shared.Primitives.Exceptions;

public class TypeMismatchException : InvalidOperationException
{
    private const string Template =
        "An attempt has been made to unwrap «{0}» as a «{1}», but the underlying value is a «{2}».";

    public TypeMismatchException(string wrapperType, string expectedType, string actualType) : 
        base(string.Format(Template, wrapperType, expectedType, actualType))
    {
    }

    public TypeMismatchException(string wrapperType, string expectedType, string actualType, Exception inner) : 
        base(string.Format(Template, wrapperType, expectedType, actualType), inner)
    {
    }
}