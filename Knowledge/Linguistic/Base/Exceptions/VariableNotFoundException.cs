namespace Knowledge.Linguistic.Base.Exceptions;

public class VariableNotFoundException : Exception
{
    private const string Template =
        """
        An attempt has been made to retrieve a linguistic variable that is not contained in the Linguistic Base.
        The Linguistic variable is: «{0}»
        """;

    public VariableNotFoundException(string variableName) : base(string.Format(Template, variableName))
    {
    }

    public VariableNotFoundException(string variableName, Exception inner) : base(string.Format(Template, variableName),
        inner)
    {
    }
}