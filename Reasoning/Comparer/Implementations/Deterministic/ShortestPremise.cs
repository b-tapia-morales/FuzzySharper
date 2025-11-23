using Knowledge.Memory.Abstractions;
using Reasoning.Comparer.Abstractions;
using Reasoning.Rule.Abstractions;

namespace Reasoning.Comparer.Implementations.Deterministic;

public class ShortestPremise(IWorkingMemory memory) : IRuleComparer
{
    public IWorkingMemory Memory { get; } = memory;

    public int ComparerMethod(IRule x, IRule y)
    {
        var a = x.PremiseLength();
        var b = y.PremiseLength();
        return a.CompareTo(b);
    }
}