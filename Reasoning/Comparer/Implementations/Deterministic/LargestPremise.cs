using Knowledge.Memory.Abstractions;
using Reasoning.Comparer.Abstractions;
using Reasoning.Rule.FuzzySet.Abstractions;

namespace Reasoning.Comparer.Implementations.Deterministic;

public class LargestPremise(IWorkingMemory memory) : IRuleComparer
{
    public IWorkingMemory Memory { get; } = memory;

    public int ComparerMethod(IFuzzySetRule x, IFuzzySetRule y)
    {
        var a = x.PremiseLength();
        var b = y.PremiseLength();
        return b.CompareTo(a);
    }
}