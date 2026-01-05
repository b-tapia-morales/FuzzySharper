namespace Knowledge.Linguistic.Base.Exceptions;

public class DuplicateVariableException : Exception
{
    private const string TemplateSingle =
        """
        An attempt has been made to add a Linguistic Variable whose name is already in use to the Linguistic Base.
        The Linguistic variable's name is: «{0}»
        """;

    private const string TemplateMultiple =
        """
        An attempt has been made to add Linguistic Variables whose names are already in use to the Linguistic Base.
        The Linguistic Variable names found to be duplicates are the following: «{0}»
        """;

    public DuplicateVariableException(string variableName) : base(string.Format(TemplateSingle, variableName))
    {
    }

    public DuplicateVariableException(IEnumerable<string> variableNames) : base(string.Format(TemplateMultiple, string.Join(", ", variableNames)))
    {
    }

    public DuplicateVariableException(string variableName, Exception inner) : base(
        string.Format(TemplateSingle, variableName), inner)
    {
    }
}