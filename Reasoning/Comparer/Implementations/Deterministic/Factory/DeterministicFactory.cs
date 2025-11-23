using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Abstractions;

namespace Reasoning.Comparer.Implementations.Deterministic.Factory;

public static class DeterministicFactory
{
    public static IComparer<IRule> GetInstance(DeterministicMethod method, IWorkingMemory memory) =>
        method switch
        {
            DeterministicMethod.HighestPriority => new HighestPriority(memory),
            DeterministicMethod.ShortestPremise => new ShortestPremise(memory),
            DeterministicMethod.LargestPremise => new LargestPremise(memory),
            DeterministicMethod.EarliestCreated => new EarliestCreated(memory),
            DeterministicMethod.LatestCreated => new LatestCreated(memory),
            DeterministicMethod.PremiseWeight => new PremiseWeight(memory),
            DeterministicMethod.Certainty => new CertaintyComparer(memory),
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };

    public static IComparer<IRule> GetInstance(DeterministicMethod method, IWorkingMemory memory,
        IOperatorFamily family) =>
        method switch
        {
            DeterministicMethod.HighestPriority => new HighestPriority(memory),
            DeterministicMethod.ShortestPremise => new ShortestPremise(memory),
            DeterministicMethod.LargestPremise => new LargestPremise(memory),
            DeterministicMethod.EarliestCreated => new EarliestCreated(memory),
            DeterministicMethod.LatestCreated => new LatestCreated(memory),
            DeterministicMethod.PremiseWeight => new PremiseWeight(memory, family),
            DeterministicMethod.Certainty => new CertaintyComparer(memory, family),
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };
}