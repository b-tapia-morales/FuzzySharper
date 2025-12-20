using Inference.Aggregator.Abstractions;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Shared.Options.Implementations;

namespace Inference.Defuzzifier.Abstractions;

public interface IDefuzzifier
{
    Option<double> Defuzzify(ICollection<IFuzzySetRule> rules, IWorkingMemory memory,
        IOperatorFamily family, IValueAggregator aggregator, ImplicationMethod method,
        out ICollection<IFuzzySetRule> activatedRules);
    
    Option<double> Defuzzify(ICollection<IFuzzySetRule> rules, IWorkingMemory memory,
        IOperatorFamily family, ImplicationMethod method,
        out ICollection<IFuzzySetRule> activatedRules);
    
    Option<double> Defuzzify(ICollection<IFuzzySetRule> rules, IWorkingMemory memory,
        IValueAggregator aggregator, ImplicationMethod method,
        out ICollection<IFuzzySetRule> activatedRules);
    
    

    Option<double> Defuzzify(ICollection<IFuzzySetRule> rules, IWorkingMemory memory, 
        ImplicationMethod method,
        out ICollection<IFuzzySetRule> activatedRules);
}