using Knowledge.Memory.Abstractions;
using Reasoning.Comparer.Abstractions;
using Reasoning.Rule.Abstractions;

namespace Reasoning.Comparer.Implementations.Deterministic;

public class LatestCreated(IWorkingMemory memory) : IRuleComparer
{
    public IWorkingMemory Memory { get; } = memory;

    public int ComparerMethod(IRule x, IRule y) =>
        DateTimeOffset.Compare(y.CreationTime, x.CreationTime);
}