namespace Reasoning.Rule.Functional.Exceptions;

public class CoefficientArityMismatchException : InvalidOperationException
{
    private const string Template =
        """
        The following rule creation policy has been violated: each unique premise identifier must correspond to exactly one coefficient.
        Expected {0} coefficients, but received {1} instead.
        """;

    public CoefficientArityMismatchException(int identifierCount, int coefficientCount) : base(string.Format(Template, identifierCount, coefficientCount))
    {
    }

    public CoefficientArityMismatchException(int identifierCount, int coefficientCount, Exception inner) : base(string.Format(Template, identifierCount, coefficientCount), inner)
    {
    }
}