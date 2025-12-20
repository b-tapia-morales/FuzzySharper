using Knowledge.Memory.Abstractions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Abstractions;

namespace Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic;

public class ShortestPremise(IWorkingMemory memory) : IRuleComparer
{
    public IWorkingMemory Memory { get; } = memory;

    public int ComparerMethod(IFuzzySetRule x, IFuzzySetRule y)
    {
        var a = x.PremiseLength;
        var b = y.PremiseLength;
        return a.CompareTo(b);
    }
}