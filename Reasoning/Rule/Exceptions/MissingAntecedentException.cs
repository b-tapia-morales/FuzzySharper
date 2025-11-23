namespace Reasoning.Rule.Exceptions;

public class MissingAntecedentException : Exception
{
    private const string Template =
        """
        The following rule creation policy has been violated: 
        In order to append propositions with the connectives AND, OR, THEN, there must already be a proposition with the IF connective.
        """;

    public MissingAntecedentException() : base(Template)
    {
    }

    public MissingAntecedentException(Exception inner) : base(Template, inner)
    {
    }
}