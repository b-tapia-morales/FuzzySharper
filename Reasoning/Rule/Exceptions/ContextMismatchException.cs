namespace Reasoning.Rule.Exceptions;

public class ContextMismatchException : Exception
{
    private const string Template =
        """
        The following rule creation policy has been violated:
        A context-bound proposition cannot be used inside a context-free rule. This rule does not have access to a linguistic base.
        """;

    public ContextMismatchException() : base(Template)
    {
    }

    public ContextMismatchException(Exception inner) : base(Template, inner)
    {
    }
}