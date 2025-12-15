using Knowledge.Memory.Abstractions;
using Reasoning.Comparer.Abstractions;
using Reasoning.Rule.Abstractions;

namespace Reasoning.Comparer.Implementations.Deterministic;

public class HighestPriority(IWorkingMemory memory) : IRuleComparer
{
    public IWorkingMemory Memory { get; } = memory;

    public int ComparerMethod(IFuzzySetRule x, IFuzzySetRule y)
    {
        var a = x.Priority.Match(e => (int) e, _ => -1);
        var b = y.Priority.Match(e => (int) e, _ => -1);
        return b.CompareTo(a);
    }
}