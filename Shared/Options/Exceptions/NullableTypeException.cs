namespace Shared.Options.Exceptions;

public class NullableTypeException : Exception
{
    private const string Template =
        """
        ‘T’ cannot be a nullable value type for Option[‘T’].
        Use Option[{0}] instead of Option[{0}?]/Option[Nullable<{0}>].
        """;

    public NullableTypeException(Type type) :
        base(string.Format(Template, type.Name))
    {
    }

    public NullableTypeException(Type type, Exception inner) :
        base(string.Format(Template, type.Name), inner)
    {
    }
}