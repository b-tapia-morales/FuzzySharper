using Knowledge.Memory.Abstractions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Abstractions;

namespace Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic;

public class EarliestCreated(IWorkingMemory memory) : IRuleComparer
{
    public IWorkingMemory Memory { get; } = memory;

    public int ComparerMethod(IFuzzySetRule x, IFuzzySetRule y) =>
        DateTimeOffset.Compare(x.CreationTime, y.CreationTime);
}