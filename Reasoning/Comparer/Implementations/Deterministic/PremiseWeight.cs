using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Family.Factory.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Comparer.Abstractions;
using Reasoning.Rule.Abstractions;
using Shared.Options.Factory;

namespace Reasoning.Comparer.Implementations.Deterministic;

public class PremiseWeight(IWorkingMemory memory, IOperatorFamily family) : IRuleComparer
{
    public IWorkingMemory Memory { get; } = memory;
    private IOperatorFamily Family { get; } = family;

    public PremiseWeight(IWorkingMemory memory) : this(memory, CanonicalFactory.UseFamily(CanonicalType.Godel))
    {
    }

    public int ComparerMethod(IFuzzySetRule x, IFuzzySetRule y)
    {
        var firstIsPresent = x.EvaluatePremiseWeight(Memory, Family).IsSomeVal(out var w1);
        var secondIsPresent = y.EvaluatePremiseWeight(Memory, Family).IsSomeVal(out var w2);
        return (firstIsPresent, secondIsPresent) switch
        {
            (false, false) => +0,
            (true, false) => -1,
            (false, true) => +1,
            (true, true) => w2.CompareTo(w1)
        };
    }
}