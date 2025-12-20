namespace Reasoning.Rule.FuzzySet.Exceptions;

public class VariableOverlapException : InvalidOperationException
{
    private const string Template =
        """
        The following rule creation policy has been violated: a linguistic variable cannot appear in both the premise and the consequent parts of the rule.
        Variable «{0}» was found in both parts of the rule.
        """;
    
    public VariableOverlapException(string variableName) : base(string.Format(Template, variableName))
    {
    }

    public VariableOverlapException(string variableName, Exception inner) : base(string.Format(Template, variableName), inner)
    {
    }
}