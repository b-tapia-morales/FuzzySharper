namespace Reasoning.Rule.Functional;

public class FunctionalConsequent
{
    private uint Arity { get; } = 0;
    private IDictionary<string, double> CoefficientDict { get; } = new Dictionary<string, double>();
    private IList<double> Coefficients { get; } = [];
    private double Bias { get; } = 0;
    
    
}