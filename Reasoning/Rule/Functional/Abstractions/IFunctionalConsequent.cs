using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Abstractions;
using Shared.Options.Implementations;

namespace Reasoning.Rule.Functional.Abstractions;

public interface IFunctionalConsequent: IRuleOutput
{
    IReadOnlyDictionary<string, double> CoefficientDict { get; }
    double Bias { get; }
    uint Arity { get; }
    
    bool IsEvaluable(IWorkingMemory memory);
    
    Option<double> Evaluate(IWorkingMemory memory);
}