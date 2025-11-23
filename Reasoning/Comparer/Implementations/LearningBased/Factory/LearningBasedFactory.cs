using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Abstractions;

namespace Reasoning.Comparer.Implementations.LearningBased.Factory;

public class LearningBasedFactory
{
    public static IComparer<IRule> GetInstance(LearningBasedMethod method, IWorkingMemory memory) =>
        method switch
        {
            LearningBasedMethod.LatestWeight => new LatestLearnedWeight(memory),
            LearningBasedMethod.AggregatedWeight => new AggregatedWeight(memory),
            LearningBasedMethod.FireCount => new FireCount(memory),
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };

    public static IComparer<IRule> GetInstance(LearningBasedMethod method, IWorkingMemory memory,
        IOperatorFamily family) =>
        method switch
        {
            LearningBasedMethod.LatestWeight => new LatestLearnedWeight(memory, family),
            LearningBasedMethod.AggregatedWeight => new AggregatedWeight(memory, family),
            LearningBasedMethod.FireCount => new FireCount(memory),
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };
}