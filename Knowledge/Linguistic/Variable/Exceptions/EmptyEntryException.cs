namespace Knowledge.Linguistic.Variable.Exceptions;

public class EmptyEntryException : Exception
{
    private const string Template =
        """
        An attempt has been made to add to a linguistic variable a linguistic entry with a name that is either an empty or a whitespace string.
        The Linguistic variable is: «{0}»
        """;

    public EmptyEntryException() : base(Template)
    {
    }

    public EmptyEntryException(Exception inner) : base(Template, inner)
    {
    }
}