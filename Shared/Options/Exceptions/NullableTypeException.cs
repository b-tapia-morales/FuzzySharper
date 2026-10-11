namespace Shared.Options.Exceptions;

/// <summary>The exception thrown when an option factory is used with a nullable value type as its stored type.</summary>
/// <remarks>
/// Use the non-nullable underlying type as the option's stored type and convert nullable inputs
/// with <c>SomeNullable</c> or <c>MaybeNullable</c>.
/// </remarks>
public class NullableTypeException : Exception
{
    private const string Template =
        """
        ‘T’ cannot be a nullable value type for Option[‘T’].
        Use Option[{0}] instead of Option[{0}?]/Option[Nullable<{0}>].
        """;

    /// <summary>Creates an exception explaining which non-nullable stored type should replace the rejected nullable type.</summary>
    /// <param name="type">The non-nullable underlying value type of the rejected nullable type, used in the message.</param>
    public NullableTypeException(Type type) :
        base(string.Format(Template, type.Name))
    {
    }

    /// <summary>Creates a nullable-type exception with a suggested stored type and an underlying exception.</summary>
    /// <param name="type">The non-nullable underlying value type of the rejected nullable type, used in the message.</param>
    /// <param name="inner">The exception retained as the cause of this exception.</param>
    public NullableTypeException(Type type, Exception inner) :
        base(string.Format(Template, type.Name), inner)
    {
    }
}
