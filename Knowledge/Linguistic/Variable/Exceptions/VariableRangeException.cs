using Shared.Intervals.Implementations;

namespace Knowledge.Linguistic.Variable.Exceptions;

public class VariableRangeException : Exception
{
    private const string Template =
        """
        An attempt has been made to add to a linguistic variable a linguistic entry whose Membership Function's range is outside of the variable's closed interval.
        The Linguistic Variable is: «{0}»; its Closed Interval is: {1}.
        The Linguistic Entry is: «{2}»; The range and type of its Membership Function is: {3} and «{4}» respectively
        """;

    public VariableRangeException(string variableName, Interval universeRange, string termName,
        Interval functionRange, string functionType) :
        base(string.Format(Template, variableName, universeRange.ToString(), termName, functionRange.ToString(),
            functionType))
    {
    }

    public VariableRangeException(string variableName, Interval universeRange, string termName,
        Interval functionRange, string functionType, Exception inner) :
        base(string.Format(Template, variableName, universeRange.ToString(), termName, functionRange.ToString(),
            functionType), inner)
    {
    }
}