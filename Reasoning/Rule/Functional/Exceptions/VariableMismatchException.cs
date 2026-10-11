namespace Reasoning.Rule.Functional.Exceptions;

public class VariableMismatchException : InvalidOperationException
{
    private const string Template =
        """
        The following rule creation policy has been violated: each unique premise identifier must correspond to exactly one coefficient.
        The following identifiers were expected, but not present: {0}.
        The following identifiers were present, but not expected: {1}.
        """;

    public VariableMismatchException(IReadOnlyList<string> missing, IReadOnlyList<string> unexpected) :
        base(string.Format(Template, string.Join(", ", missing), string.Join(", ", unexpected)))
    {
    }

    public VariableMismatchException(IReadOnlyList<string> missing, IReadOnlyList<string> unexpected, Exception inner) :
        base(string.Format(Template, string.Join(", ", missing), string.Join(", ", unexpected)), inner)
    {
    }
}