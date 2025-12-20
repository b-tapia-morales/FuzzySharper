namespace Inference.Defuzzifier.Exceptions;

public class ConsequentMismatchException : Exception
{
    private const string Template = "The rules' consequents must all be equal";

    public ConsequentMismatchException() : base(Template)
    {
    }

    public ConsequentMismatchException(Exception inner) : base(Template, inner)
    {
    }
}