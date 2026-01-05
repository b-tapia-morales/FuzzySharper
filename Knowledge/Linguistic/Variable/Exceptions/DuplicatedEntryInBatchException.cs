namespace Knowledge.Linguistic.Variable.Exceptions;

public class DuplicatedEntryInBatchException : Exception
{
    private const string Template =
        """
        An attempt has been made to add a collection of Linguistic Variables that use the same variable name. Linguistic variable names must be unique within a collection.
        The Linguistic Variable name is: «{0}»
        """;

    public DuplicatedEntryInBatchException(string variableName) : base(string.Format(Template, variableName))
    {
    }

    public DuplicatedEntryInBatchException(string variableName, Exception inner) : base(string.Format(Template, variableName), inner)
    {
    }
}