namespace Reasoning.Rule.Exceptions;

public class ConflictingVariableException : Exception
{
    private const string Template =
        """
        The following rule creation policy has been violated: A linguistic variable cannot be stored in multiple bases under the same name.
        Conflicting variable «{0}» found in both local base and foreign base.
        """;

    public ConflictingVariableException(string variableName) : base(string.Format(Template, variableName))
    {
    }

    public ConflictingVariableException(string variableName, Exception inner) : base(string.Format(Template, variableName), inner)
    {
    }
}