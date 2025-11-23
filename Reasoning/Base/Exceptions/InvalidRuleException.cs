namespace Reasoning.Base.Exceptions;

public class InvalidRuleException : Exception
{
    private const string Template =
        "An attempt has been made to add an invalid rule (it has no Antecedent, or Consequent, or neither) to the rule base.";

    public InvalidRuleException() : base(Template)
    {
    }

    public InvalidRuleException(Exception inner) : base(Template, inner)
    {
    }
}