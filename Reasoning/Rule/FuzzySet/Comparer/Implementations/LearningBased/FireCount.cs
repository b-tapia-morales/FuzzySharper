using Knowledge.Memory.Abstractions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Abstractions;

namespace Reasoning.Rule.FuzzySet.Comparer.Implementations.LearningBased;

public class FireCount(IWorkingMemory memory) : IRuleComparer
{
    public IWorkingMemory Memory { get; } = memory;

    public int ComparerMethod(IFuzzySetRule x, IFuzzySetRule y) => 
        y.AdaptationState.FireCount.CompareTo(x.AdaptationState.FireCount);
}