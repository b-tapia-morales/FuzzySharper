namespace Reasoning.Rule.Functional.Policy.Implementations;

public class NormalizedMeanDevInit : MeanDevInit
{
    public NormalizedMeanDevInit () => Normalize = true;
}