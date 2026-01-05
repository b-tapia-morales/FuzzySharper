namespace Knowledge.Linguistic.Base.Exceptions;

public class UndefinedVariableException : Exception
{
    private const string Template =
        """
        An attempt has been made to add a semantically undefined Linguistic Variable «{0}».
        A linguistic variable must define at least one linguistic term.
        """;

    public UndefinedVariableException(string argumentName) :
        base(string.Format(Template, argumentName))
    {
    }

    public UndefinedVariableException(string argumentName, Exception inner) :
        base(string.Format(Template, argumentName), inner)
    {
    }
}