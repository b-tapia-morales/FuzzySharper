namespace Reasoning.Rule.Exceptions;

public class InvalidRuleException: InvalidOperationException
{
    private const string Template = "This rule is not structurally valid. A fuzzy rule must contain a premise and a consequent, and all components must be properly defined.";
    
    public InvalidRuleException() : base(Template)
    {
    }

    public InvalidRuleException(Exception inner) : base(Template, inner)
    {
    }
}