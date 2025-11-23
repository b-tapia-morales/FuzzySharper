using Knowledge.Memory.Abstractions;
using Reasoning.Comparer.Abstractions;
using Reasoning.Rule.Abstractions;

namespace Reasoning.Comparer.Implementations.LearningBased;

public class FireCount(IWorkingMemory memory) : IRuleComparer
{
    public IWorkingMemory Memory { get; } = memory;

    public int ComparerMethod(IRule x, IRule y) => 
        y.AdaptationState.FireCount.CompareTo(x.AdaptationState.FireCount);
}