namespace Inference.Defuzzifier.Exceptions;

public class InapplicableRulesException : Exception
{
    private const string Template =
        "There must be at least one rule that is applicable from the facts provided as parameters";

    public InapplicableRulesException() : base(Template)
    {
    }

    public InapplicableRulesException(Exception inner) : base(Template, inner)
    {
    }
}