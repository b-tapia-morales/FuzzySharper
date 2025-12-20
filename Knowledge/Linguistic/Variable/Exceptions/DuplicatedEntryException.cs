namespace Knowledge.Linguistic.Variable.Exceptions;

public class DuplicatedEntryException : Exception
{
    private const string Template =
        """
        An attempt has been made to add a Semantic Mapping whose name is already in use to a Linguistic Variable.
        The Linguistic Variable is: «{0}»
        The Semantic Mapping is: «{1}»
        """;

    public DuplicatedEntryException(string variableName, string termName) : base(string.Format(Template, variableName,
        termName))
    {
    }

    public DuplicatedEntryException(string variableName, string termName, Exception inner) : base(
        string.Format(Template, variableName, termName), inner)
    {
    }
}