using Knowledge.Memory.Abstractions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Abstractions;

namespace Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic;

public class LatestCreated(IWorkingMemory memory) : IRuleComparer
{
    public IWorkingMemory Memory { get; } = memory;

    public int ComparerMethod(IFuzzySetRule x, IFuzzySetRule y) =>
        DateTimeOffset.Compare(y.CreationTime, x.CreationTime);
}