namespace Reasoning.Rule.Exceptions;

public class FinalizedRuleException : InvalidOperationException
{
    private const string Template =
        """
        The following rule creation policy has been violated: 
        It is not possible to append more propositions after a proposition with the connective THEN has been appended, as the rule has already finalized its instantiation.
        """;

    public FinalizedRuleException() : base(Template)
    {
    }

    public FinalizedRuleException(Exception inner) : base(Template, inner)
    {
    }
}