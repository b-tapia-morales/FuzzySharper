namespace Reasoning.Rule.Exceptions;

public class DuplicatedAntecedentException : Exception
{
    private const string Template =
        "The following rule creation policy has been violated: There can be one and only one proposition with the IF connective.";

    public DuplicatedAntecedentException() : base(Template)
    {
    }

    public DuplicatedAntecedentException(Exception inner) : base(Template, inner)
    {
    }
}