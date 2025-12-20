namespace Reasoning.Rule.FuzzySet.Exceptions;

public class NegatedConsequentException : InvalidOperationException
{
    private const string Template =
        "The following rule creation policy has been violated: The proposition with the THEN connective cannot be in negated form.";

    public NegatedConsequentException() : base(Template)
    {
    }

    public NegatedConsequentException(Exception inner) : base(Template, inner)
    {
    }
}